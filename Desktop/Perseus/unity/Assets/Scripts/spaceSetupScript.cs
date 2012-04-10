using UnityEngine;
using System.Collections;

public class spaceSetupScript : MonoBehaviour {
	
	public GameObject asteroidField;
	public GameObject spaceSounds;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void setupSpace()
	{
		GameObject toDel = GameObject.FindGameObjectWithTag("Terrain");
		Destroy(toDel);
		GameObject newField = (GameObject)Instantiate(asteroidField);
		newField.transform.position = new Vector3(0,0,0);
		GameObject sounds = (GameObject)Instantiate(spaceSounds);
		sounds.transform.position = new Vector3(0,0,0);
	}
}
