using UnityEngine;
using System.Collections;

public class resourceRobot : MonoBehaviour
{
	private GameObject moneyTarget;
	private Vector3 tempTarget;
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(moneyTarget == null)
		{
			Collider[] coins = Physics.OverlapSphere(transform.position,100f,1<<11);
			int foundAt = -1;
			float minDist = 200;
			for(int x = 0; x < coins.Length; x++)
			{
				if((coins[x].transform.position - transform.position).magnitude < minDist)
				{
					minDist = (coins[x].transform.position - transform.position).magnitude;
					foundAt = x;
				}
			}
			if(foundAt > -1)
			{
				moneyTarget = coins[foundAt].gameObject;
			}
		}
		
		if(tempTarget == null)
		{
			
		}
	
	}
}

