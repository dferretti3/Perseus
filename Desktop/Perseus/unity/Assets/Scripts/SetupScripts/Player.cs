using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	
	public Color color;
	public bool my_turn;
	
	public GameObject turret;
	
	public PlayerManager manager;
	
	public GameObject plane;
	RaycastHit hitInfo;
	// Use this for initialization
	void Start () {
		my_turn=false;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (!my_turn) return;
		if (Input.GetMouseButtonDown(0)) {
			createTower();
		}
	}
	void createTower(){
		hitInfo = new RaycastHit();
		if (!Terrain.activeTerrain.collider.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hitInfo,5000.0f)) return;
		//GameObject obj = GameObject.Instantiate(turret,manager.pos(hitInfo),manager.rot()) as GameObject;
		GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
		obj.transform.position = manager.pos(hitInfo);
		obj.transform.rotation = manager.rot();
		obj.renderer.material.color = color;
		my_turn = false;
		manager.mark(hitInfo, obj.transform.position);
		manager.next();
		if(Network.isClient)
			networkView.RPC("createTower2", RPCMode.Server, obj.transform.position);
		if(Network.isServer)
			networkView.RPC("createTower2", RPCMode.Others, obj.transform.position);
		//obj.GetComponent<MeshFilter>().mesh = manager.mesh;
	}
	
	[RPC]
	void createTower2(Vector3 pos){
		GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
		obj.transform.position = pos;
		obj.transform.rotation = manager.rot();
		obj.renderer.material.color = color;
		my_turn = false;
		manager.mark(hitInfo, obj.transform.position);
		manager.next();
	}
}
