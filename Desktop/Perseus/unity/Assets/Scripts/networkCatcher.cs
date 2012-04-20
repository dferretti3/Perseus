using UnityEngine;
using System.Collections;

public class networkCatcher : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	
	[RPC]
	void setNavColor(Vector3 pColor, string nTag)
	{
		GetComponentInChildren<navPoint>().subRefresh(new Color(pColor.x,pColor.y,pColor.z,1f),nTag);
	}
	
	[RPC]
	void hitTower(int damage)
	{
		GetComponentInChildren<topLevelController>().hitPlayer(damage);
	}
	
	[RPC]
	void updateHealth(int currentHealth)
	{
		GetComponentInChildren<topLevelController>().setHealth(currentHealth);
	}
}

