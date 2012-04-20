using UnityEngine;
using System.Collections;

public class AIControl : MonoBehaviour {
	public AIControlledMissile missile;
	public Bullet b;
	int count;
	int lead;
	int spread;
	GameObject target;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		count++;
		Collider[] cols = Physics.OverlapSphere(transform.position, 300);
		foreach (Collider hit in cols){
			if((hit.tag == "P1" || hit.tag == "P2"))
			{
				target = hit.gameObject;
				break;
			}	
		}
		if(target!=null)
		{
			Vector3 distance = transform.position - target.transform.position;
			float d = distance.sqrMagnitude;
			print(d);
			if(d<=20000)
			{
				lead = 10;
				spread = 2;
			}
			if(d>20000)
			{
				lead = 40;
				spread = 5;
			}
			transform.LookAt(target.transform.position+target.transform.forward*lead);
			if(count%20==0)
			{
				transform.LookAt(target.transform.position+target.transform.forward*lead-new Vector3(Random.Range(-spread,spread),Random.Range(-spread,spread),Random.Range(-spread,spread)));
				Network.Instantiate(b, transform.position+transform.forward*20, transform.rotation, 0);
			}
		}
	}
}

