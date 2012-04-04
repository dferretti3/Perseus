using UnityEngine;
using System.Collections;

public class ControlledMissile : MonoBehaviour {
	
	public topLevelController tLC;

	float turnspeed = 2.0f;
	float flyspeed = 6.0f;
	public Camera camera;
	int view = -1;
	int invert = 1;
	// Use this for initialization
	void Start () {
		Screen.lockCursor = true;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		float x = Input.GetAxis("Mouse X");
		float y = Input.GetAxis("Mouse Y");
		if(Input.GetKeyDown(KeyCode.I))
		   invert = -1;
		
		rigidbody.AddRelativeTorque(y*invert*-turnspeed,x*turnspeed,0);
		transform.position += transform.forward*flyspeed;
		
		float rotateAmount = 1.0f;
      	if (Input.GetAxis("Horizontal") < 0)
			transform.Rotate( 0, 0,rotateAmount);
       	else if (Input.GetAxis("Horizontal") > 0)
			transform.Rotate(0,0,-rotateAmount);
	}
	
	void OnTriggerEnter(Collider col){
		print(col);
		if(col.gameObject.name=="Terrain")
			Destroy(this.gameObject);
		if(col.gameObject.name=="Bullet(Clone)")
			Destroy(this.gameObject);
		if(col.gameObject.name=="EnemyTower" || col.gameObject.name == "SpherePivot")
			Destroy(this.gameObject);
	}
	
	public void makeActive(){
	}
	
	public void transferControl(){ 
		tLC.moveToFirstPerson();
	}
	
	public void openMiniScreen(){
	}
}
