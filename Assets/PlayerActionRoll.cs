using UnityEngine;
using System.Collections;
/// <summary>
/// Modules that controls the action of falling from a height.
/// Rolls if condition is met
/// Lose speed and stumble or stop otherwise
/// 
/// If player taps within the grace period for landing
/// the player will not lose speed.
/// </summary>
public class PlayerActionRoll : MonoBehaviour 
{
	
	private Rigidbody2D r;
	private BoxCollider2D c;
	private PlayerMotor motor;
	
	private float graceInputTime;
	private float rayStart;
	private float rayDist;
	
	public bool actionActive;
	private float actionTime;
	
	void Start () {
		r = gameObject.GetComponent<Rigidbody2D>();
		c = gameObject.GetComponent<BoxCollider2D>();
		motor = GetComponent<PlayerMotor>();

		rayStart = c.bounds.size.y * (0.5f + 0.25f);
		rayDist = c.bounds.size.y * (0.5f + 0.75f);
	}

	void Update () 
	{
	
	}

	void FixedUpdate()
	{
	}
	
	void OnGUI ()
	{
		if(Settings.devPlayer)
		{
			if(!motor.grounded)GUI.Label(new Rect(0, 10, Screen.width, Screen.height), "player speed y is "+r.velocity.y);
		}
	}
}
