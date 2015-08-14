using UnityEngine;
using System.Collections;
/// <summary>
/// Module that controls the action of falling from a height.
/// Rolls if condition is met
/// Lose speed and stumble or stop otherwise
/// Can also die which should trigger a game over
/// 
/// If player taps within the grace period for landing
/// the player will not lose speed.
/// </summary>
public class PlayerActionRoll : MonoBehaviour 
{
	public const int myAction = 2;
	
	private Rigidbody2D r;
	private BoxCollider2D c;
	private PlayerMotor motor;
	
	private float graceInputTime;

	public bool rollOnLand;
	private bool isLandingStumbled;
	private bool isLandingHard;
	private bool isLandingDeath;
	private float actionTime;
	
	void Start () 
	{
		r = gameObject.GetComponent<Rigidbody2D>();
		c = gameObject.GetComponent<BoxCollider2D>();
		motor = GetComponent<PlayerMotor>();
	}

	void Update () 
	{
		if(GlobalInput.ClickDown() && graceInputTime <= -Settings.plGraceInputCoolTime)
		{
			graceInputTime = Settings.plGraceInputMaxTime;
		}
		
		if(motor.actionActive == myAction)
		{
			graceInputTime = -Settings.plGraceInputCoolTime;
		}
		else if(motor.actionActive != 0 && graceInputTime > -Settings.plGraceInputCoolTime)
		{
			graceInputTime = -Settings.plGraceInputCoolTime;
		}
		
		graceInputTime -= Time.deltaTime;
	}

	void FixedUpdate()
	{
		if(!motor.grounded && r.velocity.y < 0)
		{
			checkLandingConditions();
		}

		if(motor.grounded && !motor.lastGrounded && motor.autoBehaviour)
		{
			//Debug.Log("Yeah " + rollOnLand + "|" + isLandingStumbled + "|" + isLandingHard + "|" + isLandingDeath);
			if (rollOnLand || isLandingStumbled || isLandingHard || isLandingDeath)
			{
				motor.actionActive = myAction;
				actionTime = 0;
			}
		}

		if(motor.grounded && motor.actionActive == myAction)
		{
			motor.autoBehaviour = false;
			bool done = false;

			if(isLandingDeath)
			{
				landDeath();
			}
			else
			{
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
					motor.actionActive = 0;
					motor.autoBehaviour = true;
					resetRollVariables();
				}
				
				actionTime += Time.deltaTime;
			}
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
			if(motor.actionActive == myAction) GUI.Label(new Rect(0, Screen.height - 20, Screen.width, Screen.height), rollType + " is active: " + actionTime);
		}
	}

	public void resetRollVariables()
	{
		rollOnLand = false;
		isLandingStumbled = false;
		isLandingHard = false;
		isLandingDeath = false;
		graceInputTime = -Settings.plGraceInputCoolTime;
	}
	
	private void checkLandingConditions()
	{
		if(checkFloor() && motor.actionActive == 0)
		{
			if(graceInputTime > 0)
			{
				rollOnLand = true;
				graceInputTime = 0;
			}
			// in such as case where we magically go upwards, forget about this
			if(r.velocity.y > 0)
			{
				resetRollVariables();
			}
		}
		if (r.velocity.y < -Settings.plFallStumbleVel)
		{
			isLandingStumbled = true;
		}
		if (r.velocity.y < -Settings.plFallHardVel)
		{
			isLandingHard = true;
		}
		if(r.velocity.y < -Settings.plFallDeathVel)
		{
			isLandingDeath = true;
		}
	}

	private bool checkFloor()
	{
		Vector3 velo3D = new Vector3(r.velocity.x, r.velocity.y, 0) * Time.fixedDeltaTime;
		if(Settings.devPlayer)
		{
			Color col = Color.gray;
			if(graceInputTime > 0) col = Color.cyan;
			Debug.DrawRay(transform.position + velo3D + new Vector3(-c.bounds.size.x/2, -c.bounds.size.y/2 - 0.001f),
			              Vector2.down * Settings.plRollCheckDist,
			              col);
			Debug.DrawRay(transform.position + velo3D + new Vector3(c.bounds.size.x/2, -c.bounds.size.y/2 - 0.001f),
			              Vector2.down * Settings.plRollCheckDist,
			              col);
		}

		bool onGround = Physics2D.Raycast(
			transform.position + velo3D - new Vector3(c.bounds.size.x / 2, c.bounds.size.y / 2 + 0.005f),
			Vector2.down,
			Settings.plRollCheckDist);
		//check right side if left side is not fine
		if(!onGround)
		{
			onGround = Physics2D.Raycast(
				transform.position + velo3D - new Vector3(-c.bounds.size.x / 2, c.bounds.size.y / 2 + 0.005f),
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
		if(actionTime == 0)
		{
			r.velocity = new Vector2(r.velocity.x * 0.05f, r.velocity.y);
		}
		motor.setAccelerate(0);
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

	private void landDeath()
	{
		r.velocity = new Vector2(r.velocity.x * 0.2f, r.velocity.y);
	}
}
