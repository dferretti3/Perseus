using UnityEngine;
using System.Collections;

public enum missileType {Homing, Controlled, Static};

public class topLevelController : MonoBehaviour {
	
	public bool isActive = true;
	public GameObject thirdPerson;
	public thirdPersonTower tPT;
	public GameObject firstPerson;
	public firstPersonTower fPT;
	
	
	public GameObject homingMissileObject;
	public homingMissileCamera mC;
	
	public GameObject controlledMissileOjbect;
	public ControlledMissile contMisScript;
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
	void Start () {
		if(homingMissileObject != null)
		{
			mC = homingMissileObject.GetComponent<homingMissileCamera>();
		}
		tPT = thirdPerson.GetComponent<thirdPersonTower>();
		fPT = firstPerson.GetComponent<firstPersonTower>();
		if(tPT != null && !isActive)
		{
			tPT.shutDownControl();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(mC == null && homingMissileObject != null)
		{
			mC = homingMissileObject.GetComponent<homingMissileCamera>();
		}
		if(tPT == null)
		{
			tPT = thirdPerson.GetComponent<thirdPersonTower>();
			if(tPT != null && !isActive)
			{
				tPT.shutDownControl();
			}
		}
		if(fPT == null)
		{
			fPT = firstPerson.GetComponent<firstPersonTower>();
		}
	}
	
	public void moveToThirdPerson()
	{
		tPT.makeActive();
	}
	
	public void moveToFirstPerson()
	{
		fPT.makeActive();
	}
	
	public bool moveToMissile(int missileNum)
	{
		if(missileNum == 0 && mC != null)
		{
			mC.makeActive();
			return true;
		}
		else if(missileNum == 1 && contMisScript != null)
		{
			contMisScript.makeActive();
			return true;
		}
		return false;
	}
	
	public void openMiniScreen(int missileNum)
	{
		if(missileNum == 0 && mC != null)
		{
			mC.openMiniScreen();
		}
		else if(missileNum == 1 && contMisScript != null)
		{
			contMisScript.openMiniScreen();
		}
	}
	
	public bool moveToMissile()
	{
		if(contMisScript != null)
		{
			contMisScript.makeActive();
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
	
	public void openMiniScreen()
	{
		if(contMisScript != null)
		{
			contMisScript.openMiniScreen();
		}
		/*if(mC != null)
		{
			mC.openMiniScreen();
		}*/
	}
	
	public bool addNewMissile(GameObject missile,missileType mType)
	{
		switch(mType)
		{
			case(missileType.Homing):
				if(homingMissileObject != null)
				{
					return false;
				}
		
				homingMissileObject = missile;
				mC = homingMissileObject.GetComponentInChildren<homingMissileCamera>();
				mC.tLC = this;
				return true;
			case(missileType.Controlled):
				if(controlledMissileOjbect != null)
				{
					return false;
				}
				
				controlledMissileOjbect = missile;
				//TODO Arlen:  replace this getcomponent with the correct information
				contMisScript = missile.GetComponentInChildren<ControlledMissile>();
				contMisScript.tLC = this;
				return true;
			default:
			return false;
		}
	}
}
