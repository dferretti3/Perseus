using UnityEngine;
using System.Collections;

public class PlayerTestExpansion : MonoBehaviour {
	
	public Color color;
	public bool my_turn;
	private Color alphaedColor;
	public GameObject networkedMarker;
	
	public GameObject turret;
	
	public PlayerManagerTestExpansion manager;
	
	bool forceFlag = false;
	RaycastHit forceHit;
	
	//public GameObject plane;
	RaycastHit hitInfo;
	// Use this for initialization
	void Start () {
		my_turn=false;
	}
	
	// Update is called once per frame
	void Update () {
		if (!my_turn) return;
		if ((Input.GetMouseButtonDown(0)||forceFlag)) {
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
		GameObject obj = (GameObject)Network.Instantiate(networkedMarker,manager.pos(hitInfo),manager.rot(),0);
		networkMarkerScript nMS = obj.GetComponent<networkMarkerScript>();
		nMS.pushColor(color);
		my_turn = false;
		forceFlag = false;
		manager.mark(hitInfo.normal, obj.transform.position, obj);
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
		Vector3 pos;
		Quaternion rot;
		pos=new Vector3(x,y,z);
		rot=Quaternion.Euler(0,r,0);
		GameObject obj = (GameObject)Network.Instantiate(networkedMarker,pos,rot,0);
		networkMarkerScript nMS = obj.GetComponent<networkMarkerScript>();
		nMS.pushColor(color);
		manager.setComp(Vector3.up,pos,obj,i);
	}
}
