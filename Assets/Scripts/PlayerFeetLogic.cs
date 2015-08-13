using UnityEngine;
using System.Collections;

public class PlayerFeetLogic : MonoBehaviour {
	private const float minColliderSize = 0.06f;
	private BoxCollider2D playerBox;
	private BoxCollider2D playerFeet;

	// Use this for initialization
	void Start () 
	{
		playerBox = transform.parent.GetComponent<BoxCollider2D>();
		playerFeet = GetComponent<BoxCollider2D>();

		playerFeet.size = new Vector2(
			playerBox.size.x,
			minColliderSize / playerBox.transform.localScale.y
			);
		playerFeet.offset = new Vector2(
			playerBox.offset.x,
			playerBox.offset.y - (playerBox.size.y - playerFeet.size.y) / 2
			);
	}
}
