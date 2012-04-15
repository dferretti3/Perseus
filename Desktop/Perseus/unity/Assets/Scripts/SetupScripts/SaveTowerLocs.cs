using UnityEngine;
using System.Collections;

public class SaveTowerLocs : MonoBehaviour {
	public GameObject towerPrefab;
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
	
	public void saveLocs(Vector3 p1, Vector3 p2, Quaternion r1, Quaternion r2, Vector3 n1, Vector3 n2)
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
			GameObject tower1 = (GameObject)Network.Instantiate(towerPrefab,p1+d*n1,r1,0);
			topLevelController topCont1 = tower1.GetComponentInChildren<topLevelController>();
			topCont1.playerColor = Color.red;
			topCont1.nameTag = "PLR1";
			topCont1.isActive = true;
		}
		else
		{
			GameObject tower2 = (GameObject)Network.Instantiate(towerPrefab,p2+d*n2,r2,0);
			topLevelController topCont2 = tower2.GetComponentInChildren<topLevelController>();
			topCont2.playerColor = Color.blue;
			topCont2.nameTag = "PLR2";
			topCont2.isActive = true;
		}
		PlayerPrefs.SetFloat("p1.x",p1.x);
		PlayerPrefs.SetFloat("p1.y",p1.y);
		PlayerPrefs.SetFloat("p1.z",p1.z);
		PlayerPrefs.SetFloat("p2.x",p2.x);
		PlayerPrefs.SetFloat("p2.y",p2.y);
		PlayerPrefs.SetFloat("p2.z",p2.z);
		PlayerPrefs.SetFloat("r1.x",r1.x);
		PlayerPrefs.SetFloat("r1.y",r1.y);
		PlayerPrefs.SetFloat("r1.z",r1.z);
		PlayerPrefs.SetFloat("r1.w",r1.w);
		PlayerPrefs.SetFloat("r2.x",r2.x);
		PlayerPrefs.SetFloat("r2.y",r2.y);
		PlayerPrefs.SetFloat("r2.z",r2.z);
		PlayerPrefs.SetFloat("r2.w",r2.w);
	}
}
