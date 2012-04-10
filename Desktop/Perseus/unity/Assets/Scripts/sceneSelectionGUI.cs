using UnityEngine;
using System.Collections;

public class sceneSelectionGUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	void OnGUI()
	{
		if(GUI.Button(new Rect(Screen.width/6,Screen.height/3,Screen.width/3,Screen.height/3),"Land"))
		{
			Application.LoadLevel("setupScene");
		}
		
		if(GUI.Button(new Rect(Screen.width*2/3,Screen.height/3,Screen.width/3,Screen.height/3),"Space"))
		{
			Application.LoadLevel("setupScene1");
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
