using UnityEngine;
using System.Collections;

public class MoneyManager : MonoBehaviour {
	public GameObject Money;
	bool setup=false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(!setup && Network.isServer)
			createMoney();
	}
	
	void createMoney(){
		setup=true;
		for(int i=1; i<200; i++)
		{
			int randx = Random.Range(-250, 250);
			int randz = Random.Range(-250, 250);
			int randy = Random.Range(10,100);
			Network.Instantiate(Money, new Vector3(randx, randy, randz), Quaternion.identity, 0);
		}
	}
}
