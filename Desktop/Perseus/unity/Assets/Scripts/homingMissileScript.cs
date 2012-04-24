using UnityEngine;
using System.Collections;

public class homingMissileScript : MonoBehaviour
{
	
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
	private bool hasTarget = false;
	private float normalSpeed = 15;
	private float targetSpeed = 45;
	public AudioClip thrusters;
	public AudioClip explosion;
	public AudioClip linkestablished;
	public GameObject expSource;
	public GameObject explosionRadius;
	private GameObject target;
	// Use this for initialization
	void Start ()
	{
		myStyle = new GUIStyle();
		myStyle.font = warning;
		myStyle.alignment = TextAnchor.MiddleCenter;
		myStyle.fontSize = 50;
		myStyle.normal.textColor = Color.red;
		myStyle.normal.background = null;
		PlayAudioClip(explosion,transform.position,4f);
		this.audio.clip = thrusters;
		this.audio.volume = 20f;
		this.audio.Play();
		
	}
	
	public void init ()
	{
		begin = true;
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
	
	private bool ownedByCurrentPlayer ()
	{
		return networkView.viewID.owner == Network.player;
	}
	
	void Update ()
	{
		
		if(ownedByCurrentPlayer())
		{
		
		
		if(transform.position.y > 200 || transform.position.y < -200 || transform.position.x > 300 || transform.position.x
				<-300 || transform.position.z > 300 || transform.position.z < -300)
			{
			kill();
		}
		
		if(hasTarget)
		{
			//transform.parent.up = Vector3.Slerp(transform.forward,target.transform.position - transform.parent.position,Time.time/10000f);
			if(target==null)
			{
				hasTarget = false;
				return;
			}
			Vector3 direction = target.transform.position - transform.position;
			Vector3 currDirection = transform.forward;
			transform.forward = Vector3.Slerp(currDirection,direction,Time.deltaTime);
			transform.rotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x,transform.eulerAngles.y,0));
			/*if((direction - currDirection + transform.right).magnitude < (direction - currDirection).magnitude)
			{
				Left();
			}
			else if((direction - currDirection - transform.right).magnitude < (direction - currDirection).magnitude)
			{
				Right();
			}
			
			if((direction - currDirection + transform.up).magnitude < (direction - currDirection).magnitude)
			{
				Down();
			}
			else if((direction - currDirection - transform.up).magnitude < (direction - currDirection).magnitude)
			{
				Up();
			}*/
			
		}
		
		
		if(controlType == ControlType.Inset && rectWidth < .2)
		{
			rectWidth += .4f*Time.deltaTime;
			if(rectWidth >= .2f)
			{
				rectWidth = .2f;
			}
			viewRect = new Rect(.85f - rectWidth/2f,.15f - rectWidth/2f,rectWidth,rectWidth);
			cameraView.rect = viewRect;
		}
		else if(controlType == ControlType.None && rectWidth > 0)
		{
			rectWidth -= .4f*Time.deltaTime;
			if(rectWidth <= 0f)
			{
				rectWidth = 0f;
				cameraView.enabled = false;
			}
			viewRect = new Rect(.85f - rectWidth/2f,.15f - rectWidth/2f,rectWidth,rectWidth);
			cameraView.rect = viewRect;
			
		}
		
		if(controlType == ControlType.Full && !hasTarget)
		{
			float y = Input.GetAxis("Mouse Y");
			float x = Input.GetAxis("Mouse X");
			transform.Rotate(new Vector3(-y, x, 0) * Time.deltaTime * 40);
			transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x,transform.rotation.eulerAngles.y,0));
			//transform.parent.rotation = Quaternion.Euler(new Vector3(transform.parent.eulerAngles.x,0,-transform.parent.eulerAngles.z));
			/*if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
			{
				Up();
			}
			else if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
			{
				Down();
			}
			
			if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
			{
				Left();
			}
			else if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
			{
				Right();
			}*/
		}
		
		if(controlType == ControlType.Full && !justActivated)
		{
			if(Input.GetMouseButtonDown(0))
			{
				if(hasTarget)
				{
					hasTarget = false;
					target = null;
				}
				else
				{
					aquireTarget();
						
				}
			}
			if(Input.GetAxis("Mouse ScrollWheel") != 0)
			{
				transferControl();
			}
			if(Input.GetMouseButtonDown(1) || Input.GetKey(KeyCode.Space))
			{
				kill();
			}
		}
		
		if(controlType != ControlType.None)
		{
			if(Input.GetKey(KeyCode.Space))
			{
				kill();
			}
		}
			if(begin)
			{
				if(hasTarget)
				{
					transform.position = transform.position + transform.forward*targetSpeed*Time.deltaTime;
				}
				else
				{
					transform.position = transform.position + transform.forward*normalSpeed*Time.deltaTime;
				}
			}
		justActivated = false;
		}
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
	}
	
	void aquireTarget()
	{
		GameObject[] found = GameObject.FindGameObjectsWithTag("targetable");
		int foundAt = -1;
		float minMag = .2f;
		for(int x = 0; x < found.Length; x++)
		{
			Vector3 zCheck = cameraView.WorldToScreenPoint(found[x].transform.position);
			Vector3 tempCheck = cameraView.WorldToNormalizedViewportPoint(found[x].transform.position);
			if(zCheck.z >= 0)
			{
				Vector2 magCheck = new Vector2(tempCheck.x - .5f,tempCheck.y - .5f);
				if(magCheck.magnitude < minMag)
				{
					foundAt = x;
					minMag = magCheck.magnitude;
				}
			}
		}
		
		if(foundAt >= 0)
		{
			hasTarget = true;
			target = found[foundAt];
			PlayAudioClip(linkestablished,transform.position,6f);
		}
		
		
	}
	
	/*void Left()
	{
		float currentDampener = dampener;
		if(hasTarget)
		{
			currentDampener = homingDampener;
		}
		Vector3 up = transform.parent.up - transform.parent.right*Time.deltaTime/currentDampener;
		Vector3 forward = transform.parent.forward;
		transform.parent.rotation = Quaternion.LookRotation(forward,up);
	}
	
	void Right()
	{
		float currentDampener = dampener;
		if(hasTarget)
		{
			currentDampener = homingDampener;
		}
		Vector3 up = transform.parent.up + transform.parent.right*Time.deltaTime/currentDampener;
		Vector3 forward = transform.parent.forward;
		transform.parent.rotation = Quaternion.LookRotation(forward,up);
	}
	
	void Up()
	{
		float currentDampener = dampener;
		if(hasTarget)
		{
			currentDampener = homingDampener;
		}
		Vector3 up = transform.parent.up - transform.parent.forward*Time.deltaTime/currentDampener;
		Vector3 forward = transform.parent.forward + transform.parent.up*Time.deltaTime/currentDampener;
		transform.parent.rotation = Quaternion.LookRotation(forward,up);
		
	}
	
	void Down()
	{
		float currentDampener = dampener;
		if(hasTarget)
		{
			currentDampener = homingDampener;
		}
		Vector3 up = transform.parent.up + transform.parent.forward*Time.deltaTime/currentDampener;
		Vector3 forward = transform.parent.forward - transform.parent.up*Time.deltaTime/currentDampener;
		transform.parent.rotation = Quaternion.LookRotation(forward,up);
	}*/
	
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
	
	public void kill ()
	{
		if(networkView.viewID.owner == Network.player)
		{
			networkView.RPC("died",RPCMode.Others,transform.position);
			PlayAudioClip(explosion,transform.position,4f);
			transferControl();
			ParticleSystem engine = GetComponentInChildren<ParticleSystem>();
			if(engine != null)
			{
				engine.transform.parent = null;
				engine.enableEmission = false;
			}
			float explosionRad = 10;
			int halfHit = 10;
			Collider[] hitTurretts = Physics.OverlapSphere(transform.position,explosionRad,1<<10);
			foreach(Collider turrett in hitTurretts)
			{
					int hitFor = (int)((explosionRad - (turrett.transform.position - transform.position).magnitude)/explosionRad)*halfHit + halfHit;
					turrett.gameObject.networkView.RPC("hitTower",RPCMode.All,hitFor);
			}
			Vector3 pos = transform.position;
        	Network.Destroy(gameObject);
			Explode(pos);
		}
	}
	
	public void Explode(Vector3 pos)
	{
		float radius = 22;
		Collider[] projectiles = Physics.OverlapSphere(pos,radius,1<<13);
		foreach (Collider proj in projectiles)
		{
			if (proj.gameObject==gameObject || !proj.gameObject.active) continue;
			kill(proj.gameObject);
		}
		Network.Instantiate(explosionRadius,pos,Quaternion.identity,0);
	}
	
	void kill(GameObject g)
	{
		if (g.GetComponent<AIControlledMissile>()!=null) {
			g.GetComponent<AIControlledMissile>().gameObject.active = false;
			g.GetComponent<AIControlledMissile>().kill();
		}
		else if (g.GetComponent<AIresourceRobot>()!=null) {
			g.GetComponent<AIresourceRobot>().gameObject.active = false;
			g.GetComponent<AIresourceRobot>().kill();
		}
		else if (g.GetComponent<Bomb>()!=null) {
			g.GetComponent<Bomb>().gameObject.active = false;
			g.GetComponent<Bomb>().kill();
		}
		else if (g.GetComponent<Bullet>()!=null) {
			if (g.networkView.viewID.owner==Network.player)
				Network.Destroy(g);
		}
		else if (g.GetComponent<ControlledMissile>()!=null) {
			g.GetComponent<ControlledMissile>().gameObject.active = false;
			g.GetComponent<ControlledMissile>().kill();
		}
		else if (g.GetComponent<homingMissileScript>()!=null) {
			g.GetComponent<homingMissileScript>().gameObject.active = false;
			g.GetComponent<homingMissileScript>().kill();
		}
		else if (g.GetComponent<resourceRobot>()!=null) {
			g.GetComponent<resourceRobot>().gameObject.active = false;
			g.GetComponent<resourceRobot>().kill();
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
		if(engine != null)
		{
			engine.transform.parent = null;
			engine.enableEmission = false;
		}
	}
	[RPC]
	void setTag(string t)
	{
		gameObject.tag = t;
	}
}
	
	/*private bool leftTower = false;
	private homingMissileCamera mC;
	public AudioClip explosion;
	public GameObject expSource;
	private bool begin = false;
	private bool hasTarget = false;
	private float normalSpeed = 15;
	private float targetSpeed = 45;
	int lives = 3;
	// Use this for initialization
	void Start ()
	{
		audio.clip = explosion;
		audio.Stop();
	}
	
	private bool ownedByCurrentPlayer ()
	{
		return networkView.viewID.owner == Network.player;
	}
	
	public void init()
	{
		begin = true;
	}

	// Update is called once per frame
	void Update ()
	{
		if(mC == null)
		{
			mC = GetComponentInChildren<homingMissileCamera>();
		}
		if(begin)
		{
			float speed = normalSpeed;
			if(hasTarget)
			{
				speed = targetSpeed;
			}
			
			transform.Translate(transform.up*speed*Time.deltaTime,Space.World);
		}
		
		
	}
	
	public void toggleTarget()
	{
		hasTarget = !hasTarget;
	}
	
	void OnTriggerEnter(Collider other) {
		if(other.gameObject.name == "Bullet(Clone)")
		{
			lives--;
			if(lives<=0)
				kill();
		}
		else if(leftTower && ownedByCurrentPlayer() && other.gameObject.name != "Money(Clone)")
		{
			PlayAudioClip(explosion,transform.position,4f);
			mC.transferControl();
        	Network.Destroy(gameObject);
		}	
    }
	
	void OnTriggerExit(Collider other)
	{
		if(!leftTower && ownedByCurrentPlayer() && other.gameObject.name != "Money(Clone)")
		{
			audio.PlayOneShot(explosion,.5f);
			mC.transferControl();
			leftTower = true;
		}
	}
	
	public void kill()
	{
		if(ownedByCurrentPlayer())
		{
			networkView.RPC("died",RPCMode.Others,transform.position);
			PlayAudioClip(explosion,transform.position,4f);
			mC.transferControl();
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
	void setNavColor(Vector3 pColor, string nTag)
	{
		Debug.Log("Received notification to change nav color...");
		GetComponentInChildren<navPoint>().subRefresh(new Color(pColor.x,pColor.y,pColor.z,1f),nTag);
	}
	
	[RPC]
	void died(Vector3 pos)
	{
		PlayAudioClip(explosion,pos,4f);
		ParticleSystem engine = GetComponentInChildren<ParticleSystem>();
		engine.transform.parent = null;
		engine.enableEmission = false;
	}*/

