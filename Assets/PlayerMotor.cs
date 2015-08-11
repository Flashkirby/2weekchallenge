using UnityEngine;
using System.Collections;
/// <summary>
/// Player motor script, controls the player's speed, collision and ground detection.
/// </summary>
public class PlayerMotor : MonoBehaviour 
{
	public Material[] mat;

	private Rigidbody2D r;
	private BoxCollider2D c;
	
	private float graceInputTime;

	public bool grounded;
	public bool lastGrounded;
	public Vector2 lastVelocity;
	public bool autoBehaviour;// move normally, not in an action
	public float jumpTime;// current jump time. resets on landing

	// Use this for initialization
	void Start () {
		r = gameObject.GetComponent<Rigidbody2D>();
		c = gameObject.GetComponent<BoxCollider2D>();
		autoBehaviour = true;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(GlobalInput.ClickDown())
		{
			graceInputTime = Settings.plGraceInputMaxTime;
		}
		
		if(autoBehaviour)
		{
			playerJumpInput();
		}

		graceInputTime -= Time.deltaTime;
	}

	void FixedUpdate()
	{
		if(autoBehaviour)
		{
			checkGround();
			autoAccelerate();
			playerJumpLogic();
		}
		else
		{
			//stop jumplogic
			jumpTime = 0;
			graceInputTime = -Settings.plGraceInputCoolTime;
		}
	}

	void OnGUI ()
	{
		if(Settings.devPlayer)
		{
			GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "player speed x is "+r.velocity.x);
		}
	}

	private void playerJumpInput()
	{
		if(GlobalInput.ClickUp() && jumpTime > 0)
		{
			jumpTime = 0;
		}
		
		if(graceInputTime > 0 && GlobalInput.Click() && grounded)
		{
			graceInputTime = 0;
			jumpTime = Settings.plJumpMax;
		}
	}
	/// <summary>
	/// Determines behaviour for jumping.
	/// Tap when grounded to begin jump
	/// Hold for continued upwards velocity
	/// Automatically stops when let go or reached 0 jumptime
	/// </summary>
	private void playerJumpLogic()
	{
		if(GlobalInput.Click())
		{
			if(jumpTime > 0)
			{
				float jumpSpeed;
				jumpSpeed = Settings.plJumpPower * (jumpTime/Settings.plJumpMax);
				r.velocity = new Vector2(r.velocity.x, jumpSpeed);
				jumpTime -= Time.deltaTime;
			}
		}
	}

	/// <summary>
	/// Increase the player speed on the ground.
	/// </summary>
	private void autoAccelerate()
	{
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
		lastGrounded = grounded;
		//At some point need to check platforms only if we're going downwards

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
	
	/// <summary>
	/// Sets acceleration for actions where default acceleration is disabled
	/// </summary>
	public void setAccelerate(float ratio)
	{
		float velX = r.velocity.x;
		if(grounded)
		{
			velX += Settings.plAccel * ratio;
			velX *= Settings.plFriction;
		}
		else
		{
			velX *= Settings.plAirResistance;
		}
		r.velocity = new Vector2(velX, r.velocity.y);
	}
}
