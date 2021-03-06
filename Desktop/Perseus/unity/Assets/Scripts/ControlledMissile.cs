using UnityEngine;
using System.Collections;

public class ControlledMissile : MonoBehaviour
{
	private static GameObject expr;
	
	public topLevelController tLC;
	private ControlType controlType = ControlType.None;
	private Rect viewRect = new Rect (0f, 0f, 0f, 0f);
	private float rectWidth = 0f;
	private bool justActivated = false;
	float turnspeed = 2.0f;
	float flyspeed = 0.8f;
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
	public GameObject explosionRadius;
	public AudioClip thrusters;
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
		this.audio.volume = 10f;
		this.audio.Play();
		expr = explosionRadius;
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
		
			if (begin) {
				transform.position += transform.forward * flyspeed;
			}
		
			if (controlType == ControlType.Full && !justActivated) {
				if (Input.GetAxis ("Mouse ScrollWheel") != 0) {
					transferControl ();
				}
				if (Input.GetMouseButtonDown (1)) {
					kill ();
				}
				float y = Input.GetAxis("Mouse Y");
				float x = Input.GetAxis("Mouse X");
				transform.Rotate(new Vector3(-y, x, 0) * Time.deltaTime * 60);
				transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x,transform.rotation.eulerAngles.y,0));
			}
		
			if (controlType != ControlType.None) {
				if (Input.GetKey (KeyCode.Space)) {
					kill ();
				}
			}
			if(transform.position.y > 200 || transform.position.y < -200 || transform.position.x > 300 || transform.position.x
				<-300 || transform.position.z > 300 || transform.position.z < -300)
			{
				kill();
			}
			justActivated = false;
		}
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
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
		
	}
	
	public void transferControl ()
	{ 
		if (controlType == ControlType.Full && tLC != null) {
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
				turrett.networkView.RPC("hitTower",RPCMode.All,hitFor);
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
