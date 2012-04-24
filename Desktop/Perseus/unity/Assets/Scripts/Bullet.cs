using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	
	float projectilespeed = 2.0f;
	int count = 0;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		count++;
		if(ownedByCurrentPlayer())
		{
			
		transform.position += transform.forward * projectilespeed;
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
