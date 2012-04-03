using UnityEngine;
using System.Collections;

public class asteroidScript : MonoBehaviour {
	
	private float life = 50;
	private float speed = 0;
	private float side;
	private Vector3 unitSpeed;
	public Material[] selection;
	private Vector3 angRot;
	private Vector3 normal;
	private float maxRot = 45;
	
	// Use this for initialization
	void Start () {
		renderer.material = selection[Mathf.FloorToInt(Random.value*selection.Length)];
		angRot = new Vector3(Random.value*maxRot - maxRot/2,Random.value*maxRot - maxRot/2,Random.value*maxRot - maxRot/2);
	}
	
	public void initialize(float vel, float field,Vector3 norm)
	{
		
		speed = vel;
		side = field;
		life = side/speed;
		normal = norm;
		normal = normal.normalized;
		
	}
	
	// Update is called once per frame
	void Update () {
		
		transform.position = transform.position + normal*speed*Time.deltaTime;
		transform.Rotate(angRot*Time.deltaTime);
		life -= Time.deltaTime;
		
		if(life <= 0)
		{
			Destroy(gameObject);
		}
		
	}
}
