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
		float turnspeed = 2.0f;

	
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
				}
				else if(tLC.currentMissileSelection == 1)
				{
					currentMissile = "CONTROLLED MISSILE";
				}
				
				GUI.TextArea(new Rect(0,0,200,50),"Current Missile:\n\n\t\t" + currentMissile);
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
			if(tLC.addNewMissile(tempMissile,mType))
			{
				tempMissile.transform.rotation = Quaternion.LookRotation(-transform.parent.up,transform.parent.forward);
				tempMissile.transform.position = transform.parent.position;
				homingMissileScript msS = tempMissile.GetComponent<homingMissileScript>();
				msS.init();
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
			if(tLC.addNewMissile(tempMissile,mType))
			{
				tempMissile.transform.rotation = Quaternion.LookRotation(transform.parent.forward,transform.parent.up);
				tempMissile.transform.position = transform.parent.position+transform.forward*20 ;
				ControlledMissile msS = tempMissile.GetComponent<ControlledMissile>();
				msS.init();
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
}
