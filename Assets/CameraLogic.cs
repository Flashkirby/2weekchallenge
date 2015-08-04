using UnityEngine;
using System.Collections;

public class CameraLogic : MonoBehaviour {
	Transform playerTransform;
	// Use this for initialization
	void Start () {
		playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = playerTransform.position + new Vector3(Settings.camXOffset, Settings.camYOffset, Settings.camZOffset);
	}
}
