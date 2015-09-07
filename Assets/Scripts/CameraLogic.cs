using UnityEngine;
using System.Collections;

public class CameraLogic : MonoBehaviour 
{
	public Vector3 CameraVelocity{get{return camVel;}}
	private Vector3 camVel;
	private Vector3 lastPosition;
	private float realVelX;

	private Transform playerTransform;
	private Camera cam;

	// Use this for initialization
	void Start () {
		playerTransform = GameController.findPlayer().transform;
		lastPosition = new Vector3(playerTransform.position.x, transform.position.y);
		cam = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		/*
		transform.position = playerTransform.position + new Vector3(
			Settings.camXOffset, 
			Settings.camYOffset, 
			Settings.camZOffset);
		cam.orthographicSize = Settings.camSize;
    	*/
		
		float smoothVelX = realVelX;
		realVelX = (playerTransform.position.x - lastPosition.x) / Time.fixedDeltaTime;
		smoothVelX = (realVelX + smoothVelX * 2) / 3;

		//Debug.Log(realVelX + " | " + smoothVelX);

		float camS = (Settings.camSize * 0.5f) 
			+ Settings.camSize * smoothVelX * 0.06f;

		cam.orthographicSize = (9 * cam.orthographicSize + camS) / 10f;

		float moveY = Settings.camYOffset + playerTransform.position.y
			+ Mathf.Pow(playerTransform.GetComponent<Rigidbody2D>().velocity.y, 1/3);
		transform.position = new Vector3(
			playerTransform.position.x + Settings.camXOffset - (Settings.camSize - cam.orthographicSize), 
			(transform.position.y + moveY * 0.1f) / 1.1f, 
			Settings.camZOffset);

		camVel = new Vector3(playerTransform.position.x, transform.position.y) - lastPosition;
		
		lastPosition = new Vector3(playerTransform.position.x, transform.position.y);
	}
}
