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
	
	private bool isVisible = false;
	
	void OnGUI()
	{
		if(isVisible)
		{
			//draw stuff
		}
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
	}
	
	public void showCurrentStats()
	{
		
	}
	
	public void hideCurrentStats()
	{
		
	}
	
	public void addPlayerTower(GameObject turret,int team, string name)
	{
		networkView.RPC("pushPlayerTurretAdd",RPCMode.Others,turret.networkView.viewID,team,name);
		playerTurretTransforms.Add(turret);
		playerTurrets.Add(turret.GetComponentInChildren<topLevelController>());
		playerNames.Add(name);
		teams.Add(team);
	}
	
	public void addAITower(GameObject turret, int team, string name)
	{
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
