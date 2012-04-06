using UnityEngine;
using System.Collections;

public enum ControlType {Full, Inset, None};

public class thirdPersonTower : MonoBehaviour {
	
	private float angle = Mathf.PI*3/2;
	private float verticleAngle = Mathf.PI/8;
	public ControlType controlType = ControlType.None;
	private topLevelController tLC;
	private bool justActivated = false;
	// Use this for initialization
	void Start () {
		Screen.lockCursor = true;
		float horRad = 18*Mathf.Cos(verticleAngle);
		transform.localPosition = new Vector3(Mathf.Cos(angle)*horRad,15*Mathf.Sin(verticleAngle),Mathf.Sin(angle)*horRad);
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
		
		
		if(controlType == ControlType.Full && !justActivated)
		{
			
			float yAngle = Mathf.Asin(transform.parent.forward.y/Mathf.Abs(transform.parent.forward.magnitude));
			
			if(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
			{
				transform.parent.RotateAroundLocal(Vector3.up,-Mathf.PI*Time.deltaTime/10);
			}
			else if(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
			{
				transform.parent.RotateAroundLocal(Vector3.up,Mathf.PI*Time.deltaTime/10);
			}
			
			
			if((Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) && yAngle < Mathf.PI/8)
			{
				transform.parent.RotateAroundLocal(transform.right,-Mathf.PI*Time.deltaTime/10);
			}
			else if((Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) && yAngle > -Mathf.PI/8)
			{
				transform.parent.RotateAround(transform.right,Mathf.PI*Time.deltaTime/10);
			}
			float y = Input.GetAxis("Mouse Y");
			float x = Input.GetAxis("Mouse X");
			transform.parent.Rotate(new Vector3(-y, 0, 0) * Time.deltaTime * 50);
			transform.parent.Rotate(new Vector3(0, x, 0) * Time.deltaTime * 50);
    		transform.parent.eulerAngles = new Vector3(transform.parent.eulerAngles.x,transform.parent.eulerAngles.y, 0);
			if(Input.GetKey(KeyCode.LeftShift))
			{
				if(tLC == null)
				{
					tLC = transform.parent.GetComponentInChildren<topLevelController>();
				}
				int miniScreen = -1;
				if(Input.GetKeyDown(KeyCode.Alpha1))
				{
					tLC.openMiniScreen(0);
				}
				else if(Input.GetKeyDown(KeyCode.Alpha2))
				{
					tLC.openMiniScreen(1);
				}
				
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
			
			
			if(tLC == null)
			{
					tLC = transform.parent.GetComponentInChildren<topLevelController>();
			}
			
			if(Input.GetKeyDown(KeyCode.Alpha1))
			{
				if(Input.GetKey(KeyCode.LeftShift))
				{
					tLC.openMiniScreen(0);
				}
				else
				{
					if(tLC.moveToMissile(0))
					{
						cleanUpOnExit();
					}
				}
			}
			else if(Input.GetKeyDown(KeyCode.Alpha2))
			{
				if(Input.GetKey(KeyCode.LeftShift))
				{
					tLC.openMiniScreen(1);
				}
				else
				{
					if(tLC.moveToMissile(1))
					{
						cleanUpOnExit();
					}
				}
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
	}
}
