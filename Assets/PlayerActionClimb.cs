using UnityEngine;
using System.Collections;
/// <summary>
/// Module that controls the action of climbing a wall.
/// 
/// When raycast hits near a wall in the grace period
/// and input is still being held down, climb the wall based on 
/// the height of the player in the timeline/progress.
/// </summary>
public class PlayerActionClimb : MonoBehaviour 
{

	private Rigidbody2D r;
	private BoxCollider2D c;
	private PlayerMotor motor;

	private float graceInputTime;
	private float rayStart;
	private float rayDist;
	private Vector2 edgePoint;
	
	public bool actionActive;
	private float actionTime;
	private float actionSnapSpeed = 50f;
	private float autoSnapTime;
	private float saveVelocityX;
	private bool hasSnapped;

	void Start () {
		r = gameObject.GetComponent<Rigidbody2D>();
		c = gameObject.GetComponent<BoxCollider2D>();
		motor = GetComponent<PlayerMotor>();

		rayStart = c.bounds.size.y * (0.5f + 0.25f);
		rayDist = c.bounds.size.y * (0.5f + 0.75f);
	}

	void Update () {
		if(GlobalInput.ClickDown())
		{
			graceInputTime = Settings.plGraceInputMaxTime;
		}

		if(actionActive)
		{
			graceInputTime = 0;
			climbEdge();
		}

		graceInputTime -= Time.deltaTime;
	}

	void FixedUpdate()
	{
		if(graceInputTime > 0 && GlobalInput.Click() && !actionActive)
		{
			checkEdges();
		}
	}
	
	void OnGUI ()
	{
		if(Settings.devPlayer)
		{
			if(actionActive) GUI.Label(new Rect(0, Screen.height - 20, Screen.width, Screen.height), "action is active: " + actionTime);
		}
	}

	/// <summary>
	/// Checks if an edge is climbable
	/// </summary>
	/// <returns><c>true</c>, if edge is found and is clear behind character, <c>false</c> otherwise.</returns>
	private void checkEdges()
	{
		if(Settings.devPlayer)
		{
			Debug.DrawRay(transform.position + new Vector3(c.bounds.size.x * (0.5f + Settings.plClimbReachXMult), rayStart),
			              Vector2.down * rayDist,
			              Color.green);
			Debug.DrawRay(transform.position + new Vector3(c.bounds.size.x * (0.5f + Settings.plClimbReachXMult / 2), rayStart),
			              Vector2.down * rayDist,
			              Color.green);
			Debug.DrawRay(transform.position + new Vector3(0, c.bounds.size.y * 0.5f),
			              Vector2.down * c.bounds.size.y,
			              Color.green);
		}

		bool canClimb = true;
		//check no platforms in me
		RaycastHit2D[] allHits = Physics2D.RaycastAll(
			transform.position + new Vector3(0, c.bounds.size.y * 0.5f),
			Vector2.down,
			c.bounds.size.y);
		foreach (RaycastHit2D hits in allHits)
		{
			if(hits.collider != null && !hits.transform.tag.Equals("Player"))
			{
				canClimb = false;
			}
		}

		//check ahead to set up cllimbEdge
		if(canClimb)
		{
			RaycastHit2D hit = Physics2D.Raycast(
				transform.position + new Vector3(c.bounds.size.x * (0.5f + Settings.plClimbReachXMult), rayStart),
				Vector2.down,
				rayDist);
			if(hit.collider != null && hit.distance > 0)//hit something within the ray
			{
				//start climbing that edge
				climbEdgeInitial(hit);
			}
			else
			{
				hit = Physics2D.Raycast(
					transform.position + new Vector3(c.bounds.size.x * (0.5f + Settings.plClimbReachXMult / 2), rayStart),
					Vector2.down,
					rayDist);
				if(hit.collider != null && hit.distance > 0)//hit something within the ray
				{
					//start climbing that edge
					climbEdgeInitial(hit);
				}
			}
		}
	}

	private void climbEdgeInitial(RaycastHit2D hit)
	{
		actionTime =  hit.distance / rayDist;
		float progress = actionTime / Settings.plClimbTimeMax;
		saveVelocityX = r.velocity.x;
		if(progress < Settings.plClimbRatio)//lose velocity if climbing
		{
			saveVelocityX = 0;
		}
		actionActive = true;
		hasSnapped = false;
		//the top left corner of the collider
		edgePoint = new Vector2(hit.collider.bounds.min.x, hit.collider.bounds.max.y);

		//stop instant teleporting
		autoSnapTime = Mathf.Min(Vector2.Distance(transform.position,edgePoint) * 0.05f, Settings.plClimbTimeMax * 0.5f);
	}

	private void climbEdge()
	{
		if(actionActive)
		{
			motor.autoBehaviour = false;
			float progress = actionTime / Settings.plClimbTimeMax;
			Vector2 position2d = transform.position;
			Vector2 targetPosition;

			//climbing or stepping over
			if(progress < Settings.plClimbRatio)
			{
				targetPosition = new Vector2 (
					edgePoint.x - c.bounds.extents.x,
					(edgePoint.y + c.bounds.extents.y) - (c.bounds.size.y * (Settings.plClimbRatio - progress) * (1/Settings.plClimbRatio))
					);

				transform.position = 
					(position2d + (targetPosition * Time.deltaTime * actionSnapSpeed)) 
						/ (1 + Time.deltaTime * actionSnapSpeed);

				r.velocity = new Vector2(0, 0);
			}
			else
			{
				if(autoSnapTime > 0)
				{
					targetPosition = new Vector2 (
						edgePoint.x - c.bounds.extents.x + 0.1f,
						edgePoint.y + c.bounds.extents.y + 0.05f
						);
					transform.position = 
						(position2d + (targetPosition * Time.deltaTime * actionSnapSpeed)) 
							/ (1 + Time.deltaTime * actionSnapSpeed);
					r.velocity = new Vector2(0, 0);
					hasSnapped = false;
				}
				else
				{
					if(!hasSnapped)
					{
						transform.position = new Vector2(
							edgePoint.x - c.bounds.extents.x + 0.1f,
							edgePoint.y + c.bounds.extents.y + 0.05f);
						r.velocity = new Vector2(saveVelocityX * 0.5f + 3f, 0);
						hasSnapped = true;
					}
					motor.setAccelerate(1.1f);
				}
			}

			if(actionTime > Settings.plClimbTimeMax)
			{
				if(!hasSnapped)
				{
					transform.position = new Vector2(
						edgePoint.x - c.bounds.extents.x + 0.1f,
						edgePoint.y + c.bounds.extents.y + 0.05f);
					r.velocity = new Vector2(saveVelocityX, 0);
				}
				actionActive = false;
				motor.autoBehaviour = true;
			}
			
			actionTime += Time.deltaTime;
			autoSnapTime -= Time.deltaTime;
		}

	}
}