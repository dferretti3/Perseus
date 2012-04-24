using UnityEngine;
using System.Collections;

public class explodeRadius : MonoBehaviour {
	
	float startTime;
	
	// Use this for initialization
	void Start () {
		startTime = Time.fixedTime;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (networkView.viewID.owner==Network.player)
		{
			if (Time.fixedTime-startTime > 10.0f)
			{
					print("DESTROYING PARTICLE EFFECT");
				Network.Destroy(gameObject);
			}
		}
	}
}
