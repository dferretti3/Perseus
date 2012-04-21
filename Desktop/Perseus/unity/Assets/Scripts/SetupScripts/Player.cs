using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	
	public Color color;
	public bool my_turn;
	
	public GameObject turret;
	
	public PlayerManager manager;
	
	bool forceFlag = false;
	RaycastHit forceHit;
	
	public GameObject plane;
	RaycastHit hitInfo;
	// Use this for initialization
	void Start () {
		my_turn=false;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (!my_turn) return;
		if (gameObject.name=="Player1" && Network.isServer && Input.GetMouseButtonDown(0)) {
			createTower();
		}
		if (gameObject.name=="Player2" && Network.isClient && Input.GetMouseButtonDown(0)) {
			createTower();
		}
	}
	void createTower(){
		if (!forceFlag) {
			hitInfo = new RaycastHit();
			if (!Terrain.activeTerrain.collider.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hitInfo,5000.0f)) return;
		} else {
			hitInfo = forceHit;
		}
		//GameObject obj = GameObject.Instantiate(turret,manager.pos(hitInfo),manager.rot()) as GameObject;
		GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
		obj.transform.localScale*=5;
		obj.transform.position = manager.pos(hitInfo);
		obj.transform.rotation = manager.rot();
		obj.renderer.material.color = color;
		my_turn = false;
		manager.mark(hitInfo.normal, obj.transform.position, obj);
		manager.next();
		if(Network.isClient)
			networkView.RPC("createTower2", RPCMode.Server, obj.transform.position, hitInfo.normal);
		if(Network.isServer)
			networkView.RPC("createTower2", RPCMode.Others, obj.transform.position, hitInfo.normal);
		//obj.GetComponent<MeshFilter>().mesh = manager.mesh;
	}
	
	[RPC]
	void createTower2(Vector3 pos, Vector3 normal){
		GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
		obj.transform.localScale = new Vector3(5,5,5);
		obj.transform.position = pos;
		obj.transform.rotation = manager.rot();
		obj.renderer.material.color = color;
		my_turn = false;
		manager.mark(normal, obj.transform.position,obj);
		manager.next();
	}
	
	[RPC]
	void createTower2_comp(Vector3 pos, Vector3 normal, Quaternion rot, int i,Vector3 color) {
		GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
		obj.transform.localScale = new Vector3(5,5,5);
		obj.transform.position = pos;
		obj.transform.rotation = rot;
		obj.renderer.material.color = new Color(color.x,color.y,color.z);
		my_turn = false;
		manager.setComp(normal,pos,obj, i);
	}
	
	public void forcePlacement(RaycastHit hi)
	{
		forceHit = hi;
		forceFlag = true;
	}
	
	public void comp_createTower(float x, float z, int i, Color color)
	{
		if (!Network.isServer) return;
		float y = Terrain.activeTerrain.SampleHeight(new Vector3(x,0,z));
		float r = Random.Range(0,2*Mathf.PI);
		GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
		obj.transform.localScale*=5;
		Vector3 normal,pos;
		Quaternion rot;
		obj.transform.position = pos=new Vector3(x,y,z);
		obj.transform.rotation = rot=Quaternion.Euler(0,r,0);
		obj.renderer.material.color = color;
		networkView.RPC("createTower2_comp",RPCMode.Others, obj.transform.position, normal=Vector3.up,rot, i, new Vector3(color.r,color.g,color.b));
		manager.setComp(normal,pos,obj,i);
	}
}
