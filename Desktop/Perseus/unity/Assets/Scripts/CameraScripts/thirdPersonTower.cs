using UnityEngine;
using System.Collections;

public enum ControlType {Full, Inset, None};

public class thirdPersonTower : MonoBehaviour {
	
	private float angle = Mathf.PI*3/2;
	private float verticleAngle = Mathf.PI/8;
	public ControlType controlType = ControlType.None;
	private topLevelController tLC;
	private bool justActivated = false;
	private string cost;
	// Use this for initialization
	void Start () {
		Screen.lockCursor = true;
		float horRad = 18*Mathf.Cos(verticleAngle);
		transform.localPosition = new Vector3(Mathf.Cos(angle)*horRad,15*Mathf.Sin(verticleAngle),Mathf.Sin(angle)*horRad);
	}
	
	void OnGUI()
	{
		if(controlType == ControlType.Full)
		{
			if(tLC != null)
			{
				string currentMissile = "";
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
					currentMissile = "AI MISSILE";
					cost = "Cost: 0";
				}
				
				GUI.TextArea(new Rect(0,0,200,50),"\n\t\t" + currentMissile + "\n\t\t\t\t\t\t\t\t\t"+cost);
			}
		}
	}
	
	/*void OnPreRender()
	{
		Debug.Log("Camera: " + camera.name + " OnPreRender()");
	}
	
	void OnPostRender()
	{
		Debug.Log("Camera: " + camera.name + " OnPostRender()");
	}*/
	
	
	public void makeActive()
	{
		Screen.lockCursor = true;
		controlType = ControlType.Full;
		camera.enabled = true;
		AudioListener aL = gameObject.GetComponent<AudioListener>();
		aL.enabled = true;
		justActivated = true;
	}
	
	public void shutDownControl()
	{
		cleanUpOnExit();
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
			
			if(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
			{
				transform.parent.RotateAroundLocal(Vector3.up,-Mathf.PI*Time.deltaTime/5);
			}
			else if(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
			{
				transform.parent.RotateAroundLocal(Vector3.up,Mathf.PI*Time.deltaTime/5);
			}
			
			
			if((Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) && yAngle < Mathf.PI/8)
			{
				transform.parent.RotateAroundLocal(transform.right,-Mathf.PI*Time.deltaTime/5);
			}
			else if((Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) && yAngle > -Mathf.PI/8)
			{
				transform.parent.RotateAround(transform.right,Mathf.PI*Time.deltaTime/5);
			}
			float y = Input.GetAxis("Mouse Y");
			float x = Input.GetAxis("Mouse X");
			transform.parent.Rotate(new Vector3(-y, 0, 0) * Time.deltaTime * 50);
			transform.parent.Rotate(new Vector3(0, x, 0) * Time.deltaTime * 50);
    		transform.parent.eulerAngles = new Vector3(transform.parent.eulerAngles.x,transform.parent.eulerAngles.y, 0);
			if(Input.GetMouseButtonDown(0))
			{
				if(tLC.moveToMissile(tLC.currentMissileSelection))
				{
					cleanUpOnExit();
				}
			}
			if(Input.GetMouseButtonDown(1))
			{
				if(tLC == null)
				{
					tLC = transform.parent.GetComponentInChildren<topLevelController>();
				}
				tLC.openMiniScreen(tLC.currentMissileSelection);
				
			}
			if(Input.GetKeyDown(KeyCode.Q))
			{
				if(tLC == null)
				{
					tLC = transform.parent.GetComponentInChildren<topLevelController>();
					if(tLC != null)
					{
						cleanUpOnExit();
						tLC.moveToFirstPerson();
					}
				}
				else
				{
					cleanUpOnExit();
					tLC.moveToFirstPerson();
				}
				//TODO implement the rest of switching to the other camera
			}
			int scroll = Mathf.RoundToInt(Input.GetAxis("Mouse ScrollWheel") * 10.0f);
			if (scroll!=0)
			{
				print("scrolling "+scroll);
				tLC.manager.scroll(scroll);
			}
		}
		else if(justActivated)
		{
			justActivated = false;
		}
		//transform.forward = targetPosition - transform.position;
		if(angle >= Mathf.PI*2)
		{
			angle -= Mathf.PI*2;
		}
		else if(angle <= Mathf.PI*2)
		{
			angle += Mathf.PI*2;
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
