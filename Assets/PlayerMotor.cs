using UnityEngine;
using System.Collections;

public class PlayerMotor : MonoBehaviour {
	
	private Rigidbody2D r;
	// Use this for initialization
	void Start () {
		r = gameObject.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		autoAccelerate();
		playerInput();
	}

	private void playerInput()
	{
		if(GlobalInput.Click())
		{
			r.velocity = new Vector2(r.velocity.x, Settings.plJumpPower);
		}
	}

	private void autoAccelerate()
	{
		float velX = r.velocity.x;
		velX += Settings.plAccel;
		velX *= Settings.plFriction;
		r.velocity = new Vector2(velX, r.velocity.y);
	}
}
