using UnityEngine;
using System.Collections;

public class firstPersonTower : MonoBehaviour {
	
	public ControlType controlType = ControlType.None;
	//public Font warning;	
	private topLevelController tLC;
	private bool justActivated = false;
	private float targetSize;
	public Texture2D target;
	public Texture2D homing;
	public Texture2D controlled;
	public Texture2D gatherer;
	public Texture2D machinegun;
	public Texture2D white;
	public Texture2D defense;
	public Texture2D mortar;
	private float missileButtonWidth;
	private float missileButtonHeight;
	public GameObject[] missilePrefab;
	public AudioClip commandcodes;
	public AudioClip warning;
	public AudioClip dsystem;
	public GameObject expSource;
		float turnspeed = 2.0f;
	public AudioClip shot;
	int highlightx=8;
	int highlighty;
	bool warn=false;
	int playcount=0;
	bool delaying = false;
	float delaycount = 0.33f;
	public Font myFont;
	LineRenderer lineRenderer;
	// Use this for initialization
	void Start () {
		targetSize = Screen.width/10;
		missileButtonWidth = Screen.width/10;
		missileButtonHeight = Screen.height/10;
		Screen.lockCursor = true;
		lineRenderer = transform.parent.GetComponentInChildren<LineRenderer>();
	}
	
	
	
	void OnGUI()
	{
		if(controlType == ControlType.Full)
		{
			GUIStyle myStyle = new GUIStyle();
			myStyle.font = myFont;
			myStyle.normal.textColor = Color.white;
			GUI.DrawTexture(new Rect(Screen.width/2 - targetSize/2,Screen.height/2 - targetSize/2,targetSize,targetSize),target);
			string currentMissile = "";
			if(tLC != null)
			{
				if(tLC.currentMissileSelection == 0)
				{
					currentMissile = "HOMING MISSILE";
					highlighty = 75;
				}
				else if(tLC.currentMissileSelection == 1)
				{
					currentMissile = "CONTROLLED MISSILE";
					highlighty = 135;
				}
				else if(tLC.currentMissileSelection == 2)
				{
					currentMissile = "MACHINE GUN";
					highlighty = 195;
				}
				else if(tLC.currentMissileSelection == 3)
				{
					currentMissile = "COLLECTOR";
					highlighty = 255;
				}
				else if(tLC.currentMissileSelection == 4)
				{
					currentMissile = "DEFENSE SYSTEM";
					highlighty = 315;
				}
				else if (tLC.currentMissileSelection == 5)
				{
					currentMissile = "MORTAR";	
					highlighty = 375;
				}
				
				GUI.Label(new Rect(highlightx,highlighty,60,60), white);
				GUI.DrawTexture(new Rect(10,80,50,50), homing);
				GUI.DrawTexture(new Rect(10,140,50,50), controlled);
				GUI.DrawTexture(new Rect(10,200,50,50), machinegun);
				GUI.DrawTexture(new Rect(10,260,50,50), gatherer);
				GUI.DrawTexture(new Rect(10,320,50,50), defense);
				GUI.DrawTexture(new Rect(10,380,50,50), mortar);
				GUI.Label(new Rect(0,0,300,50),"\n\t\t" + currentMissile, myStyle);
			}
		}
	}
	
	
	
	void fireMissile(int missileTypeNum)
	{
		missileType mType;
		if(missileTypeNum == 0)
		{
			mType = missileType.Homing;
		}
		else if(missileTypeNum == 1)
		{
			mType = missileType.Controlled;
		}
		else if(missileTypeNum == 2)
		{
			mType = missileType.Static;
		}
		else if(missileTypeNum == 3)
		{
			mType = missileType.Collector;
		}
		else if(missileTypeNum == 4)
		{
			mType = missileType.DefenseSystem;
		}
		else if (missileTypeNum == 5)
		{
			mType = missileType.Mortar;
		}
		else
		{
			return;
		}
		int prefabNum = -1;
		GameObject tempMissile;
		if(mType == missileType.Homing)
		{
			prefabNum = 0;
			tempMissile = (GameObject)Network.Instantiate(missilePrefab[prefabNum],new Vector3(-1000f,-1000f,-1000f),Quaternion.identity,0);
			tempMissile.networkView.RPC("setTag", RPCMode.All, ""+tLC.teamNum);
			if(!tLC.moveToMissile(0)&&PlayerPrefs.GetFloat("money")>10)
			{
				if(tLC.addNewMissile(tempMissile,mType))
				{
					PlayerPrefs.SetFloat("money", PlayerPrefs.GetFloat("money")-10);
					tempMissile.transform.rotation = Quaternion.LookRotation(transform.parent.forward,transform.parent.up);
					tempMissile.transform.position = transform.parent.position + transform.parent.forward*20;
					homingMissileScript msS = tempMissile.GetComponent<homingMissileScript>();
					msS.init();
				}
			}
			else
			{
				Destroy(tempMissile);
				if(tLC.moveToMissile(0))
				{
					cleanUpOnExit();
				}
			}
		}
		else if(mType == missileType.Controlled)
		{
			prefabNum = 1;
			tempMissile = (GameObject)Network.Instantiate(missilePrefab[prefabNum],new Vector3(-1000f,-1000f,-1000f),Quaternion.identity,0);
			tempMissile.networkView.RPC("setTag", RPCMode.All, ""+tLC.teamNum);
			if(!tLC.moveToMissile(1)&&PlayerPrefs.GetFloat("money")>10)
			{
				if(tLC.addNewMissile(tempMissile,mType))
				{
					PlayerPrefs.SetFloat("money", PlayerPrefs.GetFloat("money")-10);
					tempMissile.transform.rotation = Quaternion.LookRotation(transform.parent.forward,transform.parent.up);
					tempMissile.transform.position = transform.parent.position+transform.forward*20 ;
					ControlledMissile msS = tempMissile.GetComponent<ControlledMissile>();
					msS.init();
				}
			}
			else
			{
				Destroy(tempMissile);
				if(tLC.moveToMissile(1))
				{
					cleanUpOnExit();
				}
			}
		}
		else if(mType == missileType.Static)
		{
			if(!delaying)
			{
				prefabNum = 2;
				if(PlayerPrefs.GetFloat("money")>=0.5f)
				{
					PlayerPrefs.SetFloat("money", PlayerPrefs.GetFloat("money")-0.5f);
					tempMissile = (GameObject)Network.Instantiate(missilePrefab[prefabNum],transform.parent.position+transform.forward*20,Quaternion.LookRotation(transform.parent.forward,transform.parent.up),0);
					PlayAudioClip(shot,transform.position,0.3f);
					delaying=true;
					delaycount=0.33f;
				}
			}
		}
		else if(mType == missileType.Collector)
		{
			prefabNum = 3;
			tempMissile = (GameObject)Network.Instantiate(missilePrefab[prefabNum],new Vector3(-1000f,-1000f,-1000f),Quaternion.identity,0);
			if(!tLC.moveToMissile(3)&&PlayerPrefs.GetFloat("money")>30)
			{
				Debug.Log("Sending message to controller");
				if(tLC.addNewMissile(tempMissile,mType))
				{
					PlayerPrefs.SetFloat("money", PlayerPrefs.GetFloat("money")-30);
					tempMissile.transform.rotation = Quaternion.LookRotation(transform.parent.forward,transform.parent.up);
					tempMissile.transform.position = transform.position+Vector3.up*8 ;
					PlayAudioClip(commandcodes,transform.position,2f);
				}
			}
			else
			{
				Destroy(tempMissile);
				if(tLC.moveToMissile(3))
				{
					cleanUpOnExit();
				}
			}
		}
		else if(mType == missileType.DefenseSystem)
		{
			prefabNum = 4;
			tempMissile = (GameObject)Network.Instantiate(missilePrefab[prefabNum], transform.parent.position, Quaternion.identity,0);
			if(tLC.addNewMissile(tempMissile,mType) && PlayerPrefs.GetFloat("money")>=50)
			{
				PlayerPrefs.SetFloat("money", PlayerPrefs.GetFloat("money")-50f);
				tempMissile.transform.position = transform.parent.position;
				tempMissile.transform.parent = transform.parent;
				tempMissile.transform.localRotation = Quaternion.identity;
				PlayAudioClip(dsystem,transform.position,2f);
			}
			else
			{
				Destroy(tempMissile);
			}
		}
		else if(mType == missileType.Mortar)
		{
			prefabNum = 5;
			if(PlayerPrefs.GetFloat("money")>=20f)
			{
				PlayerPrefs.SetFloat("money", PlayerPrefs.GetFloat("money")-20f);
				tempMissile = (GameObject)Network.Instantiate(missilePrefab[prefabNum],transform.parent.position+transform.forward*20,Quaternion.LookRotation(transform.parent.forward,transform.parent.up),0);
				tempMissile.networkView.RPC("setTag", RPCMode.All, ""+tLC.teamNum);
				PlayAudioClip(shot,transform.position,0.3f);
			}
		}
		if(missilePrefab.Length <= prefabNum || prefabNum < 0)
		{
			return;
		}
	}
	
	
	public void makeActive()
	{
		Screen.lockCursor = true;
		controlType = ControlType.Full;
		camera.enabled = true;
		AudioListener aL = gameObject.GetComponent<AudioListener>();
		aL.enabled = true;
		justActivated = true;
	}
	
	// Update is called once per frame
	void Update () {
		if(delaying)
			delaycount -= Time.deltaTime;
		if(delaycount <= 0)
			delaying=false;
		if(tLC == null)
		{
			tLC = transform.parent.GetComponentInChildren<topLevelController>();
		}
		
		if(controlType == ControlType.Full && !justActivated)
		{
			float yAngle = Mathf.Asin(transform.parent.forward.y/Mathf.Abs(transform.parent.forward.magnitude));
			
			Collider[] cols = Physics.OverlapSphere(transform.position, 150);
			foreach (Collider hit in cols){
				if((hit.gameObject.name == "AIContMissile(Clone)" || hit.gameObject.name == "homingMissileRedo(Clone)" || hit.gameObject
				.name == "ControlledMissile(Clone)"))
				{
					GameObject closeobject = hit.gameObject;	
					if(closeobject.tag.CompareTo(""+tLC.teamNum)!=0)
					{
						if(!warn)
						PlayAudioClip(warning, transform.position, 2f);
						warn = true;
						break;
					}
				}	
		}
		if(warn)
		{
			playcount++;
			if(playcount>=400)
			{
				warn=false;
				playcount=0;
			}
		}
			
			
			
			if (tLC.currentMissileSelection==5)
			{
				lineRenderer.enabled = true;
				int max=200;
				int v;
				Vector3 pos = transform.parent.position+transform.forward*20;
				Vector3 vel = transform.forward*Bomb.projectilespeed + Bomb.initialVelocityOffset;
				lineRenderer.SetVertexCount(max);
				float dt = Time.fixedDeltaTime;
				for (v=0; v < max && pos.y>0; v++)
				{
					vel += Bomb.gravity*Vector3.down*dt;
					pos += vel*dt;
					lineRenderer.SetPosition(v,pos);
				}
				lineRenderer.SetVertexCount(v);
			} else{
				lineRenderer.enabled = false;
			}
			
			
			if(Input.GetMouseButtonDown(0))
			{
				fireMissile(tLC.currentMissileSelection);
			}
			else if(Input.GetMouseButtonDown(1))
			{
				tLC.openMiniScreen(tLC.currentMissileSelection);
			}
			else if(Input.GetMouseButtonDown(2))
			{
				if(tLC == null)
				{
					tLC = transform.parent.GetComponentInChildren<topLevelController>();
					if(tLC != null)
					{
						cleanUpOnExit();
						tLC.moveToThirdPerson();
					}
				}
				else
				{
					cleanUpOnExit();
					tLC.moveToThirdPerson();
				}
			}
			
			if(Input.GetKeyDown(KeyCode.Tab))
			{
				tLC.manager.scrollFromTab();
			}
			float scrollValue = Input.GetAxis("Mouse ScrollWheel");
			scrollValue = scrollValue*10;
			if(scrollValue != 0)
			{
				tLC.scrollMissileSelection(Mathf.RoundToInt(scrollValue));
			}
			else if(Input.GetKeyDown(KeyCode.P))
			{
				tLC.scrollMissileSelection(1);
			}
			else if(Input.GetKeyDown(KeyCode.O))
			{
				tLC.scrollMissileSelection(-1);
			}
			
			
			
			if(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
			{
				transform.parent.RotateAroundLocal(Vector3.up,-Mathf.PI*Time.deltaTime/20);
			}
			else if(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
			{
				transform.parent.RotateAroundLocal(Vector3.up,Mathf.PI*Time.deltaTime/20);
			}
			
			
			if((Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) && yAngle < Mathf.PI/8)
			{
				transform.parent.RotateAroundLocal(transform.right,-Mathf.PI*Time.deltaTime/20);
			}
			else if((Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))  && yAngle > -Mathf.PI/8)
			{
				transform.parent.RotateAround(transform.right,Mathf.PI*Time.deltaTime/20);
			}
	
			float y = Input.GetAxis("Mouse Y");
			float x = Input.GetAxis("Mouse X");
			transform.parent.Rotate(new Vector3(-y, 0, 0) * Time.deltaTime * 50);
			transform.parent.Rotate(new Vector3(0, x, 0) * Time.deltaTime * 50);
    		transform.parent.eulerAngles = new Vector3(transform.parent.eulerAngles.x,transform.parent.eulerAngles.y, 0);
			
			if(Input.GetKeyDown(KeyCode.Q))
			{
				if(tLC == null)
				{
					tLC = transform.parent.GetComponentInChildren<topLevelController>();
					if(tLC != null)
					{
						cleanUpOnExit();
						tLC.moveToThirdPerson();
					}
				}
				else
				{
					cleanUpOnExit();
					tLC.moveToThirdPerson();
				}
				//TODO implement the rest of switching to the other camera
			}
		}
		else if(justActivated)
		{
			justActivated = false;
		}
	}
	
	public void cleanUpOnExit()
	{
		controlType = ControlType.None;
		camera.enabled = false;
		AudioListener aL = gameObject.GetComponent<AudioListener>();
		aL.enabled = false;
		Screen.lockCursor = false;
		if (lineRenderer!=null) lineRenderer.enabled = false;
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
	
	private bool ownedByCurrentPlayer ()
	{
		return transform.networkView.viewID.owner == Network.player;
	}
}
