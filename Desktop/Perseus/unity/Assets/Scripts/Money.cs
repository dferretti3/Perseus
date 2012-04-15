using UnityEngine;
using System.Collections;

public class Money : MonoBehaviour {
	int scale;
	int inc;
		
	// Use this for initialization
	void Start () {
		scale = Random.Range(1,5);
		inc = 6-scale;
		transform.localScale = new Vector3(scale, scale, scale);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider col){
		PlayerPrefs.SetFloat("money", PlayerPrefs.GetFloat("money")+inc);
		Network.Destroy(gameObject);
	}

}
