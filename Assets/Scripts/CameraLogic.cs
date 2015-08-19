using UnityEngine;
using System.Collections;

public class CameraLogic : MonoBehaviour {
	Transform playerTransform;
	float playerLastPosX;
	Camera cam;
	// Use this for initialization
	void Start () {
		playerTransform = GameController.findPlayer().transform;
		playerLastPosX = playerTransform.position.x;
		cam = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		/*
		transform.position = playerTransform.position + new Vector3(
			Settings.camXOffset, 
			Settings.camYOffset, 
			Settings.camZOffset);
		cam.orthographicSize = Settings.camSize;
		*/
		float realVelX = (playerTransform.position.x - playerLastPosX) / Time.fixedDeltaTime;
		float camS = (Settings.camSize * 0.5f) 
			+ Settings.camSize * realVelX * 0.06f;
		cam.orthographicSize = (9 * cam.orthographicSize + camS) / 10f;

		float moveY = Settings.camYOffset + playerTransform.position.y + Mathf.Pow(playerTransform.GetComponent<Rigidbody2D>().velocity.y, 3) * 0.001f;
		transform.position = new Vector3(
			playerTransform.position.x + Settings.camXOffset - (Settings.camSize - cam.orthographicSize), 
			(transform.position.y + moveY * 0.1f) / 1.1f, 
			Settings.camZOffset);
		playerLastPosX = playerTransform.position.x;
	}
}
