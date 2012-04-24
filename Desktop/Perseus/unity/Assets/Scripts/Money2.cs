using UnityEngine;
using System.Collections;

public class Money2 : MonoBehaviour {
	float inc;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	void OnTriggerEnter(Collider col){
		print(col.gameObject.name);
		if(col.gameObject.name == "AIResourceGatherer(Clone)")
			GameObject.Destroy(gameObject);
	}

}
