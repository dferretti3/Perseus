using UnityEngine;
using System.Collections;

public class lobbyScript : MonoBehaviour {
	
	
	private string[] sampleNames = new string[] {"PLYR","YOYO","DUCK","BONE","CRAB","FIDO","SPOT","BOND","GRIM","HACK","MOON"};
	private bool connected = false;
	private bool isHost = false;
	private int plyrNum = -1;
	private int maxPlayers = -1;
	private string callSign = "";
	private int currentColor = 0;
	private string ip = "127.0.0.1";
	private ArrayList names;
	private ArrayList colors;
	public Texture2D colorPicker;
	
	
	void OnGUI()
	{
		if(!connected)
		{
			if(GUI.Button(new Rect(10,10,100,20), "Create Server"))
			{
				Network.InitializeServer(5, 25000);
				isHost = true;
				if(callSign == "")
				{
					selectRandomName();
				}
			}
			ip = GUI.TextArea(new Rect(10,30,200,20), ip);
			if(GUI.Button(new Rect(110, 10, 100, 20), "Connect"))
			{
				Debug.Log("Trying to connect");
				Network.Connect(ip, 25000);
				Debug.Log("Done trying to connect");
				if(callSign == "")
				{
					selectRandomName();
				}
			}
			GUI.Label(new Rect(10,50,60,20),"Call Tag:");
			callSign = GUI.TextField(new Rect(30,70,60,20),callSign,4);
			if(Network.isClient || Network.isServer)
			{
				connected = true;
			}
		}
		else
		{
			for(int x = 0; x < names.Count; x++)
			{
				GUIContent gC = new GUIContent(colorPicker,"Scroll");
				GUI.Label(new Rect(10,100 + 20*x,70,20),"" + (x+1) + ". " + (string)names[x]);
				GUI.color = PlayerManagerTestExpansion.teams[(int)colors[x]];
				GUI.Label(new Rect(100,100 + 20*x,80,20),gC);
				GUI.color = Color.white;
			}
		}
	}
	
	void OnPlayerConnected(NetworkPlayer player) {
        Debug.Log("Player " + maxPlayers + " connected from " + player.ipAddress + ":" + player.port);
		networkView.RPC("distributeCallSign",player,currentColor,callSign,plyrNum);
		if(isHost)
		{
			Debug.Log("Giving player their number");
			networkView.RPC("tellNumber",player,maxPlayers);
			Debug.Log("Increasing player count");
			networkView.RPC("increaseMaxNum",RPCMode.All,maxPlayers+1);
			Debug.Log("Done");
		}
    }
	
	void OnServerInitialized() { 
		plyrNum = 0;
		maxPlayers = 1;
		names.Add(callSign);
		colors.Add(currentColor = Mathf.FloorToInt(Random.value*PlayerManagerTestExpansion.teams.Length));
	}
	
	void OnConnectedToServer() { 
		connected = true;
		print("CONNECTED");
	}
	
	void OnFailedToConnect(NetworkConnectionError e){
		print("X " + e);
	}
	
	
	// Use this for initialization
	void Start () {
		selectRandomName();
		names = new ArrayList();
		colors = new ArrayList();
	}
	
	private void selectRandomName()
	{
		callSign = sampleNames[Mathf.FloorToInt(Random.value*sampleNames.Length)];
	}
	
	// Update is called once per frame
	void Update () {
		
		if(Network.isClient || Network.isServer)
		{
			connected = true;
		}
		
		
	
	}
	
	[RPC]
	void tellNumber(int myNum)
	{
		
		plyrNum = myNum;
		currentColor = Mathf.FloorToInt(Random.value*PlayerManagerTestExpansion.teams.Length);
		networkView.RPC("distributeCallSign",RPCMode.All,currentColor,callSign,plyrNum);
	}
	
	[RPC]
	void increaseMaxNum(int newMax)
	{
		maxPlayers = newMax;
	}
	
	[RPC]
	void distributeCallSign(int initColor, string name,int player)
	{
		Debug.Log("Receiving init for player " + player);
		while(names.Count <= player)
		{
			names.Add("");
		}
		names[player] = name;
		while(colors.Count <= player)
		{
			colors.Add(0);
		}
		colors[player] = initColor;
	}
	
	[RPC]
	void distributeColorChange(int color, int player)
	{
		while(colors.Count <= player)
		{
			colors.Add(0);
		}
		colors[player] = color;
	}
}
