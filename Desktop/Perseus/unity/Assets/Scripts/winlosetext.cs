using UnityEngine;
using System.Collections;

public class winlosetext : MonoBehaviour {
	float delay = 3;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		delay -= Time.deltaTime;
		
		if(PlayerPrefs.GetInt("win") == 1)
			guiText.text = "you won";
		else
			guiText.text = "you lost";
		
		
		
		
		if(Input.GetMouseButtonDown(0) && delay <= 0)
		{
			Application.LoadLevel(0);
		}
	}
}
