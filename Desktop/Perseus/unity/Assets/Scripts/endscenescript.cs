using UnityEngine;
using System.Collections;

public class endscenescript : MonoBehaviour {
	public GameObject Money;
	// Use this for initialization
	void Start () {
		Network.Disconnect(100);
		createMoney();
	}
	
	public void createMoney(){		
		int RandomCount = Mathf.FloorToInt(Random.value*50 + 175);
		
		for(int i=1; i<RandomCount; i++)
		{
			int randx = Random.Range(-250, 250);
			int randz = Random.Range(-250, 250);
			int randy = Random.Range(10,100) + Mathf.FloorToInt(Terrain.activeTerrain.SampleHeight(new Vector3(randx,0,randz)));
			GameObject m = (GameObject)GameObject.Instantiate(Money, new Vector3(randx, randy, randz), Quaternion.identity);
			int scale = Random.Range(1,5);
			m.transform.localScale = new Vector3(scale, scale, scale);
		}
	}
	// Update is called once per frame
	void Update () {
	
	}
}
