using UnityEngine;
using System.Collections;

public class PlayerMover : MonoBehaviour 
{
	public PlayerAvatar Avatar = null;

	public float CameraViewDistance = 5;

	// Use this for initialization
	void Start () 
	{
		Avatar = gameObject.GetComponent<PlayerAvatar>();

		if (Avatar == null)
		{
			Debug.Log("Player Mover spawned but not attached to an avatar");
		}
		else
		{
			// attach the camera to us
		}
		{
			Camera.main.gameObject.transform.SetParent(null,true);
			Camera.main.gameObject.transform.position = new Vector3(0, CameraViewDistance, 0);
			Camera.main.gameObject.transform.SetParent(this.gameObject.transform, true);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
