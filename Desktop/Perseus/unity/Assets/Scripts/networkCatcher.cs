using UnityEngine;
using System.Collections;

public class networkCatcher : MonoBehaviour
{
	public static Color[] teams = new Color[] {Color.red,Color.blue,Color.black,Color.magenta,new Color(1, 0.5f,0,1)};
	int c;
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
		if(gameObject.networkView.owner == Network.player)
			GetComponentInChildren<topLevelController>().hitPlayer(damage);
	}
	
	[RPC]
	void updateHealth(int currentHealth)
	{
		GetComponentInChildren<topLevelController>().setHealth(currentHealth);
	}
}

