using UnityEngine;
using System.Collections;

public enum missileType
{
	Homing,
	Controlled,
	Static,
	AIControlled,
	Collector
};

public class topLevelController : MonoBehaviour
{
	
	public Color playerColor;
	public string nameTag;
	private bool coloredParent = false;
	public bool isActive = true;
	public GameObject thirdPerson;
	public thirdPersonTower tPT;
	public GameObject firstPerson;
	public firstPersonTower fPT;
	public GameObject homingMissileObject;
	public homingMissileCamera mC;
	public GameObject controlledMissileOjbect;
	public ControlledMissile contMisScript;
	public GameObject AIContMissileObject;
	public AIControlledMissile aiContMissileScript;
	public GameObject ResourceMissile;
	public resourceRobot resourceMissileScript;
	public int currentMissileSelection = 0;
	private int numMissiles = 5;
	private int health = 100;
	
	//TODO Arlen: public yourMissileScript 
	/*
	 * Your script will need the following function calls:
	 * 
	 * 		openMiniScreen()  - you can check out homingMissileCamera to see how I open it or I can copy and paste once you add
	 * 		makeActive()   -  this means that the tower is losing camera control and the missile will handle all camera and inputs
	 * 
	 * 
	 * 
	 * 		Your script will need a way to invoke this topLevelController.moveToFirstPerson or third person
	 * 		I would recommend adding a public topLevelController variable so that upon creation we can just assign from here
	 * 
	 * 		We can discuss control schemes for selecting different cameras later as well
	 * 			I've removed my selection so that you can test with yours
	 */
	
	
	
	// Use this for initialization
	void Start ()
	{
		if (transform.parent.networkView.viewID.owner != Network.player) {
			isActive = false;
		}
		
		if (homingMissileObject != null) {
			mC = homingMissileObject.GetComponent<homingMissileCamera> ();
		}
		tPT = thirdPerson.GetComponent<thirdPersonTower> ();
		fPT = firstPerson.GetComponent<firstPersonTower> ();
		if (tPT != null && !isActive) {
			tPT.shutDownControl ();
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
			Network.Destroy(transform.parent.gameObject);
		}
		transform.parent.networkView.RPC("updateHealth",RPCMode.OthersBuffered,health);
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
		currentMissileSelection += direction;
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
					toColor.refresh ();
					coloredParent = true;
				}
			}
		
		
			if (mC == null && homingMissileObject != null) {
				mC = homingMissileObject.GetComponent<homingMissileCamera> ();
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
		else if(missileNum == 3 && aiContMissileScript != null) {
			aiContMissileScript.makeActive();
			return true;
		}
		else if(missileNum == 4 && resourceMissileScript != null)
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
		} else if (missileNum == 1 && contMisScript != null) {
			contMisScript.openMiniScreen ();
		}
		else if (missileNum == 3 && aiContMissileScript != null) {
			aiContMissileScript.openMiniScreen ();
		}
		else if(missileNum == 4 && resourceMissileScript != null)
		{
			resourceMissileScript.openMiniScreen();
		}
	}
	
	public bool moveToMissile ()
	{
		if (contMisScript != null) {
			contMisScript.makeActive ();
			return true;
		}
		return false;
		/*if(mC != null)
		{
			mC.makeActive();
			return true;
		}
		return false;*/
	}
	
	public void openMiniScreen ()
	{
		if (contMisScript != null) {
			contMisScript.openMiniScreen ();
		}
		/*if(mC != null)
		{
			mC.openMiniScreen();
		}*/
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
			mC = homingMissileObject.GetComponentInChildren<homingMissileCamera> ();
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
		case(missileType.AIControlled):
			Debug.Log("Received fire command");
			if(AIContMissileObject != null) {
				return false;
			}
			Debug.Log("Setting up AI missile");
			AIContMissileObject = missile;
			navPoint aicontNav = AIContMissileObject.GetComponentInChildren<navPoint>();
			aicontNav.playerColor = playerColor;
			aicontNav.nameTag = nameTag;
			aicontNav.refresh();
			aiContMissileScript = missile.GetComponentInChildren<AIControlledMissile>();
			aiContMissileScript.tLC = this;
			Debug.Log("Returning true");
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
		default:
			return false;
		}
	}
}
