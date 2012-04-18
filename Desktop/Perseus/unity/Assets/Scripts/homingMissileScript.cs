using UnityEngine;
using System.Collections;

public class homingMissileScript : MonoBehaviour
{
	
	private bool leftTower = false;
	private homingMissileCamera mC;
	public AudioClip explosion;
	public GameObject expSource;
	private bool begin = false;
	private bool hasTarget = false;
	private float normalSpeed = 15;
	private float targetSpeed = 45;
	int lives = 2;
	// Use this for initialization
	void Start ()
	{
		audio.clip = explosion;
		audio.Stop();
	}
	
	private bool ownedByCurrentPlayer ()
	{
		return networkView.viewID.owner == Network.player;
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
			float speed = normalSpeed;
			if(hasTarget)
			{
				speed = targetSpeed;
			}
			
			transform.Translate(transform.up*speed*Time.deltaTime,Space.World);
		}
		
		
	}
	
	public void toggleTarget()
	{
		hasTarget = !hasTarget;
	}
	
	void OnTriggerEnter(Collider other) {
		if(other.gameObject.name == "Bullet(Clone)")
		{
			lives--;
			if(lives<=0)
				kill();
		}
		else if(leftTower && ownedByCurrentPlayer() && other.gameObject.name != "Money(Clone)")
		{
			PlayAudioClip(explosion,transform.position,4f);
			mC.transferControl();
        	Network.Destroy(gameObject);
		}	
    }
	
	void OnTriggerExit(Collider other)
	{
		if(!leftTower && ownedByCurrentPlayer() && other.gameObject.name != "Money(Clone)")
		{
			audio.PlayOneShot(explosion,.5f);
			mC.transferControl();
			leftTower = true;
		}
	}
	
	public void kill()
	{
		if(ownedByCurrentPlayer())
		{
			networkView.RPC("died",RPCMode.Others,transform.position);
			PlayAudioClip(explosion,transform.position,4f);
			mC.transferControl();
        	Network.Destroy(gameObject);
		}
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
	
	[RPC]
	void setNavColor(Vector3 pColor, string nTag)
	{
		Debug.Log("Received notification to change nav color...");
		GetComponentInChildren<navPoint>().subRefresh(new Color(pColor.x,pColor.y,pColor.z,1f),nTag);
	}
	
	[RPC]
	void died(Vector3 pos)
	{
		PlayAudioClip(explosion,pos,4f);
	}
	
	
	
}

