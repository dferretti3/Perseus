using UnityEngine;
using System.Collections;

public class freeFlyCamera : MonoBehaviour {

	public float acceleration;
	public float max_velocity;
	public float friction;
	public float rotation_speed;
	
	Vector3 velocity,ud_velocity;
	//mouse rotate
	public float mr_x,mr_y;
	
	public bool mode;
	
	// Use this for initialization
	void Start () {
		mode = true;
		velocity = Vector3.zero;
		ud_velocity = Vector3.zero;
		mr_x = transform.rotation.eulerAngles.y;
		mr_y = -transform.rotation.eulerAngles.x;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		float move_x = Input.GetAxis("Horizontal");
		float move_z = Input.GetAxis("Vertical");
		float move_y = Input.GetAxis("Height");
		
		
		//if we're placing towers
		if (mode) {
			//keep x/z velocity separate from y velocity
			Vector3 A1 = move_x*Vector3.right + move_z*Vector3.forward;
			Vector3 A2 = move_y*Vector3.up;
			velocity += acceleration*A1;
			ud_velocity += acceleration*A2;
			velocity *= (1.0f-friction);
			ud_velocity *= (1.0f-friction);
			if (velocity.magnitude>max_velocity) {
				velocity *= max_velocity/velocity.magnitude;
			}
			if (ud_velocity.magnitude>max_velocity) {
				ud_velocity *= max_velocity/ud_velocity.magnitude;
			}
			//multiply the x/z velocity by rotation, then add y velocity
			transform.Translate( (ud_velocity + Quaternion.Euler(0,mr_x,0)*velocity) *Time.fixedDeltaTime,Space.World);
			
			//handle rotation on mouse movement
			if (Input.GetMouseButton(2) || Input.GetKey("r")) {
				mr_x += rotation_speed*Input.GetAxis("Mouse X")*Time.fixedDeltaTime;
				mr_y += rotation_speed*Input.GetAxis("Mouse Y")*Time.fixedDeltaTime;
				transform.rotation = Quaternion.Euler(-mr_y,mr_x,0);
			}
		} else {
			//else we're inside the towers
			mr_x += rotation_speed*Input.GetAxis("Mouse X")*Time.fixedDeltaTime;
			mr_y += rotation_speed*Input.GetAxis("Mouse Y")*Time.fixedDeltaTime;
			transform.rotation = Quaternion.Euler(-mr_y,mr_x,0);
		}
	}
	
	public void changeMode(bool over) {
		if (!mode && !over) return;
		mode = false;
		mr_x = transform.rotation.eulerAngles.y;
		mr_y = -transform.rotation.eulerAngles.x;
	}
}
