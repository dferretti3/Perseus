using UnityEngine;
using System.Collections;

public class homingMissileCamera : MonoBehaviour
{
	private ControlType camType;
	private Rect viewRect = new Rect(.85f,.15f,0f,0f);
	private float rectWidth = 0f;
	public Texture2D videoBorder;
	public topLevelController tLC;
	private bool justActivated = false;
	public AudioClip thruster;
	private bool hasTarget = false;
	private GameObject target;
	private float dampener = 10f;
	private float homingDampener = 1f;
	GUIStyle myStyle;
	public Font warning;
	private homingMissileScript missile;


	// Use this for initialization
	void Start ()
	{
		myStyle = new GUIStyle();
			myStyle.font = warning;
			myStyle.alignment = TextAnchor.MiddleCenter;
			myStyle.fontSize = 50;
			myStyle.normal.textColor = Color.red;
			myStyle.normal.background = null;
		audio.clip = thruster;
		audio.loop = true;
		audio.Play();
		camType = ControlType.None;
		camera.enabled = false;
		Screen.lockCursor = true;
	}
	
	private bool ownedByCurrentPlayer ()
	{
		return transform.parent.networkView.viewID.owner == Network.player;
	}
	
	void OnGUI()
	{
		if(ownedByCurrentPlayer())
		{
			if(camType == ControlType.Inset)
			{
				Rect currentScreen = camera.rect;
				currentScreen.x = currentScreen.x*Screen.width;
				currentScreen.width = currentScreen.width*Screen.width + 2;
				currentScreen.height = currentScreen.height*Screen.height + 2;
				currentScreen.y = Screen.height - currentScreen.y*Screen.height - currentScreen.height;
				GUI.DrawTexture(currentScreen,videoBorder);
			}
			else if(camType == ControlType.Full)
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
		if(missile == null)
		{
			missile = transform.parent.gameObject.GetComponentInChildren<homingMissileScript>();
		}
		
		if(ownedByCurrentPlayer())
		{
		
		
		if(transform.position.y > 200 || transform.position.y < -200 || transform.position.x > 300 || transform.position.x
				<-300 || transform.position.z > 300 || transform.position.z < -300)
			{
			selfDestruct();
		}
		
		if(hasTarget)
		{
			//transform.parent.up = Vector3.Slerp(transform.forward,target.transform.position - transform.parent.position,Time.time/10000f);
			Vector3 direction = target.transform.position - transform.parent.position;
			Vector3 currDirection = transform.forward;
			if((direction - currDirection + transform.right).magnitude < (direction - currDirection).magnitude)
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
			}
			
		}
		
		
		if(camType == ControlType.Inset && rectWidth < .2)
		{
			rectWidth += .4f*Time.deltaTime;
			if(rectWidth >= .2f)
			{
				rectWidth = .2f;
			}
			viewRect = new Rect(.85f - rectWidth/2f,.15f - rectWidth/2f,rectWidth,rectWidth);
			camera.rect = viewRect;
		}
		else if(camType == ControlType.None && rectWidth > 0)
		{
			rectWidth -= .4f*Time.deltaTime;
			if(rectWidth <= 0f)
			{
				rectWidth = 0f;
				camera.enabled = false;
			}
			viewRect = new Rect(.85f - rectWidth/2f,.15f - rectWidth/2f,rectWidth,rectWidth);
			camera.rect = viewRect;
			
		}
		
		if(camType == ControlType.Full && !hasTarget)
		{
			float y = Input.GetAxis("Mouse Y");
			float x = Input.GetAxis("Mouse X");
			transform.Rotate(new Vector3(-y, 0, 0) * Time.deltaTime * 20);
			transform.Rotate(new Vector3(0, x, 0) * Time.deltaTime * 20);
			if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
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
			}
		}
		
		if(camType == ControlType.Full && !justActivated)
		{
			if(Input.GetMouseButtonDown(1))
			{
				if(hasTarget)
				{
					hasTarget = false;
					missile.toggleTarget();
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
			if(Input.GetMouseButtonDown(0))
			{
				homingMissileScript msS = transform.parent.gameObject.GetComponent<homingMissileScript>();
				msS.kill();
			}
		}
		
		if(camType != ControlType.None)
		{
			if(Input.GetKey(KeyCode.Space))
			{
				homingMissileScript msS = transform.parent.gameObject.GetComponent<homingMissileScript>();
				msS.kill();
			}
		}
		
		justActivated = false;
		}
	}
	
	void selfDestruct()
	{
		homingMissileScript msS = transform.parent.gameObject.GetComponent<homingMissileScript>();
		msS.kill();
	}
	
	void aquireTarget()
	{
		GameObject[] found = GameObject.FindGameObjectsWithTag("targetable");
		int foundAt = -1;
		float minMag = .2f;
		for(int x = 0; x < found.Length; x++)
		{
			Vector3 zCheck = camera.WorldToScreenPoint(found[x].transform.position);
			Vector3 tempCheck = camera.WorldToNormalizedViewportPoint(found[x].transform.position);
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
			missile.toggleTarget();
			hasTarget = true;
			target = found[foundAt];
		}
		
		
	}
	
	void Left()
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
	}
	
	public void openMiniScreen()
	{
		if(camType == ControlType.None)
		{
			camera.enabled = true;
			camType = ControlType.Inset;
		}
		else
		{
			camType = ControlType.None;
		}
	}
	
	public void makeActive()
	{
		if(ownedByCurrentPlayer())
		{
			camera.enabled = true;
			camType = ControlType.Full;
			justActivated = true;
			camera.rect = new Rect(0f,0f,1f,1f);
			AudioListener aL = gameObject.GetComponent<AudioListener>();
			aL.enabled = true;
		}
	}
	
	public void transferControl()
	{
		if(camType == ControlType.Full)
		{
			AudioListener aL = gameObject.GetComponent<AudioListener>();
			aL.enabled = false;
			tLC.moveToFirstPerson();
			camera.enabled = false;
			camType = ControlType.None;
			camera.rect = new Rect(.85f,.15f,0f,0f);
		}
	}
}

