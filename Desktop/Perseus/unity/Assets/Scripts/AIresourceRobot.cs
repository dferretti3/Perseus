using UnityEngine;
using System.Collections;

public class AIresourceRobot : MonoBehaviour {
	private GameObject moneyTarget;
	private Vector3 tempTarget;
	
	float turnspeed = 2.0f;
	float flyspeed = 1.0f;
	
	int view = -1;
	int invert = 1;
	private bool begin = false;
	
	int lives = 3;
	public AudioClip explosion;
	public GameObject expSource;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if(moneyTarget == null)
		{
			Collider[] coins = Physics.OverlapSphere(transform.position,200f,1<<11);
			int foundAt = -1;
			float minDist = 200;
			for(int x = 0; x < coins.Length; x++)
			{
				Vector3 toMoneyVec = coins[x].transform.position - transform.position;
				if(toMoneyVec.magnitude < minDist)
				{
					Ray toMoney = new Ray(transform.position,toMoneyVec);
					RaycastHit outHit;
					if(!Physics.Raycast(toMoney,out outHit,toMoneyVec.magnitude,1<<8))
					{
						minDist = toMoneyVec.magnitude;
						foundAt = x;
					}
				}
			}
			if(foundAt > -1)
			{
				moneyTarget = coins[foundAt].gameObject;
			}
		}
		
		if(tempTarget == null)
		{
			float x = Random.value*600f - 300f;
			float z = Random.value*600f - 300f;
			float y = Terrain.activeTerrain.SampleHeight(new Vector3(x,0,z));
			tempTarget = new Vector3(x,y + 5 + Random.value*15,z);
		}
		
		Vector3 currentTarget;
		if(moneyTarget != null)
		{
			currentTarget = moneyTarget.transform.position;
		}
		else
		{
			currentTarget = tempTarget;
		}
		
		Transform toMove = transform;
					Vector3 realTargetPos = currentTarget;
					float bigDistance = (realTargetPos - transform.position).magnitude;
					bool blocked = true;
					float offset = 0;
					while(blocked && offset < 100)
					{
						offset += 10;
						blocked = false;
					Vector3 toTarget = realTargetPos - toMove.position;
					Ray rToTarget = new Ray (toMove.position, toTarget);
					RaycastHit hitInfo;
						float rayDist = toTarget.magnitude + 3f;
						if(rayDist > 50)
						{
							rayDist = 50f;
						}
					if (Physics.SphereCast (rToTarget, 5, out hitInfo, rayDist, 9 << 8)) {
						if(hitInfo.collider.gameObject.tag == "money")
						{
						break;
						}
						blocked = true;
						RaycastHit LeftHitInfo, RightHitInfo, UpHitInfo, DownHitInfo;
						//Debug.Log("spherecast hit!");
						Ray leftTarget, rightTarget, upTarget, downTarget;
						int direction = 0;
						float maxHitDist = 9000;
						leftTarget = new Ray (toMove.transform.position - Vector3.Cross (toTarget.normalized, Vector3.up) * 5, toTarget);
						rightTarget = new Ray (toMove.transform.position + Vector3.Cross (toTarget.normalized, Vector3.up) * 5, toTarget);
						upTarget = new Ray (toMove.transform.position + new Vector3 (0, 5, 0), toTarget);
						downTarget = new Ray (toMove.transform.position - new Vector3 (0, 3, 0), toTarget);
					
						if (Physics.Raycast (leftTarget, out LeftHitInfo, bigDistance, 1 << 8)) {
							maxHitDist = LeftHitInfo.distance;
							//Debug.DrawRay(leftTarget.origin,leftTarget.direction,Color.red,.5);
							if (Physics.Raycast (rightTarget, out RightHitInfo, bigDistance, 1 << 8)) {
								if (maxHitDist < RightHitInfo.distance) {
									direction = 1;
									maxHitDist = RightHitInfo.distance;
								}
							
								if (Physics.Raycast (upTarget, out UpHitInfo, bigDistance, 1 << 8)) {
									if (maxHitDist < UpHitInfo.distance) {
										direction = 2;
										maxHitDist = UpHitInfo.distance;
									}
								
									if (Physics.Raycast (downTarget, out DownHitInfo, bigDistance, 1 << 8)) {
										if (maxHitDist < DownHitInfo.distance) {
											direction = 3;
											maxHitDist = DownHitInfo.distance;
										}
									
									
										if (direction == 0) {
											realTargetPos = hitInfo.point + Vector3.Cross (toTarget.normalized, Vector3.up) * offset;
										} else if (direction == 1) {
											realTargetPos = hitInfo.point + Vector3.Cross (toTarget.normalized, Vector3.up) * offset;
										} else if (direction == 2) {
											realTargetPos = hitInfo.point + new Vector3 (0, offset, 0);
										} else if (direction == 3) {
											realTargetPos = hitInfo.point - new Vector3 (0, offset, 0);
										}
									
									
									} else {
										realTargetPos = hitInfo.point - new Vector3 (0, offset, 0);
									}
								
								} else {
									realTargetPos = hitInfo.point + new Vector3 (0, offset, 0);
								}
							
							} else {
								realTargetPos = hitInfo.point + Vector3.Cross (toTarget.normalized, Vector3.up) * offset;
							}
						} else {
							realTargetPos = hitInfo.point - Vector3.Cross (toTarget.normalized, Vector3.up) * offset;
						}
					
					}
					else{
						//Debug.Log("Spherecast not hit");
					}
					}
					Debug.DrawLine(transform.position,realTargetPos,Color.red,.3f);
					if(moneyTarget != null)
					{
						Debug.DrawLine(transform.position,moneyTarget.transform.position,Color.black,.3f);
					}
					transform.forward = Vector3.Slerp (transform.forward, realTargetPos - transform.position, Time.deltaTime*.1f);
		transform.position = transform.position + (realTargetPos - transform.position).normalized* 3 * Time.deltaTime;
	}
	
	void OnTriggerEnter (Collider col)
	{	
		if(col.gameObject.name == "Bullet(Clone)")
		{
			lives--;
			if(lives<=0)
				kill();
		}
		else if(col.gameObject.name != "Money(Clone)")
		{
			kill();
		}

	}
	
	private void kill ()
	{
		networkView.RPC("died",RPCMode.Others,transform.position);
		PlayAudioClip(explosion,transform.position,4f);
		ParticleSystem engine = GetComponentInChildren<ParticleSystem>();
		float explosionRad = 10;
		int halfHit = 40;
		Collider[] hitTurretts = Physics.OverlapSphere(transform.position,explosionRad,1<<10);
		foreach(Collider turrett in hitTurretts)
		{
			topLevelController ttlc = turrett.transform.GetComponentInChildren<topLevelController>();
			int hitFor = (int)(explosionRad - (turrett.transform.position - transform.position).magnitude)*halfHit + halfHit;
			turrett.networkView.RPC("hitTower",turrett.networkView.owner,hitFor);
		}
    	Network.Destroy(gameObject);
	}
	
	AudioSource PlayAudioClip(AudioClip clip, Vector3 position, float volume) {
        GameObject go = (GameObject)Instantiate(expSource);
        go.transform.position = position;
        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.Play();
        Destroy(go, clip.length);
        return source;
    }
	
	[RPC]
	void died(Vector3 pos)
	{
		PlayAudioClip(explosion,pos,4f);
		ParticleSystem engine = GetComponentInChildren<ParticleSystem>();
	}
}
