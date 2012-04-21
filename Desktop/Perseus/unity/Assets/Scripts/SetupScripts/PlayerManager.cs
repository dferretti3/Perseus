using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {
	
	public GameObject PlayerPrefab;
	
	public Player[] players;
	int index,playernum;
	int next_b;
	
	public float offset;
	public int max_towers;
	int num_towers;
	string ip = "127.0.0.1";
	freeFlyCamera my_camera;
	
	public GameObject plane;
	Vector3[] tower_pos,normals;
	GameObject[] objects;
	Quaternion[] tower_rots;
	
	GUIStyle myStyle;
	public Font warning;
	
	int timeLeft = 1259;
	RaycastHit lastHit;
	
	Vector3[] comp_p,comp_n;
	GameObject[] comp_o;
	Player[] computers;
	public int num_computers;
	
	bool netsetup = false;
	bool isready = false;
	bool began = false;
	void OnGUI(){
		if(!netsetup)
		{
			if(GUI.Button(new Rect(10,10,100,20), "Create Server"))
			{
				Network.InitializeServer(2, 25000);
				PlayerPrefs.SetInt("playerNum",playernum=0);
			}
			ip = GUI.TextArea(new Rect(10,30,200,20), ip);
			if(GUI.Button(new Rect(10, 50, 100, 20), "Connect"))
			{
				Network.Connect(ip, 25000);
				PlayerPrefs.SetInt("playerNum",playernum=1);
			}
			if(Network.isClient || Network.isServer)
				netsetup=true;
		}
		if(netsetup)
		{
			if(Network.isServer)
			GUI.Label(new Rect(10, 10, 100, 30), "Server");
			if(Network.isClient)
			GUI.Label(new Rect(10, 10, 100, 30), "Client");
			
			string turnLabel = (index==playernum)?"Place a tower":"Opponent is placing a tower";
			GUI.Label(new Rect(50,30,500,50),turnLabel,myStyle);
			
			if (index==playernum)
			{
				GUI.Label(new Rect(300,90,100,50),""+timeLeft/60,myStyle);	
			}
		}
	}
	// Use this for initialization
	void Start () {
		tower_pos = new Vector3[2*max_towers];
		tower_rots = new Quaternion[2*max_towers];
		normals = new Vector3[2*max_towers];
		objects = new GameObject[2*max_towers];
		computers = new Player[num_computers];
		for (int i = 0; i < num_computers; i++)
		{
			computers[i] = (GameObject.Instantiate(PlayerPrefab) as GameObject).GetComponent<Player>();
			computers[i].color = new Color(Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f));
		}
		comp_p = new Vector3[computers.Length];
		comp_n = new Vector3[computers.Length];
		comp_o = new GameObject[computers.Length];
		num_towers = 0;
		max_towers = 1;
		next_b = 0;
		index = 0;
		foreach (Player p in players) {
			p.my_turn = false;
			p.manager = this;
		}
		foreach (Player p in computers) {
			p.my_turn = false;
			p.manager = this;
		}
		//players[0].my_turn = true;
		
		GameObject cam = GameObject.Find("Main Camera");
		my_camera = cam.GetComponent<freeFlyCamera>();
		
		myStyle = new GUIStyle();
		myStyle.font = warning;
		myStyle.alignment = TextAnchor.MiddleCenter;
		myStyle.fontSize = 20;
		myStyle.normal.textColor = Color.red;
		myStyle.normal.background = null;
	}
	
	void OnServerInitialized() { 
		print("STARTED");
	}
	
	void OnConnectedToServer() { 
		print("CONNECTED");
		isready = true;
	}
	
	void OnFailedToConnect(NetworkConnectionError e){
		print("X " + e);
	}
	// Update is called once per frame
	void FixedUpdate () {
		
		if (Input.GetKeyDown("n")) {
			//Application.LoadLevel("testScene");
		}
		
		if (Network.isServer && Network.connections.Length>0) isready = true;
		
		if (!began && isready)
		{
			print(players);
			print(players[0]);
			players[0].my_turn = true;
			began = true;
			print("STARTING - Server and Client are ready");
		}
		
		if(netsetup){
			if (num_towers>=max_towers) {
				renderer.enabled = false;
				players[index].my_turn = false;		
				
				
				
				GameObject.Find("SavedData").GetComponent<SaveTowerLocs>().saveLocs(tower_pos[0],tower_pos[1],tower_rots[0],tower_rots[1],normals[0],normals[1]);
				GameObject.Find("SavedData").GetComponent<SaveTowerLocs>().saveComp(comp_p,comp_n);
				//Application.LoadLevel("testScene");
				enabled = false;
				my_camera.GetComponent<AudioListener>().enabled = false;
				GameObject.Destroy(objects[0]);
				GameObject.Destroy(objects[1]);
				foreach (GameObject o in comp_o)
					GameObject.Destroy(o);
				//controlTowers();
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
			bool rayHit = Terrain.activeTerrain.collider.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hitInfo,5000.0f);
			renderer.enabled = rayHit;
			if (rayHit)
				lastHit = hitInfo;
			transform.position = pos(hitInfo);
			transform.rotation = rot();
			if (began && index==playernum)
			{
				timeLeft--;	
				if (timeLeft<=0) players[index].forcePlacement(lastHit);
			}
			
			if (began && num_computers>0 && comp_o[0]==null)
			{
				float rr = 150;
				for (int i = 0; i < computers.Length; i++)
				{
					computers[i].comp_createTower(Random.Range(-rr,rr),Random.Range(-rr,rr),i,computers[i].color);	
				}
			}
		}
	}
	
	public void mark(Vector3 hitInfo, Vector3 posit, GameObject obj) {
		//if (index!=0) return;
		tower_pos[index] = posit;
		objects[index] = obj;
		tower_rots[index] = Quaternion.Euler(0,90,0)*Quaternion.RotateTowards(rot(),Quaternion.LookRotation(Vector3.up),15);
		normals[index] = hitInfo.normalized;
		tower_rots[index] = Quaternion.LookRotation(Vector3.Cross(Camera.main.transform.right,hitInfo),hitInfo);
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
		float y = Terrain.activeTerrain.SampleHeight(new Vector3(x,0,z));
		return new Vector3(x,y,z);
	}
	
	public Quaternion rot() {
		return Quaternion.Euler(0,my_camera.mr_x-90,0);
	}
	
	public void setComp(Vector3 n, Vector3 p, GameObject obj, int i)
	{
		comp_n[i] = n;
		comp_p[i] = p;
		comp_o[i] = obj;
	}
}
