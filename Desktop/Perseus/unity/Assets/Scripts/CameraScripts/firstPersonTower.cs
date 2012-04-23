using UnityEngine;
using System.Collections;

public class firstPersonTower : MonoBehaviour {
	
	public ControlType controlType = ControlType.None;
	//public Font warning;	
	private topLevelController tLC;
	private bool justActivated = false;
	private float targetSize;
	public Texture2D target;
	private float missileButtonWidth;
	private float missileButtonHeight;
	public GameObject[] missilePrefab;
	public AudioClip commandcodes;
	public GameObject expSource;
		float turnspeed = 2.0f;
	string cost;
	
	// Use this for initialization
	void Start () {
		targetSize = Screen.width/10;
		missileButtonWidth = Screen.width/10;
		missileButtonHeight = Screen.height/10;
		Screen.lockCursor = true;
	}
	
	
	
	void OnGUI()
	{
		if(controlType == ControlType.Full)
		{
			GUI.DrawTexture(new Rect(Screen.width/2 - targetSize/2,Screen.height/2 - targetSize/2,targetSize,targetSize),target);
			string currentMissile = "";
			if(tLC != null)
			{
				if(tLC.currentMissileSelection == 0)
				{
					currentMissile = "HOMING MISSILE";
					cost = "Cost: 10";
				}
				else if(tLC.currentMissileSelection == 1)
				{
					currentMissile = "CONTROLLED MISSILE";
					cost = "Cost: 10";
				}
				else if(tLC.currentMissileSelection == 2)
				{
					currentMissile = "MACHINE GUN";
					cost = "Cost: 0";
				}
				else if(tLC.currentMissileSelection == 3)
				{
					currentMissile = "COLLECTOR";
					cost = "Cost: 30";
				}
				
				GUI.TextArea(new Rect(0,0,200,50),"\n\t\t" + currentMissile + "\n\t\t\t\t\t\t\t\t\t"+cost);
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
			if(Network.isServer)
				tempMissile.tag = "P1";
			if(Network.isClient)
				tempMissile.tag = "P2";
			
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
			if(Network.isServer)
				tempMissile.tag = "P1";
			if(Network.isClient)
				tempMissile.tag = "P2";
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
			prefabNum = 2;
			tempMissile = (GameObject)Network.Instantiate(missilePrefab[prefabNum],transform.parent.position+transform.forward*20,Quaternion.LookRotation(transform.parent.forward,transform.parent.up),0);
		}
		else if(mType == missileType.Collector)
		{
			prefabNum = 3;
			tempMissile = (GameObject)Network.Instantiate(missilePrefab[prefabNum],new Vector3(-1000f,-1000f,-1000f),Quaternion.identity,0);
			if(Network.isServer)
				tempMissile.tag = "P1";
			if(Network.isClient)
				tempMissile.tag = "P2";
			if(!tLC.moveToMissile(3)&&PlayerPrefs.GetFloat("money")>30)
			{
				Debug.Log("Sending message to controller");
				if(tLC.addNewMissile(tempMissile,mType))
				{
					PlayerPrefs.SetFloat("money", PlayerPrefs.GetFloat("money")-30);
					tempMissile.transform.rotation = Quaternion.LookRotation(transform.parent.forward,transform.parent.up);
					tempMissile.transform.position = transform.position+Vector3.up*8 ;
					PlayAudioClip(commandcodes,transform.position,4f);
				}
			}
			else
			{
				Destroy(tempMissile);
				if(tLC.moveToMissile(4))
				{
					cleanUpOnExit();
				}
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
		if(tLC == null)
		{
			tLC = transform.parent.GetComponentInChildren<topLevelController>();
		}
		
		if(controlType == ControlType.Full && !justActivated)
		{
			float yAngle = Mathf.Asin(transform.parent.forward.y/Mathf.Abs(transform.parent.forward.magnitude));
			
			if(Input.GetMouseButtonDown(0))
			{
				fireMissile(tLC.currentMissileSelection);
			}
			else if(Input.GetMouseButtonDown(1))
			{
				tLC.openMiniScreen(tLC.currentMissileSelection);
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
}
