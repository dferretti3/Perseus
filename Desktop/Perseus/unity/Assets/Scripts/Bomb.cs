using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour {
	
	Vector3 velocity;
	float projectilespeed = 80.5f;
	float gravity = 50.8f;
	
	// Use this for initialization
	void Start () {
		velocity = Vector3.zero;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		if (ownedByCurrentPlayer()) {
			if (velocity.magnitude==0) velocity = transform.forward*projectilespeed;
			velocity += gravity*Vector3.down*Time.fixedDeltaTime;
//			transform.position += transform.forward * projectilespeed + gravity*Vector3.down;;
			transform.position += velocity*Time.fixedDeltaTime;
			if(transform.position.y > 200 || transform.position.y < -200 || transform.position.x > 300 || transform.position.x
				<-300 || transform.position.z > 300 || transform.position.z < -300)
			{
				Network.Destroy(this.gameObject);
			}
		}
	}

	void OnTriggerEnter(Collider col)
	{
		if(col.gameObject.name==("Terrain") || col.gameObject.name==("ControlledMissile(Clone)") ||
			col.gameObject.name==("homingMissile(Clone)") || col.gameObject.name == ("AIContMissile(Clone)"))
			Network.Destroy(this.gameObject);
	}
	
	private bool ownedByCurrentPlayer ()
	{
		return transform.networkView.viewID.owner == Network.player;
	}
}
