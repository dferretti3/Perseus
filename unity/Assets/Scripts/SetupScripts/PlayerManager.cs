using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {
	
	public Player[] players;
	int index;
	bool next_b;
	
	public float offset;
	public int max_towers;
	int num_towers;
	
	freeFlyCamera my_camera;
	
	public GameObject plane;
	
	Vector3[] tower_pos;
	Quaternion[] tower_rots;
	
	// Use this for initialization
	void Start () {
		tower_pos = new Vector3[max_towers];
		tower_rots = new Quaternion[max_towers];
		num_towers = 0;
		next_b = false;
		index = 0;
		foreach (Player p in players) {
			p.my_turn = false;
			p.manager = this;
		}
		players[0].my_turn = true;
		
		GameObject cam = GameObject.Find("Main Camera");
		my_camera = cam.GetComponent<freeFlyCamera>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		if (num_towers>=max_towers) {
			renderer.enabled = false;
			players[index].my_turn = false;
			controlTowers();
			return;
		}
		if (next_b) {
			next_b = false;
			index++;
			index %= players.Length;
			players[index].my_turn = true;
			if (index==0) num_towers++;
		}
		Color c = players[index].color;
		renderer.material.color = new Color(c.r,c.g,c.b,0.5f);
		RaycastHit hitInfo = new RaycastHit();
		renderer.enabled = plane.collider.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hitInfo,1000.0f);
		transform.position = pos(hitInfo);
		transform.rotation = rot();
	}
	
	public void mark(RaycastHit hitInfo) {
		if (index!=0) return;
		tower_pos[num_towers] = pos(hitInfo)+0.5f*Vector3.up;
		tower_rots[num_towers] = Quaternion.Euler(0,90,0)*Quaternion.RotateTowards(rot(),Quaternion.LookRotation(Vector3.up),15);
	}
	
	int cont = 0;
	
	void controlTowers() {
		bool over;
		if (over=Input.GetKeyDown("tab"))
			cont += Input.GetKey("left shift") ? (max_towers-1) : 1;
		cont %= max_towers;
		my_camera.transform.position = tower_pos[cont];
		my_camera.transform.rotation = tower_rots[cont];
		my_camera.changeMode(over);
	}
	
	public void next() {
		next_b = true;
	}
	
	public Vector3 pos() {
		float x = my_camera.transform.position.x;
		float z = my_camera.transform.position.z;
		return new Vector3(x,0,z) + Quaternion.Euler(0,my_camera.mr_x,0)*Vector3.forward*offset;
	}
	
	public Vector3 pos(RaycastHit hitInfo) {
		float x = hitInfo.point.x;
		float z = hitInfo.point.z;
		return new Vector3(x,0,z);
	}
	
	public Quaternion rot() {
		return Quaternion.Euler(0,my_camera.mr_x-90,0);
	}
}
