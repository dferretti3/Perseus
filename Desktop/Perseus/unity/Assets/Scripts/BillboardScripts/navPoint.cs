using UnityEngine;
using System.Collections;

public class navPoint : MonoBehaviour
{

	// Use this for initialization
	Vector3 localPos;
	public Color playerColor;
	public string nameTag;
	public int playerTeam;
	public GameObject nameDisplayPref;
	private GameObject nameDisplay;
	
	public GameObject navPref;
	private GameObject navPrefFree;

	void Start ()
	{
		if(nameDisplay != null)
		{
			Destroy(nameDisplay);
		}
		localPos = transform.localPosition;
		renderer.material.SetColor("_TintColor",playerColor);
		nameDisplay = (GameObject)Instantiate(nameDisplayPref);
		nameDisplay.transform.position = transform.position;
		TextMesh displayMesh = nameDisplay.GetComponent<TextMesh>();
		displayMesh.text = nameTag;
		displayMesh.renderer.material.color = playerColor;
	}
	
	void OnDisable()
	{
		Destroy(nameDisplay);
		Destroy(navPrefFree);
		
	}
	
	public void refresh()
	{
		if(navPrefFree != null)
		{
			Destroy(navPrefFree);
		}
		Destroy(nameDisplay);
		renderer.material.SetColor("_TintColor",playerColor);
		nameDisplay = (GameObject)Instantiate(nameDisplayPref);
		nameDisplay.transform.position = transform.position;
		TextMesh displayMesh = nameDisplay.GetComponent<TextMesh>();
		displayMesh.text = nameTag;
		displayMesh.renderer.material.color = playerColor;
		navPrefFree = (GameObject)GameObject.Instantiate(navPref);
		navPrefFree.renderer.material.SetColor("_TintColor",playerColor);
		//Debug.Log("Sending RPC to change nav color...");
		transform.parent.networkView.RPC("setNavColor",RPCMode.OthersBuffered,new Vector3(playerColor.r,playerColor.g,playerColor.b),nameTag);
	}
	
	public void subRefresh(Color pCol, string nTag)
	{
		if(navPrefFree != null)
		{
			Destroy(navPrefFree);
		}
		//Debug.Log("Attempting to update a networked nav point...");
		playerColor = pCol;
		nameTag = nTag;
		Destroy(nameDisplay);
		renderer.material.SetColor("_TintColor",playerColor);
		nameDisplay = (GameObject)Instantiate(nameDisplayPref);
		if(nameDisplay == null)
		{
			Debug.Log("Name display == null here....");
			//nameDisplay = (GameObject)Instantiate(nameDisplayPref);
		}
		nameDisplay.transform.position = transform.position;
		TextMesh displayMesh = nameDisplay.GetComponent<TextMesh>();
		displayMesh.text = nameTag;
		displayMesh.renderer.material.color = playerColor;
		navPrefFree = (GameObject)GameObject.Instantiate(navPref);
		navPrefFree.renderer.material.SetColor("_TintColor",playerColor);
	}


	void OnWillRenderObject ()
	{
		if(navPrefFree == null)
		{
			return;
		}
		transform.localScale = Vector3.zero;
		Vector3 vectorTo = transform.position - Camera.current.transform.position;
		float depth = Vector3.Cross(vectorTo,Camera.current.transform.forward).magnitude;
		if ((vectorTo).magnitude < 25) {
			transform.localScale = Vector3.zero;
			nameDisplay.transform.localScale = Vector3.zero;
			navPrefFree.transform.localScale = Vector3.zero;
			return;
		}
		//Debug.Log("Camera: " + Camera.current.name + " OnWillRenderObject()");
		//transform.position = Camera.current.NormalizedViewportToWorldPoint(pos);
		navPrefFree.transform.localScale = ((transform.position - Camera.current.transform.position).magnitude* new Vector3 (.003f, .0f, .003f));/*(transform.position - Camera.current.transform.position).magnitude)*/
		Vector3 fwd = Camera.current.transform.up;
		
		//then calc the player's new rotation quat
		
		//transform.localPosition = localPos;
		navPrefFree.transform.position = transform.position + Camera.current.transform.up * (transform.renderer.bounds.size.y + 2f);
		navPrefFree.transform.rotation = Quaternion.LookRotation (fwd, Camera.current.transform.forward);//vectorTo.normalized);
		
		Vector3 screenPos = Camera.current.WorldToNormalizedViewportPoint(transform.parent.position);
		Vector2 xY = new Vector2(screenPos.x - .5f,screenPos.y - .5f);
		if(xY.magnitude < .1)
		{
			nameDisplay.transform.localScale = ((transform.position - Camera.current.transform.position).magnitude) * new Vector3 (.01f, .01f, .01f);
			nameDisplay.transform.position = navPrefFree.transform.position + Camera.current.transform.up * nameDisplay.transform.renderer.bounds.size.y;
			nameDisplay.transform.rotation = Quaternion.LookRotation(Camera.current.transform.forward,Camera.current.transform.up);
		}
		else
		{
			nameDisplay.transform.localScale = Vector3.zero;
		}
		
		//Camera.current.Render();
	}

	// Update is called once per frame
	void Update ()
	{
		
	}
}



public static class CameraExtensions
{
	/// [summary]
	/// The resulting value of z' is normalized between the values of -1 and 1, 
	/// where the near plane is at -1 and the far plane is at 1. Values outside of 
	/// this range correspond to points which are not in the viewing frustum, and 
	/// shouldn't be rendered.
	/// 
	/// See: http://en.wikipedia.org/wiki/Z-buffering
	/// [/summary]
	/// [param name="camera"]
	/// The camera to use for conversion.
	/// [/param]
	/// [param name="point"]
	/// The point to convert.
	/// [/param]
	/// [returns]
	/// A world point converted to view space and normalized to values between -1 and 1.
	/// [/returns]
	public static Vector3 WorldToNormalizedViewportPoint (this Camera camera, Vector3 point)
	{
		// Use the default camera matrix to normalize XY, 
		// but Z will be distance from the camera in world units
		point = camera.WorldToViewportPoint (point);
		
		if (camera.isOrthoGraphic) {
			// Convert world units into a normalized Z depth value
			// based on orthographic projection
			point.z = (2 * (point.z - camera.nearClipPlane) / (camera.farClipPlane - camera.nearClipPlane)) - 1f;
		} else {
			// Convert world units into a normalized Z depth value
			// based on perspective projection
			point.z = ((camera.farClipPlane + camera.nearClipPlane) / (camera.farClipPlane - camera.nearClipPlane)) + (1 / point.z) * (-2 * camera.farClipPlane * camera.nearClipPlane / (camera.farClipPlane - camera.nearClipPlane));
		}
		
		return point;
	}

	/// [summary]
	/// Takes as input a normalized viewport point with values between -1 and 1,
	/// and outputs a point in world space according to the given camera.
	/// [/summary]
	/// [param name="camera"]
	/// The camera to use for conversion.
	/// [/param]
	/// [param name="point"]
	/// The point to convert.
	/// [/param]
	/// [returns]
	/// A normalized viewport point converted to world space according to the given camera.
	/// [/returns]
	public static Vector3 NormalizedViewportToWorldPoint (this Camera camera, Vector3 point)
	{
		if (camera.isOrthoGraphic) {
			// Convert normalized Z depth value into world units
			// based on orthographic projection
			point.z = (point.z + 1f) * (camera.farClipPlane - camera.nearClipPlane) * 0.5f + camera.nearClipPlane;
		} else {
			// Convert normalized Z depth value into world units
			// based on perspective projection
			point.z = ((-2 * camera.farClipPlane * camera.nearClipPlane) / (camera.farClipPlane - camera.nearClipPlane)) / (point.z - ((camera.farClipPlane + camera.nearClipPlane) / (camera.farClipPlane - camera.nearClipPlane)));
		}
		
		// Use the default camera matrix which expects normalized XY but world unit Z 
		return camera.ViewportToWorldPoint (point);
	}
}

