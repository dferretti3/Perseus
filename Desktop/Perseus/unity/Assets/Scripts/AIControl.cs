using UnityEngine;
using System.Collections;

public class AIControl : MonoBehaviour {
	public AIControlledMissile missile;
	public Bullet b;
	AIControlledMissile m;
	public AIresourceRobot rrobot;
	AIresourceRobot rr;
	
	public int team = -1;
	string callSign = "";
	
	float income = 0.33f;
	int count;
	int lead;
	int spread;
	GameObject target;
	GameObject missiletarget;
	private int health = 100;
	public float money;
	Vector3 randomangle;
	bool slerping = false;
	int slerpcount = 0;
	
	float waitTime;
	bool saving = false;
	private scoreManager sm;
	// Use this for initialization
	void Start () {
		money = 20;
		waitTime = Random.Range(5,30);
		sm = GameObject.FindGameObjectWithTag("scoreManagerTag").GetComponent<scoreManager>();
	}
	
	public void pushNavPointInfo(int teamNum, string call)
	{
		team = teamNum;
		callSign = call;
		
		navPoint np = GetComponentInChildren<navPoint>();
		np.playerColor = PlayerManagerTestExpansion.teams[teamNum];
		np.nameTag = call;
		np.refresh();
	}
	
	public int getHealth()
	{
		return health;
	}
	
	// Update is called once per frame
	void Update () {
		if(ownedByCurrentPlayer())
		{
			count++;
			money += Time.deltaTime*income;
			waitTime -= Time.deltaTime;
			
			Collider[] cols = Physics.OverlapSphere(transform.position, 200);
				foreach (Collider hit in cols){
					if(hit.gameObject.name == "AIContMissile(Clone)" || hit.gameObject.name == "ControlledMissile(Clone)" || hit.gameObject.name == "homingMissileRedo(Clone)" || 
						hit.gameObject.name == "Bomb(Clone)")
					{
						if(hit.tag.CompareTo(""+team)!=0){
						target = hit.gameObject;
						break;
						}
					}	
				}
			if(rr==null && money<=30)
			{
				income = 0.33f;
				saving=true;
			}
			
			
			if(rr==null && money>=30)
			{
				rr = (AIresourceRobot)Network.Instantiate(rrobot, transform.position+transform.up*8, Quaternion.identity,0);
				rr.pushNavPointInfo(team, callSign);
				saving=false;
				money += -30;
				income = 0.66f;
			}
			
					
			if(waitTime <= 0)
			{
				missiletarget = sm.randomTargetFromTeam(sm.highestTeamNot(team));
				if(GameObject.Find("turrettSystem(Clone)")!=null && money >= 10 && m==null && !slerping && target==null)
				{
					randomangle = new Vector3(Random.Range(0,360), 120, 0);
					slerping = true;
				}
				
				transform.forward = Vector3.Slerp (transform.forward, randomangle, Time.deltaTime);
	
				if(slerping)
					slerpcount++;
	
				if(slerpcount>=50)
				{
					m = (AIControlledMissile) Network.Instantiate(missile, transform.position+transform.forward*20, transform.rotation,0);
					m.init();
					m.giveTarget(missiletarget);
					m.tag = ""+team;
					m.pushNavPointInfo(team, callSign);
					
					money -= 10;
					slerpcount=0;
					slerping=false;
					if(rr==null)
						waitTime = Random.Range(5,30);

				}						
			}
			if(m==null)
			{
				if(target!=null)
				{
					Vector3 distance = transform.position - target.transform.position;
					float d = distance.magnitude;
					print(d);
					if(d>150)
					{
						target=null;
						return;
					}
					if(d<=120)
					{
						lead = 10;
						spread = 5;
					}
					
					//transform.forward = Vector3.Slerp(transform.forward, target.transform.position+target.transform.forward*lead, 0.5f);
					transform.LookAt(target.transform.position);
					if(count%50==0)
					{
						Bullet bul = (Bullet)Network.Instantiate(b, transform.position+transform.forward*10, transform.rotation, 0);
						bul.transform.LookAt(target.transform.position+target.transform.forward*lead-new Vector3(Random.Range(-spread,spread),Random.Range(-spread,spread),Random.Range(-spread,spread)));
					}
				}
			}
		}
	}
	
	[RPC]
	void setNavColor(Vector3 pColor, string nTag)
	{
		GetComponentInChildren<navPoint>().subRefresh(new Color(pColor.x,pColor.y,pColor.z,1f),nTag);
	}
	
	[RPC]
	void hitTower(int damage)
	{
		if(networkView.owner == Network.player)
		{
			health -= damage;
			if(health<=0)
			{
				transform.networkView.RPC("updateHealth",RPCMode.OthersBuffered,0);	
				Network.Destroy(gameObject);
			}
			else
				transform.networkView.RPC("updateHealth",RPCMode.OthersBuffered,health);	

		}
	}
	
	[RPC]
	void updateHealth(int currentHealth)
	{
		health = currentHealth;
	}
	
	private bool ownedByCurrentPlayer ()
	{
		return transform.networkView.viewID.owner == Network.player;
	}
}

