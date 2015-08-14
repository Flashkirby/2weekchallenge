using UnityEngine;
using System.Collections;

public class PlayerActionSwing : MonoBehaviour
{
	public const int myAction = 5;
	
	private Rigidbody2D r;
	private BoxCollider2D c;
	private PlayerMotor motor;
	
	private float graceInputTime;
	private Vector2 swingingPoint;
	//private GameObject swingingPoint;
	//private DistanceJoint2D joint;

	private float actionAngle;
	private float actionSnapSpeed = 50f;
	private float autoSnapTime;
	private float swingSpeed;
	
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

		if(!motor.autoBehaviour)
		{
			graceInputTime = -Settings.plGraceInputCoolTime;
		}
		
		graceInputTime -= Time.deltaTime;
	}
	
	
	void FixedUpdate () 
	{
		if(graceInputTime > 0 && motor.actionActive == 0)
		{
			if(GlobalInput.Click())
			{
				if(checkSwings())
				{
					swingInitial();
				}
			}
			else
			{
				graceInputTime = 0;
			}
		}
		
		if(motor.actionActive == myAction)
		{
			graceInputTime = -Settings.plGraceInputCoolTime;
			swingAction();
		}
		else if(motor.actionActive != 0 && graceInputTime > -Settings.plGraceInputCoolTime)
		{
			graceInputTime = -Settings.plGraceInputCoolTime;
		}
	}
	
	void OnGUI ()
	{
		if(Settings.devPlayer)
		{
			if(motor.actionActive == myAction) GUI.Label(new Rect(0, Screen.height - 20, Screen.width, Screen.height), "swinging is active");
		}
	}

	/// <summary>
	/// Looks for swings near the player
	/// </summary>
	/// <returns><c>true</c>, if swing is found and sets it, <c>false</c> otherwise.</returns>
	private bool checkSwings()
	{
		swingingPoint = new Vector2();
		Vector2 checkPoint = new Vector2();
		
		bool pointFound = false;
		
		foreach(GameObject go in GameObject.FindGameObjectsWithTag("Swing"))
		{
			Rigidbody2D swingR = go.GetComponent<Rigidbody2D>();
			checkPoint = new Vector2(go.transform.position.x, go.transform.position.y);
			if(checkPoint.x >= transform.position.x)
			{
				float distNum = Vector2.Distance(transform.position, checkPoint);
				if(distNum <= Settings.plSwingSnapDist)
				{
					if(swingingPoint == default(Vector2) || Vector2.Distance(transform.position, swingingPoint) > distNum)
					{
						swingingPoint = new Vector2(go.transform.position.x, go.transform.position.y);
						pointFound = true;
						
						if(Settings.devPlayer)
						{
							Debug.DrawLine(transform.position,checkPoint, Color.white);
						}
						continue;
					}
				}
				//edgePoint
				if(Settings.devPlayer)
				{
					Debug.DrawLine(transform.position,checkPoint, Color.gray);
				}
			}
		}
		
		return pointFound;
	}

	private void swingInitial()
	{
		motor.actionActive = myAction;
		
		//stop instant teleporting
		autoSnapTime = Mathf.Min(Vector2.Distance(transform.position, swingingPoint) * 0.05f, Settings.plClimbTimeMax * 0.5f);

		/*
		joint = gameObject.AddComponent<DistanceJoint2D>();
		joint.connectedBody = swingingPoint.GetComponent<Rigidbody2D>();
		joint.distance = Settings.plSwingRadius;
		joint.maxDistanceOnly = true;
		*/
	}

	/// <summary>
	/// Removes swing if we run out of momentum, hit the designated angle, or let go
	/// </summary>
	private void swingAction()
	{
		motor.autoBehaviour = false;
		transform.GetChild(0).GetComponent<Collider2D>().enabled = false;
		motor.grounded = false;
		Vector2 position2D = new Vector2(transform.position.x, transform.position.y);

		actionAngle = GetAngleRad(swingingPoint, position2D);
		float angleInv = GetAngleRad(position2D, swingingPoint);
		
		swingSpeed = r.velocity.magnitude;
		r.velocity = new Vector2(swingSpeed * Mathf.Cos(angleInv), -swingSpeed * Mathf.Sin(angleInv));

		float angularVelo;
		float angConst = 0.002f / Settings.plSwingRadius;
		angularVelo = swingSpeed * angConst;

		actionAngle -= angularVelo;

		Vector2 targetPosition = new Vector2(
			swingingPoint.x + Settings.plSwingRadius * Mathf.Sin(actionAngle),
			swingingPoint.y + Settings.plSwingRadius * Mathf.Cos(actionAngle)
			);

		if(autoSnapTime > 0)
		{
			transform.position = 
				(position2D + (targetPosition * Time.deltaTime * actionSnapSpeed))
					/ (1 + Time.deltaTime * actionSnapSpeed);
		}
		else
		{
			transform.position = targetPosition;
		}

		if((r.velocity.x < 1f && transform.position.x > swingingPoint.x)
		   || Settings.plSwingAutoReleaseAng >= angleInv * Mathf.Rad2Deg
		   || !GlobalInput.Click()
		   )
		{
			motor.actionActive = 0;
			motor.autoBehaviour = true;
			transform.GetChild(0).GetComponent<Collider2D>().enabled = true;
			motor.jumpTime = 0;
		}

		autoSnapTime -= Time.deltaTime;
		/*
		if(	!GlobalInput.Click() ||
		    Vector2.Angle(transform.position, swingingPoint.transform.position) > Settings.plSwingAutoReleaseAng ||
			(r.velocity.y < -0.1f && transform.position.x > swingingPoint.transform.position.x) ||
		    r.velocity.x < 0 )
		{
			motor.actionActive = 0;

			Destroy(joint);
		}
		*/
	}

	private float GetAngleRad(Vector2 start, Vector2 end)
	{
		float rad = Mathf.Atan2(end.x - start.x, end.y - start.y);
		return rad;
	}
}
