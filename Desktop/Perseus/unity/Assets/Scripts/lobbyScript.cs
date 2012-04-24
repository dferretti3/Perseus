using UnityEngine;
using System.Collections;

public class lobbyScript : MonoBehaviour {
	
	
	private string[] sampleNames = new string[] {"PLYR","YOYO","DUCK","BONE","CRAB","FIDO","SPOT","BOND","GRIM","HACK","TANK"};
	private string[] sampleAINames = new string[] {"TINY", "BIGS", "HART", "NOOB", "NEMO", "SOLO", "TOBY", "BOOT", "TIRE", "FISH", "WHAM", "MOON", "WEST", "RAIN", "ARTI"};
	private bool connected = false;
	private bool isHost = false;
	private int plyrNum = -1;
	private int maxPlayers = -1;
	private int timeout = 30;
	private string callSign = "";
	private int currentColor = 0;
	private int numTowers = 1;
	private string ip = "127.0.0.1";
	
	
	private ArrayList names;
	private ArrayList colors;
	private ArrayList aiNames;
	private ArrayList aiColors;
	
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
			Rect towerCountRect = new Rect(230,10,100,20);
			if(isHost)
			{
				if(GUI.Button(new Rect(10,10,100,20),"Add AI"))
				{
					networkView.RPC("distributeAICallSign",RPCMode.All,randomColor(),randomAIName(),aiNames.Count);
				}
				if(GUI.Button(new Rect(120,10,100,20),"Remove AI"))
				{
					networkView.RPC("eraseLastAI",RPCMode.All);
				}
				
				if(GUI.Button(new Rect(Screen.width-110,10,100,20),"Start Game"))
				{
					networkView.RPC("beginGame",RPCMode.All);
				}
				
				
				if(towerCountRect.Contains(Event.current.mousePosition))
				{
					int scroll = Mathf.RoundToInt(Input.GetAxis("Mouse ScrollWheel") * 10.0f);
					if (scroll!=0)
					{
						networkView.RPC("setTowerNums",RPCMode.All,numTowers - scroll);
					}
				}
				
				
				Rect timeoutRect = new Rect(340,10,100,20);
				GUI.Button(timeoutRect,"Timeout(s): " + timeout);
				if(timeoutRect.Contains(Event.current.mousePosition))
				{
					int scroll = Mathf.RoundToInt(Input.GetAxis("Mouse ScrollWheel") * 10.0f);
					if (scroll!=0)
					{
						networkView.RPC("setTimeout",RPCMode.All,timeout + scroll);
					}
				}
			}
			
			GUI.Button(towerCountRect,"Num Towers: " + numTowers);
			
			for(int x = 0; x < names.Count; x++)
			{
				GUI.Label(new Rect(10,100 + 25*x,70,20),"" + (x+1) + ". " + (string)names[x]);
				GUI.color = PlayerManagerTestExpansion.teams[(int)colors[x]];
				Rect sampleRect = new Rect(100,100 + 25*x,80,20);
				GUI.DrawTexture(sampleRect,colorPicker,ScaleMode.StretchToFill);
				GUI.color = Color.white;
				if(x == plyrNum && sampleRect.Contains(Event.current.mousePosition))
				{
					GUI.Label(new Rect(10,Screen.height-20,150,20),"Scroll to change color");
					int scroll = Mathf.RoundToInt(Input.GetAxis("Mouse ScrollWheel") * 10.0f);
					if (scroll!=0)
					{
						int maxColors = PlayerManagerTestExpansion.teams.Length;
						currentColor += scroll;
						if(currentColor < 0)
						{
							currentColor += maxColors;
						}
						currentColor = currentColor%maxColors;
						networkView.RPC("distributeColorChange",RPCMode.All,currentColor,plyrNum);
					}
				}
			}
			int currentPlayerCount = names.Count;
			for(int x = 0; x < aiNames.Count; x++)
			{
				GUI.Label(new Rect(10,100 + 25*(x+currentPlayerCount),70,20),"" + ((x+currentPlayerCount)+1) + ". " + (string)aiNames[x]);
				GUI.color = PlayerManagerTestExpansion.teams[(int)aiColors[x]];
				Rect sampleRect = new Rect(100,100 + 25*(x+currentPlayerCount),80,20);
				GUI.DrawTexture(sampleRect,colorPicker,ScaleMode.StretchToFill);
				GUI.color = Color.white;
				if(isHost && sampleRect.Contains(Event.current.mousePosition))
				{
					GUI.Label(new Rect(10,Screen.height-20,150,20),"Scroll to change color");
					int scroll = Mathf.RoundToInt(Input.GetAxis("Mouse ScrollWheel") * 10.0f);
					if (scroll!=0)
					{
						int maxColors = PlayerManagerTestExpansion.teams.Length;
						int tempCurColor = (int)aiColors[x];
						tempCurColor += scroll;
						if(tempCurColor < 0)
						{
							tempCurColor += maxColors;
						}
						aiColors[x] = tempCurColor%maxColors;
						networkView.RPC("distributeAIColorChange",RPCMode.All,(int)aiColors[x],x);
					}
				}
			}
			
			
		}
	}
	
	void OnPlayerConnected(NetworkPlayer player) {
        Debug.Log("Player " + maxPlayers + " connected from " + player.ipAddress + ":" + player.port);
		for(int x = 0; x < names.Count; x++)
		{
			networkView.RPC("distributeCallSign",player,colors[x],names[x],x);
		}
		Debug.Log("Giving player their number");
		networkView.RPC("tellNumber",player,maxPlayers);
		Debug.Log("Increasing player count");
		networkView.RPC("increaseMaxNum",RPCMode.All,maxPlayers+1);
		Debug.Log("Done");
		for(int x = 0; x < aiNames.Count; x++)
		{
			networkView.RPC("distributeAICallSign",player,(int)aiColors[x],(int)aiNames[x],x);
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
		aiNames = new ArrayList();
		aiColors = new ArrayList();
	}
	
	private void selectRandomName()
	{
		callSign = sampleNames[Mathf.FloorToInt(Random.value*sampleNames.Length)];
	}
	
	private string randomAIName()
	{
		return sampleAINames[Mathf.FloorToInt(Random.value*sampleAINames.Length)];
	}
	
	private int randomColor()
	{
		return Mathf.FloorToInt(Random.value*PlayerManagerTestExpansion.teams.Length);
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
	
	[RPC]
	void distributeAIColorChange(int color, int aiPlayer)
	{
		while(aiColors.Count <= aiPlayer)
		{
			aiColors.Add(0);
		}
		aiColors[aiPlayer] = color;
	}
	
	[RPC]
	void distributeAICallSign(int initColor,string name, int aiPlayer)
	{
		Debug.Log("Receiving init for AI " + aiPlayer);
		while(aiNames.Count <= aiPlayer)
		{
			aiNames.Add("");
		}
		aiNames[aiPlayer] = name;
		while(aiColors.Count <= aiPlayer)
		{
			aiColors.Add(0);
		}
		aiColors[aiPlayer] = initColor;
	}
	
	[RPC]
	void eraseLastAI()
	{
		if(aiNames.Count >= 1)
		{
			aiNames.RemoveAt(aiNames.Count-1);
			aiColors.RemoveAt(aiColors.Count-1);
		}
	}
	
	[RPC]
	void setTowerNums(int toThis)
	{
		numTowers = toThis;
		while(numTowers > 5)
		{
			numTowers -= 5;
		}
		while(numTowers < 1)
		{
			numTowers += 5;
		}
	}
	
	[RPC]
	void setTimeout(int toThis)
	{
		timeout = toThis;
		if(timeout <=  10)
		{
			timeout = 10;
		}
		if(timeout >= 60)
		{
			timeout = 60;
		}
	}
	
	[RPC]
	void beginGame()
	{
		PlayerPrefs.SetInt("myNum",plyrNum);
		PlayerPrefs.SetInt("connectedPlayers",names.Count);
		PlayerPrefs.SetString("currentCall",callSign);
		PlayerPrefs.SetInt("currentTeam",currentColor);
		PlayerPrefs.SetInt("numTowers",numTowers);
		PlayerPrefs.SetInt("timeout",timeout);
		
		if(isHost)
		{
			PlayerPrefs.SetInt("numAIs",aiNames.Count);
			for(int x = 0; x < aiNames.Count; x++)
			{
				PlayerPrefs.SetString("ai"+x+"name",(string)aiNames[x]);
				PlayerPrefs.SetInt("ai"+x+"team",(int)aiColors[x]);
			}
		}
		
		Application.LoadLevel(1);
	}
}
