using UnityEngine;
using System.Collections;

public class PlayerActionKickOff : MonoBehaviour
{
	public const int myAction = 4;
		
	private Rigidbody2D r;
	private BoxCollider2D c;
	private PlayerMotor motor;

	private float graceInputTime;
	private Vector2 edgePoint;
	private Vector2 edgePointInverse;

	private float actionTime;
	private float actionSnapSpeed = 20f;
	private float saveVelocityX;
	private float saveVelocityNormalX;

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
			kickAction();
		}
		else if(motor.actionActive != 0 && graceInputTime > -Settings.plGraceInputCoolTime)
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
				if(checkKickOffs())
				{
					kickInitial();
				}
			}
			else
			{
				graceInputTime = 0;
			}
		}
	}

	void OnGUI ()
	{
		if(Settings.devPlayer)
		{
			if(motor.actionActive == myAction) GUI.Label(new Rect(0, Screen.height - 20, Screen.width, Screen.height), "kick off is active: " + actionTime);
		}
	}

	/// <summary>
	/// Check through all vaultables for the closest one within a certain distance
	/// </summary>
	private bool checkKickOffs()
	{
		Vector2 checkPoint;
		edgePoint = new Vector2();
		edgePointInverse = new Vector2();
		float yPoint = c.bounds.min.y;
		
		bool pointFound = false;
		
		foreach(GameObject go in GameObject.FindGameObjectsWithTag("KickOff"))
		{
			Collider2D col = go.GetComponent<Collider2D>();
			checkPoint = new Vector2(col.bounds.min.x, col.bounds.max.y);
			if(checkPoint.x > c.bounds.max.x && yPoint <= checkPoint.y + 0.1f && yPoint >= col.bounds.min.y - 0.1f)
			{
				float distNum = Vector2.Distance(transform.position, checkPoint);
				if(distNum <= Settings.plKickJumpCheckDist)
				{
					if(edgePoint == default(Vector2) || Vector2.Distance(transform.position, edgePoint) > distNum)
					{
						edgePoint = checkPoint;
						edgePointInverse = new Vector2(col.bounds.max.x, col.bounds.min.y);
						pointFound = true;
						
						if(Settings.devPlayer)
						{
							Debug.DrawLine(transform.position,checkPoint, Color.magenta);
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

	private void kickInitial()
	{
		motor.actionActive = myAction;
		actionTime = 0;
		
		saveVelocityX = Mathf.Max(r.velocity.x + Settings.plKickJumpBonusVel, Settings.plKickJumpFlatVel);
		saveVelocityNormalX = Mathf.Max(r.velocity.x, Settings.plKickJumpFlatVel / 2);
	}

	private void kickAction()
	{
		motor.autoBehaviour = false;
		
		float progress = actionTime / Settings.plKickJumpTimeMax;
		Vector2 position2d = transform.position;
		Vector2 targetPosition;
		
		targetPosition = new Vector2 (
			edgePoint.x + (edgePointInverse.x - edgePoint.x) * progress,
			edgePointInverse.y + (edgePoint.y - edgePointInverse.y) * progress + c.bounds.extents.y
			);
		
		transform.position = 
			(position2d + (targetPosition * Time.deltaTime * actionSnapSpeed)) 
				/ (1 + Time.deltaTime * actionSnapSpeed);
		
		r.velocity = new Vector2(0, 0);
		
		if(actionTime > Settings.plKickJumpTimeMax || !GlobalInput.Click())
		{
			if(actionTime > Settings.plKickJumpTimeMax)
			{
				transform.position = new Vector2(
					edgePointInverse.x,
					edgePoint.y + c.bounds.extents.y);
				r.velocity = new Vector2(saveVelocityX, 0);
				motor.jumpTime = Settings.plJumpMax;
			}
			else
			{
				r.velocity = new Vector2(saveVelocityNormalX, 1);
			}

			motor.actionActive = 0;
			motor.autoBehaviour = true;
		}
		
		//action time increases faster with your speed
		actionTime += Time.deltaTime * (1 + saveVelocityX / 5);
	}
}
