using UnityEngine;
using System.Collections;

public class TurrettManager {
	
	int max = PlayerManagerTestExpansion.MAX_TOWERS;
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
		int y = 0;
		int x = 0;
		for(x = 0; x < conts.Length; x++)
		{
			if(conts[x] != null)
			{
				y++;
			}
		}
		if(y < x)
		{
			topLevelController[] tempArray = conts;
			conts = new  topLevelController[y];
			y = 0;
			for(x = 0; x < conts.Length; x++)
			{
				if(tempArray[x] != null)
				{
					conts[y] = tempArray[x];
					y++;
				}
			}
			max = y;
		}
		if (scroll%max==0) return;
		if(conts[current_index] != null)
		{
			conts[current_index].setInactive();
		}
		Debug.Log("setting "+current_index+" inactive");
		current_index = (current_index+scroll)%max;
		current_index = (current_index+max)%max;//account for negative modulus
		int h = 0;
		while(conts[current_index] == null && h < max)
		{
			current_index++;
			current_index = (current_index)%max;
			h++;
		}
		if(conts[current_index] != null)
		{
			conts[current_index].setActive();
		}
		Debug.Log("setting "+current_index+" active");
	}
	
	public void scrollFromTab()
	{
		int scroll = 1;
		int y = 0;
		int x = 0;
		for(x = 0; x < conts.Length; x++)
		{
			if(conts[x] != null)
			{
				y++;
			}
		}
		if(y < x)
		{
			topLevelController[] tempArray = conts;
			conts = new  topLevelController[y];
			y = 0;
			for(x = 0; x < conts.Length; x++)
			{
				if(tempArray[x] != null)
				{
					conts[y] = tempArray[x];
					y++;
				}
			}
			max = y;
		}
		if (scroll%max==0) return;
		if(conts[current_index] != null)
		{
			conts[current_index].setInactive();
		}
		Debug.Log("setting "+current_index+" inactive");
		current_index = (current_index+scroll)%max;
		current_index = (current_index+max)%max;//account for negative modulus
		int h = 0;
		while(conts[current_index] == null && h < max)
		{
			current_index++;
			current_index = (current_index)%max;
			h++;
		}
		if(conts[current_index] != null)
		{
			conts[current_index].setActive2();
		}
		Debug.Log("setting "+current_index+" active");
	}
}
