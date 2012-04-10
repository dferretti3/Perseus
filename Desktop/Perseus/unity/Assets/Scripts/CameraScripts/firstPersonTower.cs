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
			if(GUI.Button(new Rect(Screen.width/25,5,missileButtonWidth - 10,missileButtonHeight),"M1"))
			{
				if(tLC == null)
				{
					tLC = transform.parent.GetComponentInChildren<topLevelController>();
					if(tLC != null)
					{
						fireMissile(missileType.Homing);
					}
				}
				else
				{
					fireMissile(missileType.Homing);
				}
			}
			if(GUI.Button(new Rect(Screen.width/25 + missileButtonWidth,5,missileButtonWidth - 10,missileButtonHeight),"M2"))
			{
				if(tLC == null)
				{
					tLC = transform.parent.GetComponentInChildren<topLevelController>();
					if(tLC != null)
					{
						fireMissile(missileType.Controlled);
					}
				}
				else
				{
					fireMissile(missileType.Controlled);
				}
			}
		}
	}
	
	void fireMissile(missileType mType)
	{
		int prefabNum = -1;
		GameObject tempMissile;
		if(mType == missileType.Homing)
		{
			prefabNum = 0;
			tempMissile = (GameObject)Instantiate(missilePrefab[prefabNum]);
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
			tempMissile = (GameObject)Instantiate(missilePrefab[prefabNum]);
			if(tLC.addNewMissile(tempMissile,mType))
			{
				tempMissile.transform.rotation = Quaternion.LookRotation(transform.parent.forward,transform.parent.up);
				tempMissile.transform.position = transform.parent.position;
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
		controlType = ControlType.Full;
		camera.enabled = true;
		AudioListener aL = gameObject.GetComponent<AudioListener>();
		aL.enabled = true;
		justActivated = true;
	}
	
	// Update is called once per frame
	void Update () {
			
		
		if(controlType == ControlType.Full && !justActivated)
		{
			float yAngle = Mathf.Asin(transform.parent.forward.y/Mathf.Abs(transform.parent.forward.magnitude));
			
			if(Input.GetKeyDown(KeyCode.Alpha1))
			{
				if(tLC == null)
				{
					tLC = transform.parent.GetComponentInChildren<topLevelController>();
					if(tLC != null)
					{
						if(Input.GetKey(KeyCode.LeftShift))
						{
							tLC.openMiniScreen(0);
						}
						else
						{
							fireMissile(missileType.Homing);
						}
					}
				}
				else
				{
					if(Input.GetKey(KeyCode.LeftShift))
					{
						tLC.openMiniScreen(0);
					}
					else
					{
						fireMissile(missileType.Homing);
					}
				}
			}
			if(Input.GetKeyDown(KeyCode.Alpha2))
			{
				if(tLC == null)
				{
					tLC = transform.parent.GetComponentInChildren<topLevelController>();
					if(tLC != null)
					{
						if(Input.GetKey(KeyCode.LeftShift))
						{
							tLC.openMiniScreen(1);
						}
						else
						{
							fireMissile(missileType.Controlled);
						}
					}
				}
				else
				{
					if(Input.GetKey(KeyCode.LeftShift))
					{
						tLC.openMiniScreen(1);
					}
					else
					{
						fireMissile(missileType.Controlled);
					}
				}
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
	}
}
