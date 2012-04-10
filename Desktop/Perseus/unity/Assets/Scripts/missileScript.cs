using UnityEngine;
using System.Collections;

public class missileScript : MonoBehaviour
{
	
	private bool leftTower = false;
	private homingMissileCamera mC;
	public AudioClip explosion;
	public GameObject expSource;
	private bool begin = false;
	
	// Use this for initialization
	void Start ()
	{
		audio.clip = explosion;
		audio.Stop();
	}
	
	public void init()
	{
		begin = true;
	}

	// Update is called once per frame
	void Update ()
	{
		if(mC == null)
		{
			mC = GetComponentInChildren<homingMissileCamera>();
		}
		if(begin)
		{
			transform.Translate(transform.up*15*Time.deltaTime,Space.World);
		}
		
		
	}
	
	void OnTriggerEnter(Collider other) {
		if(leftTower)
		{
			PlayAudioClip(explosion,transform.position,4f);
			mC.transferControl();
        	Destroy(gameObject);
		}
    }
	
	void OnTriggerExit(Collider other)
	{
		if(!leftTower)
		{
			audio.PlayOneShot(explosion,.5f);
			mC.transferControl();
			leftTower = true;
		}
	}
	
	public void kill()
	{
		PlayAudioClip(explosion,transform.position,4f);
		mC.transferControl();
        Destroy(gameObject);
	}
	
	
	AudioSource PlayAudioClip(AudioClip clip, Vector3 position, float volume) {
        GameObject go = (GameObject)Instantiate(expSource);
        go.transform.position = position;
        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.Play();
        Destroy(go, clip.length);
        return source;
    }
	
	
	
}

