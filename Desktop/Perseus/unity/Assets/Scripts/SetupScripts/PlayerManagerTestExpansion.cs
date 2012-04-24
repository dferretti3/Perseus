using UnityEngine;
using System.Collections;


public class PlayerManagerTestExpansion : MonoBehaviour {
	
	public static Color[] teams = new Color[] {Color.red,Color.blue,Color.black,Color.magenta,Color.green};
	private int currentMaxPlayers = 2;
	private int currentColor;
	private string callSign;
	
	public GameObject PlayerPrefab;
	
	private PlayerTestExpansion player;
	//public Player[] players;
	int index,playernum;
	int next_b;
	
	public float offset;
	public int max_towers;
	public static int MAX_TOWERS;
	int num_towers;
	string ip = "127.0.0.1";
	freeFlyCamera my_camera;
	
	public GameObject plane;
	Vector3[] tower_pos,normals;
	GameObject[] objects;
	Quaternion[] tower_rots;
	
	GUIStyle myStyle;
	public Font warning;
	
	private static float startingTime = 30f;
	private float timeLeft = startingTime;
	RaycastHit lastHit;
	
	Vector3[] comp_p,comp_n;
	int[] comp_t;
	string[] comp_names;
	GameObject[] comp_o;
	PlayerTestExpansion[] computers;
	public int num_computers;
	
	bool netsetup = false;
	bool isready = false;
	bool began = false;
	void OnGUI(){
			if(Network.isServer)
			GUI.Label(new Rect(10, 10, 100, 30), "Server");
			if(Network.isClient)
			GUI.Label(new Rect(10, 10, 100, 30), "Client");
			
			string turnLabel = (index==playernum)?"Place a tower":"Opponent is placing a tower";
			GUI.Label(new Rect(50,30,500,50),turnLabel,myStyle);
			
			if (index==playernum)
			{
				GUI.Label(new Rect(300,90,100,50)," "+Mathf.FloorToInt(timeLeft),myStyle);	
			}
	}
	// Use this for initialization
	void Start () {
		num_towers = 0;
		playernum = PlayerPrefs.GetInt("myNum");
		currentMaxPlayers = PlayerPrefs.GetInt("connectedPlayers");
		max_towers = PlayerPrefs.GetInt("numTowers");
		num_computers = PlayerPrefs.GetInt("numAIs");
		startingTime = (float)PlayerPrefs.GetInt("timeout");
		timeLeft = startingTime;
		
		tower_pos = new Vector3[2*max_towers];
		tower_rots = new Quaternion[2*max_towers];
		normals = new Vector3[2*max_towers];
		objects = new GameObject[2*max_towers];
		
		
		MAX_TOWERS = max_towers;
		next_b = 0;
		index = 0;
		GameObject tempObj = (GameObject)(GameObject.Instantiate(PlayerPrefab));
		player = tempObj.GetComponent<PlayerTestExpansion>();
		currentColor = PlayerPrefs.GetInt("currentTeam");
		player.color = teams[currentColor];
		player.callSign = PlayerPrefs.GetString("currentCall");
		player.my_turn = false;
		player.manager = this;
		callSign = player.callSign;
		
		//players[0].my_turn = true;
		
		GameObject cam = GameObject.Find("Main Camera");
		my_camera = cam.GetComponent<freeFlyCamera>();
		
		myStyle = new GUIStyle();
		myStyle.font = warning;
		myStyle.alignment = TextAnchor.MiddleCenter;
		myStyle.fontSize = 20;
		myStyle.normal.textColor = Color.red;
		myStyle.normal.background = null;
		
		if(Network.isServer)
		{
			computers = new PlayerTestExpansion[num_computers];
			comp_t = new int[computers.Length];
			comp_names = new string[computers.Length];
			for (int i = 0; i < num_computers; i++)
			{
				computers[i] = (GameObject.Instantiate(PlayerPrefab) as GameObject).GetComponent<PlayerTestExpansion>();
				computers[i].color = teams[PlayerPrefs.GetInt("ai"+i+"team")];
				computers[i].callSign = PlayerPrefs.GetString("ai"+i+"name");
				comp_t[i] = PlayerPrefs.GetInt("ai"+i+"team");
				comp_names[i] = PlayerPrefs.GetString("ai"+i+"name");
			}
			foreach (PlayerTestExpansion p in computers) {
				p.my_turn = false;
				p.manager = this;
			}
			comp_p = new Vector3[computers.Length];
			comp_n = new Vector3[computers.Length];
			comp_o = new GameObject[computers.Length];
			
			networkView.RPC("setStartIndex",RPCMode.All,Mathf.FloorToInt(Random.value*currentMaxPlayers));
			if (num_computers>0 && comp_o[0]==null)
			{
				float rr = 150;
				for (int i = 0; i < computers.Length; i++)
				{
					computers[i].comp_createTower(Random.Range(-rr,rr),Random.Range(-rr,rr),i,computers[i].color);	
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
			if(index == playernum && num_towers < max_towers)
			{
				Color c = teams[currentColor];
				renderer.material.color = new Color(c.r,c.g,c.b,0.5f);
				RaycastHit hitInfo = new RaycastHit();
				bool rayHit = Terrain.activeTerrain.collider.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hitInfo,5000.0f);
				renderer.enabled = rayHit;
				if (rayHit)
					lastHit = hitInfo;
				transform.position = pos(hitInfo);
				transform.rotation = rot();
			}
			if (index == playernum)
			{
				timeLeft-= Time.deltaTime;	
				if (timeLeft<=0) 
				{
					if(num_towers < max_towers)
					{
						player.forcePlacement(lastHit);
					}
					else
					{
						networkView.RPC("finishedSetup",RPCMode.All);
					}
				}
			}
	}
	
	public void mark(Vector3 hitInfo, Vector3 posit, GameObject obj) {
		//if (index!=0) return;
		tower_pos[num_towers] = posit;
		objects[num_towers] = obj;
		tower_rots[num_towers] = Quaternion.Euler(0,90,0)*Quaternion.RotateTowards(rot(),Quaternion.LookRotation(Vector3.up),15);
		normals[num_towers] = hitInfo.normalized;
		tower_rots[num_towers] = Quaternion.LookRotation(Vector3.Cross(Camera.main.transform.right,hitInfo),hitInfo);
		num_towers++;
		networkView.RPC("playerPlaced",RPCMode.All,playernum);
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
	
	[RPC]
	void setStartIndex(int startAt)
	{
		Debug.Log("Starting at index: " + startAt);
		index = startAt;
		if(index == playernum && num_towers < max_towers)
		{
			Debug.Log("starting with me");
			player.my_turn = true;
			timeLeft = startingTime;
		}
	}
	
	[RPC]
	void playerPlaced(int playerNum)
	{
		index++;
		index = index%currentMaxPlayers;
		if(index == playernum && num_towers < max_towers)
		{
			player.my_turn = true;
			timeLeft = startingTime;
		}
		else if(index == playerNum)
		{
			timeLeft = 10f;
		}
	}
	
	[RPC]
	void finishedSetup()
	{
		renderer.enabled = false;
		
		GameObject.Find("SavedData").GetComponent<SaveTowerLocsTestExpansion>().saveLocs(tower_pos,tower_rots,normals,teams[currentColor],callSign,currentColor);
		if(Network.isServer)
		GameObject.Find("SavedData").GetComponent<SaveTowerLocsTestExpansion>().saveComp(comp_p,comp_n,comp_t,comp_names);
		
		enabled = false;
		my_camera.GetComponent<AudioListener>().enabled = false;
		for (int i = 0; i < max_towers; i++)
		{
			Network.Destroy(objects[i]);
		}
		if(Network.isServer)
		{
		foreach (GameObject o in comp_o)
			GameObject.Destroy(o);
		}
		//controlTowers();
		return;
	}
}
