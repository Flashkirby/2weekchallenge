using UnityEngine;
using System.Collections;

public class PlatformFiller : MonoBehaviour {

	public GameObject fillType;

	private Vector3 lastPlacement;
	private bool run;

	// Use this for initialization
	void Start () 
	{
		lastPlacement = transform.position;
		for(int i = 0; i < 10; i++)
		{
			GameObject background = Instantiate(fillType);
			Collider2D col = background.GetComponent<Collider2D>();
			background.transform.position = new Vector3(
				lastPlacement.x,
				lastPlacement.y - col.bounds.extents.y,
				0
				);
			lastPlacement = new Vector3( 
			                            background.transform.position.x,
			                            col.bounds.min.y,
			                            0
			                            );
		}
	}
	void FixedUpdate()
	{
	}
}
