using UnityEngine;
using System.Collections;

public class PlayerStates : MonoBehaviour 
{
	
	private Rigidbody2D r;
	private BoxCollider2D c;
	private PlayerMotor motor;
	private SpriteRenderer rend;

	public Sprite stateRunning;
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
			rend.sprite = stateVault;
			break;
		case 4://kickup
			rend.sprite = stateCrouch;
			break;
		case 5://swing
			rend.sprite = stateSwing;
			break;
		default:
			rend.sprite = stateRunning;
			break;
		}
	}
}
