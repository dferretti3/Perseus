using UnityEngine;
using System.Collections;

public class asteroidField : MonoBehaviour {
	
	private Vector3 centerPoint;
	private Vector3 normal;
	private float planeWidth = 500f;
	private Vector3 xS;
	private Vector3 yS;
	private float chance = .99f;
	public GameObject asteroid;
	
	
	// Use this for initialization
	void Start () {
		
		centerPoint = Random.onUnitSphere*planeWidth;
		normal = Vector3.zero - centerPoint;
		if(normal.x != 0)
		{
			xS = Vector3.Cross(normal,new Vector3(0f,normal.y,normal.z));
		}
		else
		{
			xS = new Vector3(1f,0f,0f);
		}
		yS = Vector3.Cross(normal,xS);
		xS = xS.normalized;
		yS = yS.normalized;
		
		
	}
	
	private Vector3 asteroidPosFrom(Vector2 randoms)
	{
		Vector3 toRet = centerPoint;
		toRet += xS*randoms.x;
		toRet += yS*randoms.y;
		return toRet;
	}
	
	// Update is called once per frame
	void Update () {
		//return;
		if(Random.value < (1 - Mathf.Pow(chance,1+Time.deltaTime)))
		{
			Vector2 rs = Random.insideUnitCircle*planeWidth;
			GameObject tempAs = (GameObject)Instantiate(asteroid);
			tempAs.transform.position = asteroidPosFrom(rs);
			float size = Random.value*50f + 25f;
			tempAs.transform.localScale = new Vector3(size,size,size);
			while(true)
			{
				if(Physics.SphereCast(new Ray(tempAs.transform.position,normal),size,planeWidth*4,1<<10))
				{
					rs = new Vector2(Random.value*planeWidth - planeWidth/2,Random.value*planeWidth - planeWidth/2);
					tempAs.transform.position = asteroidPosFrom(rs);
				}
				else
				{
					break;
				}
			}
			tempAs.transform.forward = normal;
			asteroidScript aScript = tempAs.GetComponent<asteroidScript>();
			aScript.initialize(Random.value*30f + 10f,planeWidth*3,normal);
		}
		
		
	}
}
