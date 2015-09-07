using UnityEngine;
using System.Collections;

public class spriteRandomiser : MonoBehaviour {
	public Sprite[] Selection;

	// Use this for initialization
	void Start () 
	{
		GetComponent<SpriteRenderer>().sprite = Selection[Random.Range(0, Selection.Length)];
	}
}
