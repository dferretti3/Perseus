using UnityEngine;
using System.Collections;

public class BGM : MonoBehaviour {
	public AudioClip[] music;
	int index;
	// Use this for initialization
	void Start () {
		index = Random.Range(0,music.Length-1);
		audio.clip = music[index];
		audio.volume = 0.3f;
		audio.Play();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.M))
		{
			index++;
			if(index > music.Length-1)
				index=0;
			audio.Stop();
			audio.clip = music[index];
			audio.volume = 0.3f;
			audio.Play();
		}
		
	}
}
