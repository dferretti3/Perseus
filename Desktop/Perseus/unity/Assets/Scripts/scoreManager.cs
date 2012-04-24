using UnityEngine;
using System.Collections;

public class scoreManager : MonoBehaviour {
	
	public ArrayList playerNames;
	public ArrayList aiNames;
	
	public ArrayList teams;
	public ArrayList aiTeams;
	
	public ArrayList playerTurretTransforms;
	public ArrayList aiTurretTransforms;
	
	public ArrayList playerTurrets;
	public ArrayList aiTurrets;
	
	public Texture2D coloredLine;
	
	private bool isVisible = false;
	private bool gameDone = false;
	private bool hasStarted = false;
	private bool countdownStarted = false;
	private float winAfter = -999f;
	
	void OnGUI()
	{
		if(false)//gameDone)
		{
			GUI.Button(new Rect(50,50,Screen.width-100,Screen.height-100),"");
			
			//GUI.Label(
		}
		else if(isVisible)
		{
			GUI.Button(new Rect(50,50,Screen.width-100,Screen.height-100),"");
			
			int currentRow = 0;
			
			for(int x = 0; x < PlayerManagerTestExpansion.teams.Length; x++)
			{
				if(teamHasPlayer(x))
				{
					GUI.color = PlayerManagerTestExpansion.teams[x];
					GUI.DrawTexture(new Rect(70,70 + (currentRow * 25),Screen.width - 140,20),coloredLine);
					GUI.color = Color.white;
					currentRow++;
					for(int y = 0; y < teams.Count; y++)
					{
						if((int)teams[y] == x)
						{
							GUI.Label(new Rect(70,70 + (currentRow *25),100,20),(string)playerNames[y]);
							int health = 0;
							if(playerTurrets.Count > y)
							{
								topLevelController tlC = (topLevelController)(playerTurrets[y]);
								health = tlC.getHealth();
							}
							if(health>0)
							{
								if(health >= 66)
								{
									GUI.color = Color.green;
								}
								else if(health >= 33)
								{
									GUI.color = Color.yellow;
								}
								else
								{
									GUI.color = Color.red;
								}
								GUI.DrawTexture(new Rect(170,70 + (currentRow *25),health,20),coloredLine);
								GUI.color = Color.white;
							}
							currentRow++;
						}
					}
					
					for(int y = 0; y < aiTeams.Count; y++)
					{
						if((int)aiTeams[y] == x)
						{
							GUI.Label(new Rect(70,70 + (currentRow *25),100,20),(string)aiNames[y]);
							int health = 0;
							if(aiTurrets.Count > y)
							{
								AIControl aiC = (AIControl)(aiTurrets[y]);
								health = aiC.getHealth();
							}
							if(health>0)
							{
								if(health >= 66)
								{
									GUI.color = Color.green;
								}
								else if(health >= 33)
								{
									GUI.color = Color.yellow;
								}
								else
								{
									GUI.color = Color.red;
								}
								GUI.DrawTexture(new Rect(170,70 + (currentRow *25),health,20),coloredLine);
								GUI.color = Color.white;
							}
							currentRow++;
						}
					}
				}
			}
			//draw stuff
		}
	}
	
	bool teamHasPlayer(int teamNum)
	{
		for(int x = 0; x < teams.Count; x++)
		{
			if((int)teams[x] == teamNum)
			{
				return true;
			}
		}
		
		for(int x = 0; x < aiTeams.Count; x++)
		{
			if((int)aiTeams[x] == teamNum)
			{
				return true;
			}
		}
		
		return false;
	}
	
	bool teamHasLivePlayer(int teamNum)
	{
		for(int x = 0; x < teams.Count; x++)
		{
			if((int)teams[x] == teamNum)
			{
				topLevelController tlC = (topLevelController)playerTurrets[x];
				if(tlC != null && tlC.getHealth() > 0)
				{
					return true;
				}
			}
		}
		
		for(int x = 0; x < aiTeams.Count; x++)
		{
			if((int)aiTeams[x] == teamNum)
			{
				AIControl aiC = (AIControl)aiTurrets[x];
				if(aiC != null && aiC.getHealth() > 0)
				{
					return true;
				}
			}
		}
		
		return false;
	}
	
	// Use this for initialization
	void Start () {
		playerNames = new ArrayList();
		aiNames = new ArrayList();
		
		teams = new ArrayList();
		aiTeams = new ArrayList();
		
		playerTurretTransforms = new ArrayList();
		aiTurretTransforms = new ArrayList();
		
		playerTurrets = new ArrayList();
		aiTurrets = new ArrayList();
	}
	
	// Update is called once per frame
	void Update () {
		if(1 != 1)
		{
			Debug.Log("..... really????");
		}
		
		if(countdownStarted && winAfter > 0)
		{
			winAfter -= Time.deltaTime;
			if(winAfter <= 0)
			{
				hasStarted = true;
			}
		}
		
		if(hasStarted)
		{
			int living = 0;
			int winningteam = -1;
			for(int x = 0; x < PlayerManagerTestExpansion.teams.Length; x++)
			{
				if(teamHasLivePlayer(x))
				{
					winningteam = x;
					living++;
				}
			}
			if(living < 2)
			{
				if(PlayerPrefs.GetInt("currentTeam") == winningteam)
				{
					PlayerPrefs.SetInt("win",1);
				}
				else
				{
					PlayerPrefs.SetInt("win",0);
				}
				Application.LoadLevel(3);
			}
			
			if(Input.GetKey(KeyCode.LeftShift))
			{
				isVisible = true;
			}
			else
			{
				isVisible = false;
			}
		}
	}
	
	public void addPlayerTower(GameObject turret,int team, string name)
	{
		if(!countdownStarted)
		{
			countdownStarted = true;
			winAfter = 10f;
		}
		networkView.RPC("pushPlayerTurretAdd",RPCMode.Others,turret.networkView.viewID,team,name);
		playerTurretTransforms.Add(turret);
		playerTurrets.Add(turret.GetComponentInChildren<topLevelController>());
		playerNames.Add(name);
		teams.Add(team);
	}
	
	public void addAITower(GameObject turret, int team, string name)
	{
		if(!countdownStarted)
		{
			countdownStarted = true;
			winAfter = 10f;
		}
		networkView.RPC("pushAITurretAdd",RPCMode.Others,turret.networkView.viewID,team,name);
		aiTurretTransforms.Add(turret);
		aiTurrets.Add(turret.GetComponent<AIControl>());
		aiNames.Add(name);
		aiTeams.Add(team);
	}
	
	public int highestTeam()
	{
		int[] healths = new int[PlayerManagerTestExpansion.teams.Length];
		for(int x = 0; x < healths.Length; x++)
		{
			healths[x] = 0;
		}
		
		for(int x = 0; x < playerTurrets.Count; x++)
		{
			if(playerTurrets[x] != null)
			{
				topLevelController tlC = (topLevelController)(playerTurrets[x]);
				healths[(int)teams[x]] += tlC.getHealth();
			}
		}
		for(int x = 0; x < aiTurrets.Count; x++)
		{
			if(aiTurrets[x] != null)
			{
				AIControl aiC = (AIControl)(aiTurrets[x]);
				healths[(int)aiTeams[x]] += aiC.getHealth();
			}
		}
		
		int toRet = 0;
		int max = 0;
		for(int x = 0; x < healths.Length; x++)
		{
			if(healths[x] > max)
			{
				toRet = x;
				max = healths[x];
			}
		}
		
		return toRet;
	}
	
	public int highestTeamNot(int teamNum)
	{
		int[] healths = new int[PlayerManagerTestExpansion.teams.Length];
		for(int x = 0; x < healths.Length; x++)
		{
			healths[x] = 0;
		}
		
		for(int x = 0; x < playerTurrets.Count; x++)
		{
			if(playerTurrets[x] != null)
			{
				topLevelController tlC = (topLevelController)(playerTurrets[x]);
				healths[(int)teams[x]] += tlC.getHealth();
			}
		}
		for(int x = 0; x < aiTurrets.Count; x++)
		{
			if(aiTurrets[x] != null)
			{
				AIControl aiC = (AIControl)(aiTurrets[x]);
				healths[(int)aiTeams[x]] += aiC.getHealth();
			}
		}
		
		int toRet = 0;
		int max = 0;
		for(int x = 0; x < healths.Length; x++)
		{
			if(healths[x] > max && x != teamNum)
			{
				toRet = x;
				max = healths[x];
			}
		}
		
		return toRet;
	}
	
	public GameObject randomTargetFromTeam(int team)
	{
		ArrayList possibleTargets = new ArrayList();
		
		for(int x = 0; x < teams.Count; x++)
		{
			if((int)(teams[x]) == team && playerTurretTransforms[x] != null)
			{
				possibleTargets.Add(playerTurretTransforms[x]);
			}
		}
		
		for(int x = 0; x < aiTeams.Count; x++)
		{
			if((int)(aiTeams[x]) == team && aiTurretTransforms[x] != null)
			{
				possibleTargets.Add(aiTurretTransforms[x]);
			}
		}
		
		return (GameObject)possibleTargets[Mathf.FloorToInt(Random.value*possibleTargets.Count)];
	}
	
	public GameObject highestTargetFromTeam(int team)
	{
		GameObject currentHighest = null;
		int highestHealth = 0;
		
		
		for(int x = 0; x < teams.Count; x++)
		{
			if((int)(teams[x]) == team && playerTurretTransforms[x] != null)
			{
				topLevelController tlC = (topLevelController)(playerTurrets[x]);
				int tempHealth = tlC.getHealth();
				if(highestHealth < tempHealth)
				{
					highestHealth = tempHealth;
					currentHighest = (GameObject)playerTurretTransforms[x];
				}
			}
		}
		
		for(int x = 0; x < aiTeams.Count; x++)
		{
			if((int)(aiTeams[x]) == team && aiTurretTransforms[x] != null)
			{
				AIControl aiC = (AIControl)(aiTurrets[x]);
				int tempHealth = aiC.getHealth();
				if(highestHealth < tempHealth)
				{
					highestHealth = tempHealth;
					currentHighest = (GameObject)aiTurretTransforms[x];
				}
			}
		}
		return currentHighest;
	}
	
	[RPC]
	void pushPlayerTurretAdd(NetworkViewID nvI, int team, string name)
	{
		GameObject t = NetworkView.Find(nvI).gameObject;
		playerTurretTransforms.Add(t);
		playerTurrets.Add(t.GetComponentInChildren<topLevelController>());
		teams.Add(team);
		playerNames.Add(name);
	}
	
	[RPC]
	void pushAITurretAdd(NetworkViewID nvI, int team, string name)
	{
		GameObject turret = NetworkView.Find(nvI).gameObject;
		aiTurretTransforms.Add(turret);
		aiTurrets.Add(turret.GetComponent<AIControl>());
		aiNames.Add(name);
		aiTeams.Add(team);
	}
}
