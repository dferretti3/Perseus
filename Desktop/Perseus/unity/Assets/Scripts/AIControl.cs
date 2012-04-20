using UnityEngine;
using System.Collections;

public class AIControl : MonoBehaviour {
	public AIControlledMissile missile;
	public Bullet b;
	int count;
	int lead;
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
				Vector3 distance = transform.position - hit.gameObject.transform.position;
				float d = distance.sqrMagnitude;
				print(d);
				if(d<=20000)
					lead = 10;
				if(d>20000)
					lead = 40;
				transform.LookAt(hit.gameObject.transform.position+hit.gameObject.transform.forward*lead);
				if(count%20==0)
				{
					transform.LookAt(hit.gameObject.transform.position+hit.gameObject.transform.forward*lead-new Vector3(Random.Range(-5,5),Random.Range(-5,5),Random.Range(-5,5)));
					Network.Instantiate(b, transform.position+transform.forward*20, transform.rotation, 0);
				}
			}
		}
	}
}
