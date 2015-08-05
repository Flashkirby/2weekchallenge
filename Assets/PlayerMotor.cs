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
		if(GlobalInput.Click())
		{
			r.velocity = new Vector2(r.velocity.x, Settings.plJumpPower);
		}
	}

	/// <summary>
	/// Increase the player speed on the ground.
	/// </summary>
	private void autoAccelerate()
	{
		float velX = r.velocity.x;
		velX += Settings.plAccel;
		velX *= Settings.plFriction;
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
