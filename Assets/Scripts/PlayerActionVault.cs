using UnityEngine;
using System.Collections;

public class PlayerActionVault : MonoBehaviour 
{
	public const int myAction = 3;
	
	private Rigidbody2D r;
	private BoxCollider2D c;
	private PlayerMotor motor;
	
	private float graceInputTime;
	private Vector2 edgePoint;

	private float actionTime;
	public float ActionTime{get{return actionTime;}}
	private float actionSnapSpeed = 50f;
	private float saveVelocityX;

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
				if(checkVaultables())
				{
					vaultInitial();
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
			vaultAction();
		}
	}
	
	void OnGUI ()
	{
		if(Settings.devPlayer)
		{
			if(motor.actionActive == myAction) GUI.Label(new Rect(0, Screen.height - 20, Screen.width, Screen.height), "vaulting is active: " + actionTime);
		}
	}

	/// <summary>
	/// Check through all vaultables for the closest one within a certain distance
	/// </summary>
	private bool checkVaultables()
	{
		Vector2 checkPoint;
		edgePoint = new Vector2();
		float yPoint = c.bounds.min.y;

		bool pointFound = false;

		foreach(GameObject go in GameObject.FindGameObjectsWithTag("VaultObj"))
		{
			Collider2D col = go.GetComponent<Collider2D>();
			checkPoint = new Vector2(col.bounds.min.x, col.bounds.max.y);
			if(checkPoint.x > c.bounds.max.x && yPoint <= checkPoint.y + 0.1f && yPoint >= col.bounds.min.y - 0.1f)
			{
				float distNum = Vector2.Distance(transform.position, checkPoint);
				if(distNum <= Settings.plVaultCheckDist)
				{
					if(edgePoint == default(Vector2) || Vector2.Distance(transform.position, edgePoint) > distNum)
					{
						edgePoint = checkPoint;
						pointFound = true;
						
						if(Settings.devPlayer)
						{
							Debug.DrawLine(transform.position,checkPoint, Color.red);
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

	private void vaultInitial()
	{
		motor.actionActive = myAction;
		actionTime = 0;

		saveVelocityX = Mathf.Max(r.velocity.x + Settings.plVaultBonusVel, Settings.plVaultFlatVel);
	}

	private void vaultAction()
	{
		motor.autoBehaviour = false;

		float progress = actionTime / Settings.plVaultTimeMax;
		Vector2 position2d = transform.position;
		Vector2 targetPosition;

		targetPosition = new Vector2 (
			edgePoint.x + c.bounds.size.x * (-0.5f + progress * 1.8f),
			edgePoint.y + c.bounds.extents.y
			);
		
		transform.position = 
			(position2d + (targetPosition * Time.deltaTime * actionSnapSpeed)) 
				/ (1 + Time.deltaTime * actionSnapSpeed);
		
		r.velocity = new Vector2(0, 0);
		
		if(actionTime > Settings.plVaultTimeMax)
		{
			transform.position = new Vector2(
				edgePoint.x + c.bounds.size.x * 1.3f,
				edgePoint.y + c.bounds.extents.y);
			r.velocity = new Vector2(saveVelocityX, -2);

			motor.actionActive = 0;
			motor.jumpTime = 0;
			motor.autoBehaviour = true;
		}

		//action time increases faster with your speed
		actionTime += Time.deltaTime * (1 + saveVelocityX / 10);
	}
}
