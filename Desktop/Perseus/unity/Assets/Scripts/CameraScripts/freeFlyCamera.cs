using UnityEngine;
using System.Collections;

public class freeFlyCamera : MonoBehaviour {

	public float acceleration;
	public float max_velocity;
	public float friction;
	public float rotation_speed;
	public float zoom_speed;
	
	Vector3 velocity,ud_velocity,zoom_velocity;
	//mouse rotate
	public float mr_x,mr_y;
	
	// Use this for initialization
	void Start () {
		Application.runInBackground = true;
		velocity = Vector3.zero;
		ud_velocity = Vector3.zero;
		zoom_velocity = Vector3.zero;
		mr_x = transform.rotation.eulerAngles.y;
		mr_y = -transform.rotation.eulerAngles.x;
	}

	// Update is called once per frame
	void FixedUpdate () {
		float move_x = Input.GetAxis("Horizontal");
		float move_z = Input.GetAxis("Vertical");
		float move_y = Input.GetAxis("Height");
		
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
		
		//handle scrolling
		float scroll = Input.GetAxis("Mouse ScrollWheel");
		Vector3 Z = scroll*transform.forward;
		zoom_velocity += zoom_speed*Z;
		zoom_velocity *= (1.0f-friction);
		if (zoom_velocity.magnitude>max_velocity) {
			zoom_velocity *= max_velocity/zoom_velocity.magnitude;
		}
		transform.position += zoom_velocity*Time.fixedDeltaTime;
		
		//q and e to rotate 90
		float rotSpeed = 1.2f;
		float rot = Input.GetKey("q") ? rotSpeed : Input.GetKey("e") ? -rotSpeed : 0;
		if (rot!=0)
		{
			transform.RotateAround(Vector3.zero,Vector3.up,rot);
			mr_x += rot;
		}
	}
}
