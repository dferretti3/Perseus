using UnityEngine;
using System.Collections;

public class defensesystem : MonoBehaviour {
	public topLevelController tLC;
	public Bullet b;
	Transform origin;
	// Use this for initialization
	int side=0;
	int lead;
	int spread;
	int count = 0;
	public AudioClip shot;
	public GameObject expSource;

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	count++;
		
	if(side%2==0)
		origin = transform.FindChild("Left");
	else
		origin = transform.FindChild("Right");
		
		
	Collider[] cols = Physics.OverlapSphere(transform.position, 250);
	foreach (Collider hit in cols){
		if(hit.gameObject.name == "AIContMissile(Clone)" || hit.gameObject.name == "ControlledMissile(Clone)" || hit.gameObject.name == "homingMissileRedo(Clone)" || 
						hit.gameObject.name == "Bomb(Clone)")
		{
			GameObject closeobject = hit.gameObject;	
			if(closeobject.tag.CompareTo(""+tLC.teamNum)!=0)
			{
				Vector3 distance = transform.position - closeobject.transform.position;
				float d = distance.magnitude;
				if(d>150)
					{
						closeobject=null;
						return;
					}
				if(d<=120)
				{
					lead = 10;
					spread = 5;
				}
				if(count%25==0)
				{
					PlayAudioClip(shot,transform.position,4f);

					Bullet bul = (Bullet)Network.Instantiate(b, origin.position, origin.rotation, 0);
					bul.transform.LookAt(closeobject.transform.position+closeobject.transform.forward*lead-new Vector3(Random.Range(-spread,spread),Random.Range(-spread,spread),Random.Range(-spread,spread)));
					side++;
					break;
				}
			}
		}	
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
}
