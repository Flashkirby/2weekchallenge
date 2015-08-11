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
	public bool rollOnLand;
	private bool isLandingStumbled;
	private bool isLandingHard;
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
		if(GlobalInput.ClickDown() && graceInputTime <= -Settings.plGraceInputCoolTime)
		{
			graceInputTime = Settings.plGraceInputMaxTime;
		}
		
		if(actionActive)
		{
			graceInputTime = -Settings.plGraceInputCoolTime;
		}
		
		graceInputTime -= Time.deltaTime;
	}

	void FixedUpdate()
	{
		if(!motor.grounded)
		{
			checkLandingConditions();
		}

		if(motor.grounded && !motor.lastGrounded && motor.autoBehaviour)
		{
			if (rollOnLand || isLandingStumbled || isLandingHard)
			{
				actionActive = true;
				actionTime = 0;
			}
		}

		if(motor.grounded && actionActive)
		{
			motor.autoBehaviour = false;
			bool done = false;
			if(isLandingHard)
			{
				if(!rollOnLand)
				{
					landHard();
					done = actionTime > Settings.plFallHardTimeMax;
				}
				else
				{
					landHardRoll();
					done = actionTime > Settings.plRollTimeMax;
				}
			}
			else
			{
				if(isLandingStumbled)
				{
					if(!rollOnLand)
					{
						landStumble();
						done = actionTime > Settings.plFallStumbleTimeMax;
					}
					else
					{
						landShortRoll();
						done = actionTime > Settings.plRollShortTimeMax;
					}
				}
				else
				{
					done = true;
				}
			}

			if(done)
			{
				actionActive = false;
				motor.autoBehaviour = true;
			}
			
			actionTime += Time.deltaTime;
		}
	}
	
	void OnGUI ()
	{
		if(Settings.devPlayer)
		{
			if(!motor.grounded)GUI.Label(new Rect(0, 10, Screen.width, Screen.height), "player speed y is "+r.velocity.y);

			string rollType;
			if(isLandingHard)
			{
				if(!rollOnLand)
				{
					rollType = "landing hard";
				}
				else
				{
					rollType = "landing roll";
				}
			}
			else
			{
				if(isLandingStumbled && !rollOnLand)
				{
					rollType = "landing stumble";
				}
				else
				{
					rollType = "landing small roll";
				}
			}
			if(actionActive) GUI.Label(new Rect(0, Screen.height - 20, Screen.width, Screen.height), rollType + " is active: " + actionTime);
		}
	}

	private void checkLandingConditions()
	{
		if(checkFloor() && !actionActive)
		{
			if(graceInputTime > 0)
			{
				rollOnLand = true;
				graceInputTime = 0;
			}
			else
			{
				if (r.velocity.y < -Settings.plFallStumbleVel)
				{
					isLandingStumbled = true;
				}
				if (r.velocity.y < -Settings.plFallHardVel)
				{
					isLandingHard = true;
				}
			}
			// in such as case where we magically go upwards, forget about this
			if(r.velocity.y > 0)
			{
				rollOnLand = false;
				isLandingStumbled = false;
				isLandingHard = false;
				graceInputTime = -Settings.plGraceInputCoolTime;
			}
		}
	}

	private bool checkFloor()
	{
		if(Settings.devPlayer)
		{
			Color col = Color.gray;
			if(graceInputTime > 0) col = Color.cyan;
			Debug.DrawRay(transform.position + new Vector3(-c.bounds.size.x/2, -c.bounds.size.y/2 - 0.001f),
			              Vector2.down * Settings.plRollCheckDist,
			              col);
			Debug.DrawRay(transform.position + new Vector3(c.bounds.size.x/2, -c.bounds.size.y/2 - 0.001f),
			              Vector2.down * Settings.plRollCheckDist,
			              col);
		}

		bool onGround = Physics2D.Raycast(
			transform.position - new Vector3(c.bounds.size.x / 2, c.bounds.size.y / 2 + 0.005f),
			Vector2.down,
			Settings.plRollCheckDist);
		//check right side if left side is not fine
		if(!onGround)
		{
			onGround = Physics2D.Raycast(
				transform.position - new Vector3(-c.bounds.size.x / 2, c.bounds.size.y / 2 + 0.005f),
				Vector2.down,		 
				Settings.plRollCheckDist);
		}

		return onGround;
	}



	private void landStumble()
	{
		//float progress = actionTime / Settings.plFallStumbleTimeMax;
		if(actionTime == 0)
		{
			r.velocity = new Vector2(r.velocity.x * Settings.plFallStumbleSlowDown, r.velocity.y);
		}
		motor.setAccelerate(Settings.plFallStumbleAccel);
	}


	private void landHard()
	{
		//float progress = actionTime / Settings.plFallStumbleTimeMax;
		r.velocity = new Vector2(0, r.velocity.y);
	}

	private void landShortRoll()
	{
		//float progress = actionTime / Settings.plFallStumbleTimeMax;
		motor.setAccelerate(Settings.plRollShortAccel);
	}

	private void landHardRoll()
	{
		//float progress = actionTime / Settings.plFallStumbleTimeMax;
		if(actionTime == 0)
		{
			r.velocity = new Vector2(r.velocity.x * Settings.plRollSlowDown, r.velocity.y);
		}
		motor.setAccelerate(Settings.plRollAccel);
	}
}
