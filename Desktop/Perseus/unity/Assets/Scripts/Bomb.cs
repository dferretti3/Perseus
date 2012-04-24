using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour {
	
	public GameObject explosionRadius;
	
	Vector3 velocity;
	public static float projectilespeed = 80.5f;
	public static float gravity = 50.8f;
	public static Vector3 initialVelocityOffset = new Vector3(0,20,0);
	
	int lives = 3;
	public AudioClip explosion;
	public GameObject expSource;
	
	// Use this for initialization
	void Start () {
		velocity = Vector3.zero;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		if (ownedByCurrentPlayer()) {
			if (velocity.magnitude==0) velocity = transform.forward*projectilespeed + initialVelocityOffset;
			velocity += gravity*Vector3.down*Time.fixedDeltaTime;
//			transform.position += transform.forward * projectilespeed + gravity*Vector3.down;;
			transform.position += velocity*Time.fixedDeltaTime;
			if(transform.position.y > 200 || transform.position.y < -200 || transform.position.x > 300 || transform.position.x
				<-300 || transform.position.z > 300 || transform.position.z < -300)
			{
				Network.Destroy(this.gameObject);
			}
		}
	}

//	void OnTriggerEnter(Collider col)
//	{
//		if(col.gameObject.name==("Terrain") || col.gameObject.name==("ControlledMissile(Clone)") ||
//			col.gameObject.name==("homingMissile(Clone)") || col.gameObject.name == ("AIContMissile(Clone)"))
//			Network.Destroy(this.gameObject);
//	}
	
	void OnTriggerEnter (Collider col)
	{	
		if (networkView.viewID.owner!=Network.player) return;
		if(col.gameObject.name == "Bullet(Clone)")
		{
			lives--;
			if(lives<=0)
				kill();
		}
		else if(col.gameObject.name != "Money(Clone)")
		{
			GameObject g = col.gameObject;
			kill();
		}

	}
	
	public void kill ()
	{
		if(networkView.viewID.owner == Network.player)
		{
			networkView.RPC("died",RPCMode.Others,transform.position);
			PlayAudioClip(explosion,transform.position,4f);
//			transferControl();
			ParticleSystem engine = GetComponentInChildren<ParticleSystem>();
			if (engine!=null) {
				engine.transform.parent = null;
				engine.enableEmission = false;
			}
			float explosionRad = 10;
			int halfHit = 20;
			Collider[] hitTurretts = Physics.OverlapSphere(transform.position,explosionRad,1<<10);
			foreach(Collider turrett in hitTurretts)
			{
				topLevelController ttlc = turrett.transform.GetComponentInChildren<topLevelController>();
				int hitFor = (int)((explosionRad - (turrett.transform.position - transform.position).magnitude)/explosionRad)*halfHit + halfHit;
				turrett.networkView.RPC("hitTower",RPCMode.All,hitFor);
			}
//        	Network.Destroy(gameObject);
//			GameObject explode = (GameObject)Network.Instantiate(explosionRadius,transform.position,Quaternion.identity,0);
//			explode.transform.localScale *= 5;
			Vector3 pos = transform.position;
			Network.Destroy(gameObject);
			Explode(pos);
		}
	}
	
	public void Explode(Vector3 pos)
	{
		float radius = 22;
		Collider[] projectiles = Physics.OverlapSphere(pos,radius,1<<13);
		foreach (Collider proj in projectiles)
		{
			if (proj.gameObject==gameObject || !proj.gameObject.active) continue;
			kill(proj.gameObject);
		}
		Network.Instantiate(explosionRadius,pos,Quaternion.identity,0);
	}
	
	void kill(GameObject g)
	{
		if (g.GetComponent<AIControlledMissile>()!=null) {
			g.GetComponent<AIControlledMissile>().gameObject.active = false;
			g.GetComponent<AIControlledMissile>().kill();
		}
		else if (g.GetComponent<AIresourceRobot>()!=null) {
			g.GetComponent<AIresourceRobot>().gameObject.active = false;
			g.GetComponent<AIresourceRobot>().kill();
		}
		else if (g.GetComponent<Bomb>()!=null) {
			g.GetComponent<Bomb>().gameObject.active = false;
			g.GetComponent<Bomb>().kill();
		}
		else if (g.GetComponent<Bullet>()!=null) {
			if (g.networkView.viewID.owner==Network.player)
				Network.Destroy(g);
		}
		else if (g.GetComponent<ControlledMissile>()!=null) {
			g.GetComponent<ControlledMissile>().gameObject.active = false;
			g.GetComponent<ControlledMissile>().kill();
		}
		else if (g.GetComponent<homingMissileScript>()!=null) {
			g.GetComponent<homingMissileScript>().gameObject.active = false;
			g.GetComponent<homingMissileScript>().kill();
		}
		else if (g.GetComponent<resourceRobot>()!=null) {
			g.GetComponent<resourceRobot>().gameObject.active = false;
			g.GetComponent<resourceRobot>().kill();
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
	
	private bool ownedByCurrentPlayer ()
	{
		return transform.networkView.viewID.owner == Network.player;
	}
	[RPC]
	void setNavColor (Vector3 pColor, string nTag)
	{
		GetComponentInChildren<navPoint> ().subRefresh (new Color (pColor.x, pColor.y, pColor.z, 1f), nTag);
	}
	
	[RPC]
	void died(Vector3 pos)
	{
		PlayAudioClip(explosion,pos,4f);
		ParticleSystem engine = GetComponentInChildren<ParticleSystem>();
		engine.transform.parent = null;
		engine.enableEmission = false;
	}
	
	[RPC]
	void setTag(string t)
	{
		gameObject.tag = t;
	}
}
