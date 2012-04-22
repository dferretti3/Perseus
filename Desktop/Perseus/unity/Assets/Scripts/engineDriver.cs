using UnityEngine;
using System.Collections;

public class engineDriver : MonoBehaviour
{
	private bool freedChildren = false;
	// Use this for initialization
	void Start ()
	{
		freedChildren = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(transform.parent == null)
		{
			if(!freedChildren)
			{
				ParticleSystem[] engines = GetComponentsInChildren<ParticleSystem>();
				for(int x = 0; x < engines.Length; x++)
				{
					engines[x].transform.parent = null;
					engines[x].enableEmission = false;
				}
				freedChildren = true;
			}
			if(!particleSystem.IsAlive())
			{
				Destroy(gameObject);
			}
		}
	}
}

