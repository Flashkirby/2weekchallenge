using UnityEngine;
using System.Collections;

public class CameraLogic : MonoBehaviour {
	Transform playerTransform;
	Camera cam;
	// Use this for initialization
	void Start () {
		playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
		cam = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = playerTransform.position + new Vector3(Settings.camXOffset, Settings.camYOffset, Settings.camZOffset);
		cam.orthographicSize = Settings.camSize;
	}
}
