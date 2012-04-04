using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	
	public Color color;
	public bool my_turn;
	
	public GameObject turret;
	
	public PlayerManager manager;
	
	public GameObject plane;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (!my_turn) return;
		if (Input.GetMouseButtonDown(0)) {
			RaycastHit hitInfo = new RaycastHit();
			if (!plane.collider.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hitInfo,1000.0f)) return;
			//GameObject obj = GameObject.Instantiate(turret,manager.pos(hitInfo),manager.rot()) as GameObject;
			GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			obj.transform.position = manager.pos(hitInfo);
			obj.transform.rotation = manager.rot();
			obj.renderer.material.color = color;
			//obj.GetComponent<MeshFilter>().mesh = manager.mesh;
			my_turn = false;
			manager.mark(hitInfo);
			manager.next();
		}
	}
}
