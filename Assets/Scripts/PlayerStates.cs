using UnityEngine;
using System.Collections;

public class PlayerStates : MonoBehaviour 
{
	
	private Rigidbody2D r;
	private BoxCollider2D c;
	private PlayerMotor motor;
	private SpriteRenderer rend;
	private Animator anim;

	private PlayerActionClimb actClimb;
	private PlayerActionRoll actLand;
	private PlayerActionVault actVault;
	private PlayerActionKickOff actKick;
	private PlayerActionSwing actSwing;

	//public Sprite stateRunning;
	public Sprite stateCrouch;
	public Sprite stateSwing;
	public Sprite stateVault;

	// Use this for initialization
	void Start () 
	{
		r = gameObject.GetComponent<Rigidbody2D>();
		c = gameObject.GetComponent<BoxCollider2D>();
		motor = GetComponent<PlayerMotor>();
		rend = GetComponent<SpriteRenderer>();
		anim = GetComponent<Animator>();

		actClimb = GetComponent<PlayerActionClimb>();
		actLand = GetComponent<PlayerActionRoll>();
		actVault = GetComponent<PlayerActionVault>();
		actKick = GetComponent<PlayerActionKickOff>();
		actSwing = GetComponent<PlayerActionSwing>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		switch(motor.actionActive)
		{
		case 1://climb
			if(r.velocity.x.Equals(0))
			{
				rend.sprite = stateCrouch;
			}
			else
			{
				rend.sprite = stateVault;
			}
			break;
		case 2://roll
			rend.sprite = stateCrouch;
			break;
		case 3://vault
			float progress = actVault.ActionTime / Settings.plVaultTimeMax;
			anim.Play("vault", 0, progress);
			break;
		case 4://kickup
			rend.sprite = stateCrouch;
			break;
		case 5://swing
			rend.sprite = stateSwing;
			break;
		default:
			if(motor.grounded)
			{
				anim.Play("running");
				anim.SetFloat("SpeedMult",r.velocity.x * 0.16f);
			}
			else
			{
				anim.Play("jump");
				anim.SetInteger("ActionState", 1);
			}
			break;
		}
	}
}
