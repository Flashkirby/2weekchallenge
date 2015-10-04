using UnityEngine;
using System.Collections;

public class playerTripVault : MonoBehaviour {
	private Rigidbody2D r;
	private BoxCollider2D c;
	private PlayerMotor motor;

	// Use this for initialization
	void Start () {
		r = gameObject.GetComponent<Rigidbody2D>();
		c = gameObject.GetComponent<BoxCollider2D>();
		motor = GetComponent<PlayerMotor>();
	
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag.Equals("VaultObj") && motor.autoBehaviour && c.bounds.max.x < other.transform.position.x)
		{
			r.velocity = new Vector2(r.velocity.x * Settings.plObstacleSlowdownMult, r.velocity.y);

			other.gameObject.AddComponent<Rigidbody2D>();
			other.attachedRigidbody.velocity += new Vector2(r.velocity.x + Random.Range(3,6), r.velocity.x);
			other.attachedRigidbody.AddTorque(Random.Range(-50,-1001));
			other.tag = "Untagged";
		}
	}
}
