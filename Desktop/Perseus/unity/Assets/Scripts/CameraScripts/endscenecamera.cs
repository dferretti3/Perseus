using UnityEngine;
using System.Collections;

public class endscenecamera : MonoBehaviour {
	public GameObject aigatherer;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3(aigatherer.transform.position.x+15f, aigatherer.transform.position.y-5.3f, aigatherer.transform.position.z);
	}
}
