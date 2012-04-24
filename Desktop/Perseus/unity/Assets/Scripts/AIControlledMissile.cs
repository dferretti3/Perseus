using UnityEngine;
using System.Collections;

public class AIControlledMissile : MonoBehaviour
{
	
	private bool started = false;
	public Transform toMove;
	private float speed = 25;
	private GameObject target;
	private ControlType controlType = ControlType.None;
	public Camera cameraView;
	private Rect viewRect = new Rect (0f, 0f, 0f, 0f);
	private float rectWidth = 0f;
	private bool justActivated = false;
	private int lives = 3;
	public topLevelController tLC;
	public AudioClip explosion;
	public GameObject expSource;
	public AudioClip thrusters;
	// Use this for initialization
	void Start ()
	{
		PlayAudioClip(explosion,transform.position,4f);
		this.audio.clip = thrusters;
		this.audio.volume = 20f;
		this.audio.Play();
	}
	
	public void init ()
	{
		started = true;
	}
	
	public void giveTarget (GameObject bullseye)
	{
		target = bullseye;
	}
	
	private bool ownedByCurrentPlayer ()
	{
		return transform.parent.networkView.viewID.owner == Network.player;
	}
	
	// Update is called once per frame
	void Update ()
	{
		
		if (networkView.viewID.owner == Network.player) {
			if (controlType == ControlType.Inset && rectWidth < .2) {
				rectWidth += .4f * Time.deltaTime;
				if (rectWidth >= .2f) {
					rectWidth = .2f;
				}
				viewRect = new Rect (.85f - rectWidth / 2f, .15f - rectWidth / 2f, rectWidth, rectWidth);
				cameraView.rect = viewRect;
			} else if (controlType == ControlType.None && rectWidth > 0) {
				rectWidth -= .4f * Time.deltaTime;
				if (rectWidth <= 0f) {
					rectWidth = 0f;
					cameraView.enabled = false;
				}
				viewRect = new Rect (.85f - rectWidth / 2f, .15f - rectWidth / 2f, rectWidth, rectWidth);
				cameraView.rect = viewRect;
			
			}
		
			if (controlType == ControlType.Full && !justActivated) {
			
				if (Input.GetAxis ("Mouse ScrollWheel") != 0) {
					transferControl ();
				}
				if (Input.GetMouseButtonDown (0)) {
					kill ();
				}
			
			}
		
			if (started) {
				toMove.transform.position = toMove.transform.position + toMove.transform.forward * speed * Time.deltaTime;
				if (target == null) {
					GameObject[] possibleTargets = GameObject.FindGameObjectsWithTag ("turrett");
					for (int x = 0; x < possibleTargets.Length; x++) {
						if (possibleTargets [x].networkView.owner != Network.player) {
							target = possibleTargets [x];
							break;
						}
					}
					target = GameObject.Find("turrettSystem(Clone)");
				}
				if (target != null) {
					Vector3 realTargetPos = target.transform.position;
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
					if (Physics.SphereCast (rToTarget, 2.2f, out hitInfo, rayDist, 5 << 8)) {
							if(hitInfo.transform.tag == "turrett")
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
					transform.forward = Vector3.Slerp (transform.forward, realTargetPos - transform.position, Time.deltaTime);
				}
			}
		}
		justActivated = false;
	}
	
	void OnTriggerEnter (Collider col)
	{
		if(!started)
		{
			return;
		}
		if (col.gameObject.name == "Bullet(Clone)") {
			lives--;
			if (lives <= 0)
				kill ();
		} else if (col.gameObject.name != "Money(Clone)") {
			Debug.Log("Killed by: " + col.gameObject.name);
			//Debug.Break();
			kill ();
		}

	}

	public void makeActive ()
	{
		Screen.lockCursor = true;
		cameraView.enabled = true;
		controlType = ControlType.Full;
		justActivated = true;
		cameraView.rect = new Rect (0f, 0f, 1f, 1f);
		AudioListener aL = cameraView.gameObject.GetComponent<AudioListener> ();
		aL.enabled = true;
		
	}
	
	public void transferControl ()
	{ 
		if (controlType == ControlType.Full) {
			Screen.lockCursor = false;
			AudioListener aL = cameraView.gameObject.GetComponent<AudioListener> ();
			aL.enabled = false;
			tLC.moveToFirstPerson ();
			cameraView.enabled = false;
			controlType = ControlType.None;
			cameraView.rect = new Rect (.85f, .15f, 0f, 0f);
		}
	}
	
	public void openMiniScreen ()
	{
		if (controlType == ControlType.None) {
			cameraView.enabled = true;
			controlType = ControlType.Inset;
		} else {
			controlType = ControlType.None;
		}
	}
	
	public bool isMiniScreenOpen()
	{
		return controlType == ControlType.Inset;
	}
	
	private void kill ()
	{
		if (networkView.viewID.owner == Network.player) {
			networkView.RPC("died",RPCMode.Others,transform.position);
			PlayAudioClip(explosion,transform.position,4f);
			transferControl ();
			ParticleSystem engine = GetComponentInChildren<ParticleSystem>();
			engine.transform.parent = null;
			engine.enableEmission = false;
			float explosionRad = 10;
			int halfHit = 10;
			Collider[] hitTurretts = Physics.OverlapSphere(transform.position,explosionRad,1<<10);
			foreach(Collider turrett in hitTurretts)
			{
				topLevelController ttlc = turrett.transform.GetComponentInChildren<topLevelController>();
				int hitFor = (int)(explosionRad - (turrett.transform.position - transform.position).magnitude)*halfHit + halfHit;
				turrett.networkView.RPC("hitTower",turrett.networkView.owner,hitFor);
			}
			Network.Destroy (gameObject);
		}
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
	void setNavColor (Vector3 pColor, string nTag)
	{
		GetComponentInChildren<navPoint> ().subRefresh (new Color (pColor.x, pColor.y, pColor.z, 1f), nTag);
	}
	
	[RPC]
	void died(Vector3 pos)
	{
		PlayAudioClip(explosion,pos,4f);
		ParticleSystem engine = GetComponentInChildren<ParticleSystem>();
		engine.transform.parent = null;
		engine.enableEmission = false;
	}
}
