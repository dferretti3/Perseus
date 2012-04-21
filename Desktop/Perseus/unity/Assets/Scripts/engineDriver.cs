using UnityEngine;
using System.Collections;

public class engineDriver : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(transform.parent == null && !particleSystem.IsAlive())
		{
			Destroy(gameObject);
		}
	}
}

