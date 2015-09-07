using UnityEngine;
using System.Collections;
/// <summary>
/// Player motor script, controls the player's speed, collision and ground detection.
/// </summary>
public class PlayerMotor : MonoBehaviour 
{
	//public Material[] mat;

	private Rigidbody2D r;
	private BoxCollider2D c;
	
	public float graceInputTime;

	public bool grounded;
	public bool lastGrounded;
	public Vector2 lastVelocity;
	public int actionActive;
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
		/*
		//FLOATING POINT ERROR NOOOOOOOOO
		if(r.velocity.x.Equals(0) && autoBehaviour)
		{
			transform.position =
				new Vector3(
					transform.position.x,
					transform.position.y + 0.001f,
					0
					);
		}
		//OK FINE NOW
		*/

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
		}
		stepUpToGround();
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
		if(!GlobalInput.Click() && jumpTime > 0)
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

	private void stepUpToGround()
	{
		//don't run if the feet are disabled for whatever reason
		if(!transform.GetChild(0).GetComponent<Collider2D>().enabled) return;
		//don't run if moving up
		if(r.velocity.y > 0) return;

		float snapdist = 0.3f;
		Vector2 timeVelo = r.velocity * Time.fixedDeltaTime;
		if(Settings.devPlayer)
		{
			Debug.DrawRay(new Vector2(c.bounds.max.x + 2 * timeVelo.x, c.bounds.min.y + snapdist),
			              Vector2.down * (snapdist - 2 * Mathf.Min(timeVelo.y,0)));
		}
		RaycastHit2D hit = Physics2D.Raycast(
			new Vector2(c.bounds.max.x + 2 * timeVelo.x, c.bounds.min.y + snapdist),
			Vector2.down,
			snapdist - 2 * Mathf.Min(timeVelo.y,0));
		if(hit.collider != null && hit.distance > 0)//hit something within the ray
		{
			float setPos = snapdist - hit.distance;
			//Debug.Log(hit.distance);
			transform.position =
				new Vector3(
					transform.position.x,
					transform.position.y + setPos,
					transform.position.z);
			if(!grounded)
			{
				r.velocity = new Vector2(r.velocity.x, 0);
			}
		}
	}

	/// <summary>
	/// Checks the ground by performign two raycast checks on either side of the box.
	/// </summary>
	private void checkGround()
	{
		lastGrounded = grounded;
		//At some point need to check platforms only if we're going downwards
		bool onGround = false;
		
		//check left side
		RaycastHit2D[] allHits = Physics2D.RaycastAll(
			transform.position - new Vector3(c.bounds.size.x / 2 - 0.02f, c.bounds.size.y / 2 - 0.025f),
			Vector2.down,
			0.05f);
		foreach (RaycastHit2D hits in allHits)
		{
			if(hits.collider != null && !hits.transform.tag.Equals("Player"))
			{
				onGround = true;
			}
		}
		//check right side if left side is not fine
		if(!onGround)
		{
			allHits = Physics2D.RaycastAll(
				transform.position - new Vector3(-c.bounds.size.x / 2 + 0.02f, c.bounds.size.y / 2 - 0.025f),
				Vector2.down,		 
				0.05f);
			foreach (RaycastHit2D hits in allHits)
			{
				if(hits.collider != null && !hits.transform.tag.Equals("Player"))
				{
					onGround = true;
				}
			}
		}

		grounded = onGround;

		if(Settings.devPlayer)
		{
			Debug.DrawRay(transform.position + new Vector3(c.bounds.size.x/2 - 0.02f, -c.bounds.size.y/2 + 0.025f),
			              Vector2.down * 0.05f,
			              Color.red);
			Debug.DrawRay(transform.position + new Vector3(-c.bounds.size.x/2 + 0.02f, -c.bounds.size.y/2 + 0.025f),
			              Vector2.down * 0.05f,
			              Color.red);
			/*
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
			*/
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

	public void setRotation(float angDeg)
	{
		transform.Rotate(Vector3.forward, angDeg - transform.rotation.eulerAngles.z);
	}
}
