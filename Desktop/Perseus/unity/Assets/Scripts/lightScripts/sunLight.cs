using UnityEngine;
using System.Collections;

public class sunLight : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		transform.forward = Vector3.zero - transform.position;
	}

	// Update is called once per frame
	void Update ()
	{
		
	}
}

