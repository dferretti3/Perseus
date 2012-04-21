using UnityEngine;
using System.Collections;

public class MoneyManager : MonoBehaviour {
	public GameObject Money;
	bool setup=false;
	private float nextDrop = 120;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(!setup && Network.isServer)
			createMoney();
		
		nextDrop -= Time.deltaTime;
		if(nextDrop <= 0)
		{
			newDrop();
		}
	}
	
	void createMoney(){
		setup=true;
		
		int RandomCount = Mathf.FloorToInt(Random.value*50 + 175);
		
		for(int i=1; i<RandomCount; i++)
		{
			generateNewCoin();
		}
	}
	
	void newDrop()
	{
		int RandomCount = Mathf.FloorToInt(Random.value*15);
		
		for(int i=1; i<RandomCount; i++)
		{
			generateNewCoin();
		}
		nextDrop = Random.value*30;
	}
	
	void generateNewCoin()
	{
		int randx = Random.Range(-250, 250);
		int randz = Random.Range(-250, 250);
		int randy = Random.Range(10,100) + Mathf.FloorToInt(Terrain.activeTerrain.SampleHeight(new Vector3(randx,0,randz)));
		GameObject m = (GameObject)Network.Instantiate(Money, new Vector3(randx, randy, randz), Quaternion.identity, 0);
		int scale = Random.Range(1,5);
		m.transform.localScale = new Vector3(scale, scale, scale);
	}
}
