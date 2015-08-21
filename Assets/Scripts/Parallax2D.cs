using UnityEngine;
using System.Collections;

public class Parallax2D : MonoBehaviour {
	public Sprite filler;
	public float distanceModifier;

	private SpriteRenderer rend;
	private Transform cam;
	private GameController gc;
	private bool markForClone;

	// Use this for initialization
	void Start () 
	{
		rend = GetComponent<SpriteRenderer>();
		cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
		gc = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
		markForClone = false;
		rend.sortingOrder = -(int)(distanceModifier * 100f);

		GameObject fillerObj = new GameObject();
		fillerObj.transform.SetParent(transform);
		SpriteRenderer fillRend = fillerObj.AddComponent<SpriteRenderer>();
		fillRend.sprite = filler;
		fillerObj.transform.localPosition = Vector3.zero;
		fillerObj.transform.localScale = new Vector3(rend.sprite.bounds.size.x / fillRend.sprite.bounds.size.x, 1000, 1);
	}

	void FixedUpdate () 
	{
		transform.position += cam.GetComponent<CameraLogic>().CameraVelocity * distanceModifier;
		if(transform.position.x < gc.GameScreen.max.x)
		{
			//transform.position = transform.position + new Vector3(rend.sprite.bounds.size.x, 0);
			if(!markForClone)
			{
				Instantiate(gameObject, transform.position + new Vector3(rend.sprite.bounds.size.x, 0), transform.rotation);

			}
			markForClone = true;
		}
	}
}
