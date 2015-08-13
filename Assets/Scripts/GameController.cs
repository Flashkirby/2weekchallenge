using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour 
{
	private GameObject player;

	public bool started;
	public float timer;

	public Rect GameScreen{get{return gameScreen;}}
	private Rect gameScreen;

	private Vector3 lastPatternBaseEnd;

	public GameObject PatternStart;
	public GameObject PatternStraight8;

	// Use this for initialization
	void Start () 
	{
		player = GameObject.FindGameObjectWithTag("Player").transform.parent.gameObject;//in case we grab the feet first
		lastPatternBaseEnd = new Vector3(player.transform.position.x, player.GetComponent<Collider2D>().bounds.min.y, 0);
		gameScreen = new Rect(player.transform.position.x - Settings.gameBackScreenX,
		                      player.transform.position.y + Settings.gameScreenY / 2,
		                      Settings.gameScreenX,
		                      Settings.gameScreenY);

		levelGenerateInitial();
		timer = 0;
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.position = player.transform.position;
		if(started)
		{
			gameUpdate();
			levelGenerate();
			cleanUpGameObjects();
		}
		else
		{
			PlayerMotor pm = player.GetComponent<PlayerMotor>();
			Rigidbody2D pr = player.GetComponent<Rigidbody2D>();
			pm.autoBehaviour = false;
			pr.velocity = new Vector2(0, pr.velocity.y);

			if(GlobalInput.ClickUp())
			{
				started = true;
				pm.autoBehaviour = true;
			}
		}
	}

	private void gameUpdate()
	{
		gameScreen = new Rect(player.transform.position.x - Settings.gameBackScreenX,
		                      player.transform.position.y + Settings.gameScreenY / 2,
		                      Settings.gameScreenX,
		                      Settings.gameScreenY);

		timer += Time.deltaTime;
	}

	private void levelGenerateInitial()
	{
		placePrefab(PatternStart);
		levelGenerate();
	}

	private void levelGenerate()
	{
		GameObject prefab = PatternStraight8;
		for (int i = 0; i < 5; i++)
		{
			if(lastPatternBaseEnd.x < gameScreen.max.x)
			{
				placePrefab(prefab);
			}
			else
			{
				break;
			}
		}
	}

	private void placePrefab(GameObject pattern)
	{
		GameObject go = Instantiate(pattern);
		Transform attachStart = null;
		Transform attachEnd = null;

		for(int i = 0; i < go.transform.childCount; i++)
		{
			Transform part = go.transform.GetChild(i);
			if(part.gameObject.CompareTag("AttachStart"))
			{
				attachStart = part.transform;
				continue;
			}
			if(part.gameObject.CompareTag("AttachEnd"))
			{
				attachEnd = part.transform;
			}
		}

		go.transform.position = lastPatternBaseEnd - attachStart.localPosition;
		lastPatternBaseEnd = attachEnd.position;
	}

	private void cleanUpGameObjects()
	{
		foreach(GameObject go in GameObject.FindObjectsOfType<GameObject>())
		{
			if(go.activeInHierarchy && go.transform.position.x < gameScreen.x)
			{
				Destroy(go);
			}
		}
	}
}
