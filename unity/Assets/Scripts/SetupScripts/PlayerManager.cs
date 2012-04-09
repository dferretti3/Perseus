using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {
	
	public Player[] players;
	int index;
	int next_b;
	
	public float offset;
	public int max_towers;
	int num_towers;
	string ip = "143.215.207.77";
	freeFlyCamera my_camera;
	
	public GameObject plane;
	
	Vector3[] tower_pos;
	Quaternion[] tower_rots;
	void OnGUI(){
		if(GUI.Button(new Rect(10,10,100,20), "Create Server"))
		{
			Network.InitializeServer(2, 25000);
		}
		ip = GUI.TextArea(new Rect(10,30,200,20), ip);
		if(GUI.Button(new Rect(10, 50, 100, 20), "Connect"))
		{
			Network.Connect(ip, 25000);
		}
		
	}
	// Use this for initialization
	void Start () {
		tower_pos = new Vector3[max_towers];
		tower_rots = new Quaternion[max_towers];
		num_towers = 0;
		max_towers = 1;
		next_b = 0;
		index = 0;
		foreach (Player p in players) {
			p.my_turn = false;
			p.manager = this;
		}
		players[0].my_turn = true;
		
		GameObject cam = GameObject.Find("Main Camera");
		my_camera = cam.GetComponent<freeFlyCamera>();
	}
	
	void OnServerInitialized() { 
		print("STARTED");
	}
	
	void OnConnectedToServer() { 
		print("CONNECTED");
	}
	
	void OnFailedToConnect(NetworkConnectionError e){
		print("X " + e);
	}
	// Update is called once per frame
	void FixedUpdate () {
		
		if (Input.GetKeyDown("n")) {
			Application.LoadLevel("testScene");
		}
		if (num_towers>=max_towers) {
			renderer.enabled = false;
			players[index].my_turn = false;
			SaveTowerLocs.saveLocs(tower_pos[0],tower_pos[1],tower_rots[0],tower_rots[1]);
			Application.LoadLevel("testScene");
			controlTowers();
			return;
		}
		if (next_b>0) next_b++;
		if (next_b>=10) {
			next_b = 0;
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
		next_b = 1;
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
