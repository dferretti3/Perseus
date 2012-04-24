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
		if((hit.gameObject.name == "AIContMissile(Clone)" || hit.gameObject.name == "homingMissileRedo(Clone)" || hit.gameObject
		.name == "ControlledMissile(Clone)"))
		{
			GameObject closeobject = hit.gameObject;	
			if(closeobject.transform.networkView.viewID.owner != Network.player)
			{
				Vector3 distance = transform.position - closeobject.transform.position;
				float d = distance.sqrMagnitude;
				if(d<=20000)
				{
					lead = 10;
					spread = 2;
				}
				if(d>20000)
				{
					lead = 30;
					spread = 3;
				}
				if(count%20==0)
				{
					Bullet bul = (Bullet)Network.Instantiate(b, origin.position, origin.rotation, 0);
					bul.transform.LookAt(closeobject.transform.position+closeobject.transform.forward*lead-new Vector3(Random.Range(-spread,spread),Random.Range(-spread,spread),Random.Range(-spread,spread)));
					side++;
				}
			}
		}	
	}
		
	
	}
}
