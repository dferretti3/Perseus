using UnityEngine;
using System.Collections;

public class resourceRobot : MonoBehaviour
{
	private GameObject moneyTarget;
	private Vector3 tempTarget;
	public topLevelController tLC;
	private ControlType controlType = ControlType.None;
	private Rect viewRect = new Rect (0f, 0f, 0f, 0f);
	private float rectWidth = 0f;
	private bool justActivated = false;
	float turnspeed = 2.0f;
	float flyspeed = 1.0f;
	public Camera cameraView;
	int view = -1;
	int invert = 1;
	private bool begin = false;
	GUIStyle myStyle;
	public Font warning;
	public Texture2D videoBorder;
	int lives = 3;
	public AudioClip explosion;
	public GameObject expSource;
	// Use this for initialization
	void Start ()
	{
	
	}
	
	void OnGUI(){

		if(networkView.viewID.owner == Network.player)
		{

			if(controlType == ControlType.Inset)
			{
				Rect currentScreen = cameraView.rect;
				currentScreen.x = currentScreen.x*Screen.width;
				currentScreen.width = currentScreen.width*Screen.width + 2;
				currentScreen.height = currentScreen.height*Screen.height + 2;
				currentScreen.y = Screen.height - currentScreen.y*Screen.height - currentScreen.height;
				GUI.DrawTexture(currentScreen,videoBorder);
			}
			else if(controlType == ControlType.Full)
			{
				if(transform.position.y > 150 || transform.position.y < -150 || transform.position.x > 250 || transform.position.x
				<-250 || transform.position.z > 250 || transform.position.z < -250)
				{
					GUI.TextArea(new Rect(Screen.width/2 - 125,Screen.height*3/4,250,60),"WARNING",myStyle);
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		
		if (networkView.viewID.owner != Network.player) {
			return;
		}
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
			
		if(controlType != ControlType.Full)
		{
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
		else if(!justActivated)
		{
			float y = Input.GetAxis("Mouse Y");
			float x = Input.GetAxis("Mouse X");
			transform.Rotate(new Vector3(-y, x, 0) * Time.deltaTime * 50);
			transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x,transform.rotation.eulerAngles.y,0));
			
			if(Input.GetKeyDown(KeyCode.O) || Input.GetKeyDown(KeyCode.P) || Input.GetAxis ("Mouse ScrollWheel") != 0) {
					transferControl ();
			}
			if(Input.GetMouseButtonDown(0))
			{
				kill();
			}
			
			if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
			{
				transform.position = transform.position + transform.forward*5*Time.deltaTime;
			}
			else if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
			{
				transform.position = transform.position - transform.right*5*Time.deltaTime;
			}
			if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
			{
				transform.position = transform.position - transform.forward*5*Time.deltaTime;
			}
			else if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
			{
				transform.position = transform.position + transform.right*5*Time.deltaTime;
			}
			if(Input.GetKey(KeyCode.Q))
			{
				transform.position = transform.position - transform.up*5*Time.deltaTime;
			}
			else if(Input.GetKey(KeyCode.E))
			{
				transform.position = transform.position + transform.up*5*Time.deltaTime;
			}
			
			
			
			
			
		}
		
		
		if (controlType != ControlType.None) {
				if (Input.GetKey (KeyCode.Space)) {
					kill ();
				}
		}
		justActivated = false;
	
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

	public void makeActive ()
	{
		Screen.lockCursor = true;
		cameraView.enabled = true;
		controlType = ControlType.Full;
		justActivated = true;
		cameraView.rect = new Rect (0f, 0f, 1f, 1f);
		AudioListener aL = cameraView.gameObject.GetComponent<AudioListener> ();
		aL.enabled = true;
		moneyTarget = null;
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
		if(networkView.viewID.owner == Network.player)
		{
			networkView.RPC("died",RPCMode.Others,transform.position);
			PlayAudioClip(explosion,transform.position,4f);
			transferControl();
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
	}
}

