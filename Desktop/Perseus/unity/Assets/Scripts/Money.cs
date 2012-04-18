using UnityEngine;
using System.Collections;

public class Money : MonoBehaviour {
	float inc;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		inc = 6-transform.localScale.x;
	}
	
	void OnTriggerEnter(Collider col){
		if(col.gameObject.name != "Bullet(Clone)")
		{
			PlayerPrefs.SetFloat("money", PlayerPrefs.GetFloat("money")+inc);
			Network.Destroy(gameObject);
		}
	}

}
