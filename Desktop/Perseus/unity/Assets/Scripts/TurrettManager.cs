using UnityEngine;
using System.Collections;

public class TurrettManager {
	
	int max = PlayerManager.MAX_TOWERS;
	topLevelController[] conts;
	int current_index;
	
	public TurrettManager(GameObject towerPrefab,Vector3[] p, Vector3[] n, Quaternion[] r, Color c, string tag)
	{
		float d = 4;
		conts = new topLevelController[max];
		for (int i = 0; i < max; i++)
		{
			GameObject tower = (GameObject)Network.Instantiate(towerPrefab,p[i]+d*n[i],r[i],0);
			topLevelController cont = tower.GetComponentInChildren<topLevelController>();
			cont.manager = this;
			cont.playerColor = c;
			cont.nameTag = tag;
			cont.isActive = false;
			conts[i] = cont;
		}
		conts[current_index=max-1].isActive = true;
	}
	
	public void scroll(int scroll)
	{
		if (scroll%max==0) return;
		conts[current_index].setInactive();
		Debug.Log("setting "+current_index+" inactive");
		current_index = (current_index+scroll)%max;
		current_index = (current_index+max)%max;//account for negative modulus
		conts[current_index].setActive();
		Debug.Log("setting "+current_index+" active");
	}
}
