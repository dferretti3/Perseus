using UnityEngine;
using System.Collections;

public class resourceRobot : MonoBehaviour
{
	private GameObject moneyTarget;
	private Vector3 tempTarget;
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(moneyTarget == null)
		{
			Collider[] coins = Physics.OverlapSphere(transform.position,100f,1<<11);
			int foundAt = -1;
			float minDist = 200;
			for(int x = 0; x < coins.Length; x++)
			{
				Vector3 toMoneyVec = coins[x].transform.position - transform.position;
				if(toMoneyVec.magnitude < minDist)
				{
					Ray toMoney = new Ray(transform.position,toMoneyVec);
					RaycastHit outHit;
					if(Physics.Raycast(toMoney,out outHit,toMoneyVec.magnitude,1<<11))
					{
						minDist = toMoneyVec.magnitude;
						foundAt = x;
					}
				}
			}
			if(foundAt > -1)
			{
				moneyTarget = coins[foundAt].gameObject;
			}
		}
		
		if(tempTarget == null)
		{
			float x = Random.value*600f - 300f;
			float z = Random.value*600f - 300f;
			float y = Terrain.activeTerrain.SampleHeight(new Vector3(x,0,z));
			tempTarget = new Vector3(x,y + 5 + Random.value*15,z);
		}
		
		Vector3 currentTarget;
		if(moneyTarget != null)
		{
			currentTarget = moneyTarget.transform.position;
		}
		else
		{
			currentTarget = tempTarget;
		}
		
		Transform toMove = transform;
					Vector3 realTargetPos = currentTarget;
					float bigDistance = (realTargetPos - transform.position).magnitude;
					bool blocked = true;
					float offset = 0;
					while(blocked && offset < 100)
					{
						offset += 10;
						blocked = false;
					Vector3 toTarget = realTargetPos - toMove.position;
					Ray rToTarget = new Ray (toMove.position, toTarget);
					RaycastHit hitInfo;
						float rayDist = toTarget.magnitude + 3f;
						if(rayDist > 50)
						{
							rayDist = 50f;
						}
					if (Physics.SphereCast (rToTarget, 10, out hitInfo, rayDist, 1 << 8)) {
						blocked = true;
						RaycastHit LeftHitInfo, RightHitInfo, UpHitInfo, DownHitInfo;
						Debug.Log("spherecast hit!");
						Ray leftTarget, rightTarget, upTarget, downTarget;
						int direction = 0;
						float maxHitDist = 9000;
						leftTarget = new Ray (toMove.transform.position - Vector3.Cross (toTarget.normalized, Vector3.up) * 5, toTarget);
						rightTarget = new Ray (toMove.transform.position + Vector3.Cross (toTarget.normalized, Vector3.up) * 5, toTarget);
						upTarget = new Ray (toMove.transform.position + new Vector3 (0, 5, 0), toTarget);
						downTarget = new Ray (toMove.transform.position - new Vector3 (0, 3, 0), toTarget);
					
						if (Physics.Raycast (leftTarget, out LeftHitInfo, bigDistance, 1 << 8)) {
							maxHitDist = LeftHitInfo.distance;
							//Debug.DrawRay(leftTarget.origin,leftTarget.direction,Color.red,.5);
							if (Physics.Raycast (rightTarget, out RightHitInfo, bigDistance, 1 << 8)) {
								if (maxHitDist < RightHitInfo.distance) {
									direction = 1;
									maxHitDist = RightHitInfo.distance;
								}
							
								if (Physics.Raycast (upTarget, out UpHitInfo, bigDistance, 1 << 8)) {
									if (maxHitDist < UpHitInfo.distance) {
										direction = 2;
										maxHitDist = UpHitInfo.distance;
									}
								
									if (Physics.Raycast (downTarget, out DownHitInfo, bigDistance, 1 << 8)) {
										if (maxHitDist < DownHitInfo.distance) {
											direction = 3;
											maxHitDist = DownHitInfo.distance;
										}
									
									
										if (direction == 0) {
											realTargetPos = hitInfo.point + Vector3.Cross (toTarget.normalized, Vector3.up) * offset;
										} else if (direction == 1) {
											realTargetPos = hitInfo.point + Vector3.Cross (toTarget.normalized, Vector3.up) * offset;
										} else if (direction == 2) {
											realTargetPos = hitInfo.point + new Vector3 (0, offset, 0);
										} else if (direction == 3) {
											realTargetPos = hitInfo.point - new Vector3 (0, offset, 0);
										}
									
									
									} else {
										realTargetPos = hitInfo.point - new Vector3 (0, offset, 0);
									}
								
								} else {
									realTargetPos = hitInfo.point + new Vector3 (0, offset, 0);
								}
							
							} else {
								realTargetPos = hitInfo.point + Vector3.Cross (toTarget.normalized, Vector3.up) * offset;
							}
						} else {
							realTargetPos = hitInfo.point - Vector3.Cross (toTarget.normalized, Vector3.up) * offset;
						}
					
					}
					else{
						Debug.Log("Spherecast not hit");
					}
					}
					Debug.DrawLine(transform.position,realTargetPos,Color.red,.3f);
					transform.forward = Vector3.Slerp (transform.forward, realTargetPos - transform.position, Time.deltaTime*.1f);
		transform.position = transform.position + (realTargetPos - transform.position)* 10 * Time.deltaTime;
		
	
	}
}

