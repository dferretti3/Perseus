using UnityEngine;
using System.Collections;

public class ControlledMissile : MonoBehaviour {
	
	public topLevelController tLC;
	private ControlType controlType = ControlType.None;
	private Rect viewRect = new Rect(0f,0f,0f,0f);
	private float rectWidth = 0f;
	private bool justActivated = false;
	float turnspeed = 2.0f;
	float flyspeed = 1.0f;
	public Camera cameraView;
	int view = -1;
	int invert = 1;
	private bool begin = false;
	private bool colliding = false;
	// Use this for initialization
	void Start () {
		
	}
	
	public void init()
	{
		begin = true;
	}
	
	void Update()
	{
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
		
		if(begin)
		{
			transform.position += transform.forward*flyspeed;
		}
		
		if(controlType == ControlType.Full && !justActivated)
		{
			if(Input.GetKeyDown(KeyCode.E))
			{
				transferControl();
			}
		}
		
		if(controlType != ControlType.None)
		{
			if(Input.GetKey(KeyCode.Space))
			{
				kill();
			}
		}
		
		justActivated = false;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		if(controlType == ControlType.Full)
		{
			float x = Input.GetAxis("Mouse X");
			float y = Input.GetAxis("Mouse Y");
			if(Input.GetKeyDown(KeyCode.I))
			   invert = -1;
			
			rigidbody.AddRelativeTorque(y*invert*-turnspeed,x*turnspeed,0);
			
			float rotateAmount = 0.6f;
      		if (Input.GetAxis("Horizontal") < 0)
				transform.Rotate( 0, 0,rotateAmount);
       		else if (Input.GetAxis("Horizontal") > 0)
				transform.Rotate(0,0,-rotateAmount);
		}
	}
	
	void OnTriggerEnter(Collider col){
		if(colliding)
			kill();
	}
	
	void OnTriggerExit(Collider col){
		colliding=true;
	}
	public void makeActive(){
		Screen.lockCursor = true;
		cameraView.enabled = true;
		controlType = ControlType.Full;
		justActivated = true;
		cameraView.rect = new Rect(0f,0f,1f,1f);
		AudioListener aL = cameraView.gameObject.GetComponent<AudioListener>();
		aL.enabled = true;
		
	}
	
	public void transferControl(){ 
		if(controlType == ControlType.Full)
		{
			Screen.lockCursor = false;
			AudioListener aL = cameraView.gameObject.GetComponent<AudioListener>();
			aL.enabled = false;
			tLC.moveToFirstPerson();
			cameraView.enabled = false;
			controlType = ControlType.None;
			cameraView.rect = new Rect(.85f,.15f,0f,0f);
		}
	}
	
	public void openMiniScreen(){
		if(controlType == ControlType.None)
		{
			cameraView.enabled = true;
			controlType = ControlType.Inset;
		}
		else
		{
			controlType = ControlType.None;
		}
	}
	
	private void kill()
	{
		transferControl();
		Destroy(gameObject);
	}
}
