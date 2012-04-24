using UnityEngine;
using System.Collections;

public class SaveTowerLocsTestExpansion : MonoBehaviour {
	public GameObject towerPrefab;
	public GameObject AIPrefab;
	public GameObject TurrettManagerPrefab;
	
	private scoreManager scoreMan;
	// Use this for initialization
	float money = 20;
	bool income = false;
	void Start () {
		PlayerPrefs.SetFloat("money", money);
	}
	
	// Update is called once per frame
	void Update () {
		if(income){
			PlayerPrefs.SetFloat("money", PlayerPrefs.GetFloat("money")+Time.deltaTime*0.33f);
		}
		
		if(scoreMan == null)
		{
			scoreMan = (GameObject.FindGameObjectWithTag("scoreManagerTag")).GetComponent<scoreManager>();
		}
	}
	
	public void saveComp(Vector3[] p, Vector3[] n, int[] t,string[] names)
	{
		if (!Network.isServer) return;
		float d = 4;
		for (int i = 0; i < p.Length; i++)
		{
			GameObject tower = (GameObject)Network.Instantiate(AIPrefab,p[i]+d*n[i],Quaternion.identity,0);	
			scoreMan.addAITower(tower,t[i],names[i]);
		}
	}
	
//	public void saveLocs(Vector3 p1, Vector3 p2, Quaternion r1, Quaternion r2, Vector3 n1, Vector3 n2)
//	{
//		saveLocs(new Vector3[]{p1},new Vector3[]{p2},new Quaternion[]{r1},new Quaternion[]{r2},new Vector3[]{n1},new Vector3[]{n2});
//	}
	
	public void saveLocs(Vector3[] p1, Quaternion[] r1,  Vector3[] n1,Color team, string callSign, int teamNum)
	{
		income=true;
		GameObject ssS = GameObject.FindGameObjectWithTag("spaceSetup");
		if(ssS != null)
		{
			ssS.GetComponentInChildren<spaceSetupScript>().setupSpace();
		}
		TurrettManager manager = new TurrettManager(towerPrefab,p1,n1,r1,team,callSign);
	}
}
