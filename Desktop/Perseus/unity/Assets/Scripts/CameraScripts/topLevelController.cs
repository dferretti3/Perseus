using UnityEngine;
using System.Collections;

public enum missileType
{
	Homing,
	Controlled,
	Static,
	Collector, 
	DefenseSystem,
	Mortar
};

public class topLevelController : MonoBehaviour
{
	
	public Color playerColor;
	public string nameTag;
	public int teamNum = -1;
	private bool coloredParent = false;
	public bool isActive = true;
	public GameObject thirdPerson;
	public thirdPersonTower tPT;
	public GameObject firstPerson;
	public firstPersonTower fPT;
	public GameObject homingMissileObject;
	public homingMissileScript mC;
	public GameObject controlledMissileOjbect;
	public ControlledMissile contMisScript;
	public GameObject ResourceMissile;
	public resourceRobot resourceMissileScript;
	public GameObject DefenseSystem;
	public defensesystem DefenseSystemScript;
	public int currentMissileSelection = 0;
	private int numMissiles = 6;
	private int health = 100;
	public TurrettManager manager;
	
	public int getHealth()
	{
		return health;
	}
	
	// Use this for initialization
	void Start ()
	{
		if (transform.parent.networkView.viewID.owner != Network.player) {
			isActive = false;
		}
		
		tPT = thirdPerson.GetComponent<thirdPersonTower> ();
		fPT = firstPerson.GetComponent<firstPersonTower> ();
		if (tPT != null && !isActive) {
			tPT.shutDownControl ();
		}
		if(fPT != null && !isActive)
		{
			fPT.cleanUpOnExit();
		}
		if (isActive) {
			tPT.shutDownControl ();
			fPT.makeActive ();
		}
	}
	
	public void hitPlayer(int damage)
	{
		health -= damage;
		if(health <= 0)
		{
			transform.parent.networkView.RPC("updateHealth",RPCMode.OthersBuffered,0);
			if(isActive)
			{
				Debug.Log("it is active");
				if(mC != null)
				{
					mC.transferControl();
				}
				if(contMisScript != null)
				{
					contMisScript.transferControl();
				}
				if(resourceMissileScript != null)
				{
					resourceMissileScript.transferControl();
				}
				Debug.Log("about to scroll");
				manager.scroll(1);
				Debug.Log("scrolled");
			}
			Network.Destroy(transform.parent.gameObject);
		}
		else
		{
			transform.parent.networkView.RPC("updateHealth",RPCMode.OthersBuffered,health);
		}
	}
	
	public void setHealth(int currentVal)
	{
		health = currentVal;
	}
	
	private bool ownedByCurrentPlayer ()
	{
		return transform.parent.networkView.viewID.owner == Network.player;
	}
	
	public void scrollMissileSelection (int direction)
	{
		currentMissileSelection += -direction;
		if(currentMissileSelection<0)
			currentMissileSelection=numMissiles-1;
		if(currentMissileSelection>numMissiles-1)
			currentMissileSelection=0;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (ownedByCurrentPlayer ()) {
			if (!coloredParent) {
				navPoint toColor = gameObject.transform.parent.gameObject.GetComponentInChildren<navPoint> ();
				if (toColor != null) {
					toColor.playerColor = playerColor;
					toColor.nameTag = nameTag;
					toColor.playerTeam = teamNum;
					toColor.refresh ();
					coloredParent = true;
				}
			}
		
		
			if (mC == null && homingMissileObject != null) {
				mC = homingMissileObject.GetComponent<homingMissileScript> ();
			}
			if (tPT == null) {
				tPT = thirdPerson.GetComponent<thirdPersonTower> ();
				if (tPT != null && !isActive) {
					tPT.shutDownControl ();
				}
			}
			if (fPT == null) {
				fPT = firstPerson.GetComponent<firstPersonTower> ();
			}
		}
		
		if (Input.GetKeyDown("k"))
		{
			manager.scrollFromTab();	
		}
	}
	
	public void moveToThirdPerson ()
	{
		tPT.makeActive ();
	}
	
	public void moveToFirstPerson ()
	{
		fPT.makeActive ();
	}
	
	public bool moveToMissile (int missileNum)
	{
		if (missileNum == 0 && mC != null) {
			mC.makeActive ();
			return true;
		} else if (missileNum == 1 && contMisScript != null) {
			contMisScript.makeActive ();
			return true;
		}
		else if(missileNum == 3 && resourceMissileScript != null)
		{
			resourceMissileScript.makeActive();
			return true;
		}
		return false;
	}
	
	public void openMiniScreen (int missileNum)
	{
		if (missileNum == 0 && mC != null) {
			mC.openMiniScreen ();
			if(contMisScript != null && contMisScript.isMiniScreenOpen())
			{
				contMisScript.openMiniScreen();
			}
			if(resourceMissileScript != null && resourceMissileScript.isMiniScreenOpen())
			{
				resourceMissileScript.openMiniScreen();
			}
		} else if (missileNum == 1 && contMisScript != null) {
			if(mC != null && mC.isMiniScreenOpen())
			{
				mC.openMiniScreen();
			}
			if(resourceMissileScript != null && resourceMissileScript.isMiniScreenOpen())
			{
				resourceMissileScript.openMiniScreen();
			}
			contMisScript.openMiniScreen ();
		}
		else if(missileNum == 3 && resourceMissileScript != null)
		{
			if(mC != null && mC.isMiniScreenOpen())
			{
				mC.openMiniScreen();
			}
			if(contMisScript != null && contMisScript.isMiniScreenOpen())
			{
				contMisScript.openMiniScreen();
			}
			resourceMissileScript.openMiniScreen();
		}
	}
	
	public bool addNewMissile (GameObject missile, missileType mType)
	{
		switch (mType) {
		case(missileType.Homing):
			if (homingMissileObject != null) {
				return false;
			}
		
			homingMissileObject = missile;
			navPoint homingNav = homingMissileObject.GetComponentInChildren<navPoint> ();
			homingNav.playerColor = playerColor;
			homingNav.nameTag = nameTag;
			homingNav.refresh();
			mC = homingMissileObject.GetComponentInChildren<homingMissileScript> ();
			mC.tLC = this;
			return true;
		case(missileType.Controlled):
			if (controlledMissileOjbect != null) {
				return false;
			}
				
			controlledMissileOjbect = missile;
			navPoint contNav = controlledMissileOjbect.GetComponentInChildren<navPoint> ();
			contNav.playerColor = playerColor;
			contNav.nameTag = nameTag;
			contNav.refresh ();
			contMisScript = missile.GetComponentInChildren<ControlledMissile> ();
			contMisScript.tLC = this;
			return true;
		case(missileType.Collector):
			Debug.Log("Received fire command");
			if(ResourceMissile != null) {
				return false;
			}
			Debug.Log("Setting up AI missile");
			ResourceMissile = missile;
			navPoint rescontNav = ResourceMissile.GetComponentInChildren<navPoint>();
			rescontNav.playerColor = playerColor;
			rescontNav.nameTag = nameTag;
			rescontNav.refresh();
			resourceMissileScript = missile.GetComponentInChildren<resourceRobot>();
			resourceMissileScript.tLC = this;
			Debug.Log("Returning true");
			return true;
		case(missileType.DefenseSystem):
			if(DefenseSystem != null){
				return false;
			}
			DefenseSystem = missile;
			DefenseSystemScript = missile.GetComponentInChildren<defensesystem>();
			DefenseSystemScript.tLC = this;
			return true;
		default:
			return false;
		}
	}
	
	public void setInactive()
	{
		if(mC != null && mC.isMiniScreenOpen())
		{	
			mC.openMiniScreen();
		}
		if(contMisScript != null && contMisScript.isMiniScreenOpen())
		{
			contMisScript.openMiniScreen();
		}
		if(resourceMissileScript != null && resourceMissileScript.isMiniScreenOpen())
		{
			resourceMissileScript.openMiniScreen();
		}
		isActive = false;
		tPT.shutDownControl();
		fPT.cleanUpOnExit();
	}
	
	public void setActive()
	{
		tPT.makeActive();
	}
	
	public void setActive2()
	{
		fPT.makeActive();
	}
}
