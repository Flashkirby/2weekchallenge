using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour 
{
	private GameObject player;

	public bool started;
	public float timer;

	public Rect GameScreen{get{return gameScreen;}}
	private Rect gameScreen;
	
	
	private System.Random levelRandom;
	public int levelSeed;
	private Vector3 lastPatternBaseStart;//endpoiunt of last base
	private Vector3 lastPatternBaseEnd;//endpoiunt of last base
	private Vector3 lastPatternPlatEnd;//endpoint of last platform
	private float platformHeight;//height of current platend above baseend. -1 means inavtive
	private int platformLength;//current ongoing length of platform combo. increases chances of ending.
	private bool platformEnding;//currently trying to resolve platforms
	private float patternHeight;//current prefab height, stops platforms spawning here

	#region Patterns
	public GameObject PatternStart;
	public GameObject PatternStraight8;
	public GameObject PatternRampDown8;
	public GameObject PatternSwing16;
	public GameObject PatternPlatfStartKickOff16;
	public GameObject PatternPlatStraight8;
	public GameObject PatternPlatDown16;
	#endregion

	// Use this for initialization
	void Start () 
	{
		player = findPlayer();;//in case we grab the feet first
		lastPatternBaseEnd = new Vector3(player.transform.position.x, player.GetComponent<Collider2D>().bounds.min.y, 0);
		gameScreen = new Rect(player.transform.position.x - Settings.gameBackScreenX,
		                      player.transform.position.y + Settings.gameScreenY / 2,
		                      Settings.gameScreenX,
		                      Settings.gameScreenY);
		lastPatternPlatEnd = new Vector3();

		if(levelSeed.Equals(0)) levelSeed = (int)((0.5f - Random.value) * int.MaxValue);
		levelRandom = new System.Random(levelSeed);
		platformHeight = -1;
		platformLength = 0;
		platformEnding = false;

		levelGenerateInitial();
		timer = 0;
	}

	private GameObject findPlayer()
	{
		foreach(GameObject go in GameObject.FindGameObjectsWithTag("Player"))
		{
			if(go.layer.Equals(LayerMask.NameToLayer("Default")))
			{
				return go;
			}
		}
		return null;
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

	void OnGUI ()
	{
		GUI.Label(new Rect(Screen.width/2, 0, Screen.width, Screen.height), "Seed: "+levelSeed);
	}

	private void levelGenerateInitial()
	{
		placePrefab(PatternStart);
		for (int i = 0; i < 5; i++)
		{
			levelGenerate();
		}
	}

	private void levelGenerate()
	{
		if(platformHeight.Equals(-1))
		{
			if(lastPatternBaseEnd.x < gameScreen.max.x)
			{
				if(platformLength <= 0)
				{
					if(levelRandom.Next(Settings.levelPlatformBaseStartChance + platformLength) == 0)
					{
						platformHeight = 0;
						platformLength = 0;
					}
					else
					{
						platformLength--;
					}
				}

				placePrefab(PickPrefab());
			}
		}


		if (platformHeight >= 0
			|| (platformEnding && platformHeight > Settings.levelPlatformMinHeight))
		{

			if(lastPatternBaseEnd.x < gameScreen.max.x)
			{
				placePrefab(PickPrefab());
			}

			if(lastPatternPlatEnd.x < gameScreen.max.x)
			{
				if(platformEnding || levelRandom.Next(Settings.levelPlatformBaseEndChance - platformLength) == 0)
				{
					platformEnding = true;
				}
				else
				{
					platformLength++;
				}


				placePrefabPlat(pickPlatform());
			}
		}
		if(platformEnding && platformHeight <= Settings.levelPlatformMinHeight)
		{
			platformHeight = -1;
			platformLength = 0;
			platformEnding = false;
		}


	}

	private void placePrefab(GameObject pattern)
	{
		GameObject go = Instantiate(pattern);
		Transform attachStart = null;
		Transform attachEnd = null;
		Transform attachPlatform = null;
		patternHeight = float.MinValue;

		for(int i = 0; i < go.transform.childCount; i++)
		{
			Transform part = go.transform.GetChild(i);
			if(part.gameObject.CompareTag("AttachStart"))
			{
				attachStart = part;
				continue;
			}
			if(part.gameObject.CompareTag("AttachEnd"))
			{
				attachEnd = part;
				continue;
			}
			if(part.gameObject.CompareTag("PlatformEnd"))
			{
				attachPlatform = part;
			}
		}

		go.transform.position = lastPatternBaseEnd - attachStart.localPosition;
		for(int i = 0; i < go.transform.childCount; i++)
		{
			try
			{
				Collider2D col = go.transform.GetChild(i).GetComponent<Collider2D>();
				patternHeight = Mathf.Max(patternHeight, col.bounds.max.y);
			}
			catch{}
		}
		lastPatternBaseStart = attachStart.position;
		lastPatternBaseEnd = attachEnd.position;
		if(attachPlatform != null) lastPatternPlatEnd = attachPlatform.position;

	}
	
	private void placePrefabPlat(GameObject platform)
	{
		GameObject go = Instantiate(platform);
		Transform attachStart = null;
		Transform attachEnd = null;
		
		for(int i = 0; i < go.transform.childCount; i++)
		{
			Transform part = go.transform.GetChild(i);
			if(part.gameObject.CompareTag("PlatformStart"))
			{
				attachStart = part;
				continue;
			}
			if(part.gameObject.CompareTag("PlatformEnd"))
			{
				attachEnd = part;
			}
		}
		
		go.transform.position = lastPatternPlatEnd - attachStart.localPosition;
		lastPatternPlatEnd = attachEnd.position;

		for(int i = 0; i < go.transform.childCount; i++)
		{
			try
			{
				Collider2D col = go.transform.GetChild(i).GetComponent<Collider2D>();
				if(col.bounds.max.y < patternHeight)
				{
					Debug.Log(go.name + " height " + col.bounds.max.y + " < " + patternHeight);
					//go.transform.position += new Vector3(0,0.5f);
					//break;
					Destroy(go);
					return;
				}
			}
			catch{}
		}

		platformHeight = lastPatternPlatEnd.y - lastPatternBaseEnd.y;
	}

	private GameObject PickPrefab()
	{
		//Debug.Log(platformHeight + " | " + platformLength + " | " + platformEnding + " | " + patternHeight);

		if(platformHeight.Equals(0) && platformLength.Equals(0))
		{
			platformLength++;
			return PatternPlatfStartKickOff16;
		}
		if(levelRandom.Next(10) == 0)
		{
			return PatternRampDown8;
		}
		if(levelRandom.Next(5) == 0)
		{
			return PatternSwing16;
		}
		return PatternStraight8;
	}

	private GameObject pickPlatform()
	{
		if(platformHeight > Settings.levelPlatformMaxHeight || platformEnding)
		{
			return PatternPlatDown16;
		}
		return PatternPlatStraight8;
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
