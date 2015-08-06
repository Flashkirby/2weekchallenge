using UnityEngine;
using System.Collections;
/// <summary>
/// Player motor script, controls the player's speed, collision and ground detection.
/// </summary>
public class PlayerMotor : MonoBehaviour {
	public Material[] mat;

	private Rigidbody2D r;
	private BoxCollider2D c;

	public bool grounded;
	public float jumpTime;// current jump time. resets on landing

	// Use this for initialization
	void Start () {
		r = gameObject.GetComponent<Rigidbody2D>();
		c = gameObject.GetComponent<BoxCollider2D>();
	}
	
	// Update is called once per frame
	void Update () {
		checkGround();
		autoAccelerate();
		playerInput();
	}

	private void playerInput()
	{
		playerJump();
	}

	/// <summary>
	/// Determines behaviour for jumping.
	/// Tap when grounded to begin jump
	/// Hold for continued upwards velocity
	/// Automatically stops when let go or reached 0 jumptime
	/// </summary>
	/// <returns><c>true</c>, if jumping is still active, <c>false</c> otherwise.</returns>
	private bool playerJump()
	{
		if(GlobalInput.ClickUp() && jumpTime > 0)
		{
			jumpTime = 0;
		}
		
		if(grounded && GlobalInput.ClickDown())
		{
			jumpTime = Settings.plJumpMax;
		}

		if(GlobalInput.Click())
		{
			if(jumpTime > 0)
			{
				float jumpSpeed;
				jumpSpeed = Settings.plJumpPower * (jumpTime/Settings.plJumpMax);
				r.velocity = new Vector2(r.velocity.x, jumpSpeed);
				jumpTime -= Time.deltaTime;
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Increase the player speed on the ground.
	/// </summary>
	private void autoAccelerate()
	{
		if(Settings.devPlayer)
		{
			Debug.Log("player speed x is "+r.velocity.x);
		}
		float velX = r.velocity.x;
		if(grounded)
		{
			velX += Settings.plAccel;
			velX *= Settings.plFriction;
		}
		else
		{
			velX *= Settings.plAirResistance;
		}
		r.velocity = new Vector2(velX, r.velocity.y);
	}

	/// <summary>
	/// Checks the ground by performign two raycast checks on either side of the box.
	/// </summary>
	private void checkGround()
	{
		//!!! NEED TO CHECK platforms only if we're going downwards

		//check left side
		bool onGround = Physics2D.Raycast(
			transform.position - new Vector3(c.bounds.size.x / 2, c.bounds.size.y / 2 + 0.005f),
			Vector2.down,
			0.1f);
		//check right side if left side is not fine
		if(!onGround)
		{
			onGround = Physics2D.Raycast(
				transform.position - new Vector3(-c.bounds.size.x / 2, c.bounds.size.y / 2 + 0.005f),
				Vector2.down,		 
				0.1f);
		}

		grounded = onGround;

		if(Settings.devPlayer)
		{
			Debug.DrawRay(transform.position + new Vector3(-c.bounds.size.x/2, -c.bounds.size.y/2 - 0.001f),
			              Vector2.down * 0.1f,
			              Color.red);
			Debug.DrawRay(transform.position + new Vector3(c.bounds.size.x/2, -c.bounds.size.y/2 - 0.001f),
			              Vector2.down * 0.1f,
		              Color.red);
			if(grounded)
			{
				Renderer rend = GetComponent<Renderer>();
				rend.material = mat[0];
			}
			else
			{
				Renderer rend = GetComponent<Renderer>();
				rend.material = mat[1];
			}
		}
	}
}
