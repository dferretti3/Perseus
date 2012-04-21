using UnityEngine;
using System.Collections;

public class SaveTowerLocs : MonoBehaviour {
	public GameObject towerPrefab;
	public GameObject AIPrefab;
	public GameObject TurrettManagerPrefab;
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
	}
	
	public void saveComp(Vector3[] p, Vector3[] n)
	{
		if (!Network.isServer) return;
		float d = 4;
		for (int i = 0; i < p.Length; i++)
		{
			GameObject tower = (GameObject)Network.Instantiate(AIPrefab,p[i]+d*n[i],Quaternion.identity,0);	
		}
	}
	
//	public void saveLocs(Vector3 p1, Vector3 p2, Quaternion r1, Quaternion r2, Vector3 n1, Vector3 n2)
//	{
//		saveLocs(new Vector3[]{p1},new Vector3[]{p2},new Quaternion[]{r1},new Quaternion[]{r2},new Vector3[]{n1},new Vector3[]{n2});
//	}
	
	public void saveLocs(Vector3[] p1, Vector3[] p2, Quaternion[] r1, Quaternion[] r2, Vector3[] n1, Vector3[] n2)
	{
		income=true;
		GameObject ssS = GameObject.FindGameObjectWithTag("spaceSetup");
		if(ssS != null)
		{
			ssS.GetComponentInChildren<spaceSetupScript>().setupSpace();
		}
		float d = 4;
		bool isP1 = false;
		if(PlayerPrefs.GetInt("playerNum") == 0)
		{
			isP1 = true;
		}
		if(isP1)
		{
			/*GameObject tower1 = (GameObject)Network.Instantiate(towerPrefab,p1+d*n1,r1,0);
			topLevelController topCont1 = tower1.GetComponentInChildren<topLevelController>();
			topCont1.playerColor = Color.red;
			topCont1.nameTag = "PLR1";
			topCont1.isActive = true;*/
			TurrettManager manager = new TurrettManager(towerPrefab,p1,n1,r1,Color.red,"PLR1");
		}
		else
		{
			/*GameObject tower2 = (GameObject)Network.Instantiate(towerPrefab,p2+d*n2,r2,0);
			topLevelController topCont2 = tower2.GetComponentInChildren<topLevelController>();
			topCont2.playerColor = Color.blue;
			topCont2.nameTag = "PLR2";
			topCont2.isActive = true;*/
			TurrettManager manager = new TurrettManager(towerPrefab,p2,n2,r2,Color.blue,"PLR2");
		}
	}
}
