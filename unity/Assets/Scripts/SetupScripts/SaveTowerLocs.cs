using UnityEngine;
using System.Collections;

public class SaveTowerLocs : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public static void saveLocs(Vector3 p1, Vector3 p2, Quaternion r1, Quaternion r2)
	{
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
		print("HERE");
	}
}
