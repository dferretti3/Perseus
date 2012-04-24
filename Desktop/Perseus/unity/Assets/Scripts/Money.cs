using UnityEngine;
using System.Collections;

public class Money : MonoBehaviour {
	float inc;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	void OnTriggerEnter(Collider col){
		if(col.gameObject.name != "Bullet(Clone)")
		{
			if(col.gameObject.networkView.owner == Network.player)
			{
				if(col.gameObject.name == "AIResourceGatherer(Clone)" || col.gameObject.name == "AIContMissile(Clone)")
					Network.Destroy(gameObject);
				else
				{
					PlayerPrefs.SetFloat("money", PlayerPrefs.GetFloat("money")+ 6 - transform.localScale.x);
					Network.Destroy(gameObject);
				}
			}
		}
	}

}
