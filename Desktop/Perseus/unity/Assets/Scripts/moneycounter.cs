using UnityEngine;
using System.Collections;

public class moneycounter : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		guiText.text = "money: " + Mathf.FloorToInt(PlayerPrefs.GetFloat("money"));
	}
}
