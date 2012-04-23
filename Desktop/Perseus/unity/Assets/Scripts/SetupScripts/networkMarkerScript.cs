using UnityEngine;
using System.Collections;

public class networkMarkerScript : MonoBehaviour
{
	public Color c;
	// Use this for initialization
	void Start ()
	{
	
	}
	
	public void pushColor()
	{
		networkView.RPC("setColor",RPCMode.All,new Vector3(c.r,c.g,c.b));
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	[RPC]
	void setColor(Vector3 rgb)
	{
		Color color = new Color(rgb.x,rgb.y,rgb.z,.5f);
		renderer.material.color = color;
	}
}

