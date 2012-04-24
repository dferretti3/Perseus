using UnityEngine;
using System.Collections;

public class winlosetext : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if(PlayerPrefs.GetInt("win") == 1)
			guiText.text = "you won";
		else
			guiText.text = "you lost";
		
		
		
		
		if(Input.GetMouseButtonDown(0))
		{
			Application.LoadLevel(0);
		}
	}
}
