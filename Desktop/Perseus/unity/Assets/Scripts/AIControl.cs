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
	
	int numfired = 0;
	int numbeforesave;
	bool saving = false;
	private scoreManager sm;
	// Use this for initialization
	void Start () {
		money = 20;
		numbeforesave = Random.Range(0,3);
		sm = GameObject.FindGameObjectWithTag("scoreManagerTag").GetComponent<scoreManager>();
	}
	
	public void pushNavPointInfo(int teamNum, string call)
	{
		team = teamNum;
		callSign = call;
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
			money += Time.deltaTime*0.33f;
			
			
			Collider[] cols = Physics.OverlapSphere(transform.position, 200);
				foreach (Collider hit in cols){
					if(hit.gameObject.name == "AIContMissile(Clone)" || hit.gameObject.name == "ControlledMissile(Clone)" || hit.gameObject.name == "homingMissileRedo(Clone)" || 
						hit.gameObject.name == "Bomb(Clone)")
					{
						Debug.Log(hit.tag + "AISEE");
						if(hit.tag.CompareTo(""+team)!=0){
						target = hit.gameObject;
						break;
						}
					}	
				}
			if(rr==null && money<=30 && numfired==numbeforesave)
			{
				saving=true;
			}
			
			
			if(rr==null && money>=30)
			{
				rr = (AIresourceRobot)Network.Instantiate(rrobot, transform.position+transform.up*8, Quaternion.identity,0);
				saving=false;
				money += -30;
			}
			
					
			if(!saving)
			{
				missiletarget = sm.randomTargetFromTeam(sm.highestTeamNot(team));
				if(GameObject.Find("turrettSystem(Clone)")!=null && money >= 10 && m==null && !slerping && target==null)
				{
					randomangle = new Vector3(Random.Range(0,360), 50, 0);
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
					money -= 10;
					slerpcount=0;
					slerping=false;
					if(rr==null)
						numfired++;
					else
						numfired=0;
				}						
			}
			if(m==null)
			{
				if(target!=null)
				{
					Vector3 distance = transform.position - target.transform.position;
					float d = distance.sqrMagnitude;
					if(d<=20000)
					{
						lead = 10;
						spread = 5;
					}
					if(d>20000)
					{
						lead = 40;
						spread = 8;
					}
					
					//transform.forward = Vector3.Slerp(transform.forward, target.transform.position+target.transform.forward*lead, 0.5f);
					transform.LookAt(target.transform.position);
					if(count%40==0)
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

