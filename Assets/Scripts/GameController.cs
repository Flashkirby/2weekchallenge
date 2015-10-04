using UnityEngine;
using System.Collections;

/// <summary>
/// TODO
/// Why is random platforms appearing - trying to place more platforms but a platform starter isn't being spawned
/// see seed -578153728
/// </summary>

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
	private float patternHeight;//current prefab height world position, stops platforms spawning here.
	private enum GenState{
		ground,
		platformStart,
		groundPlatform,
		platformEnding
	}
	private GenState genstate;

	private int patternStyle;//used to denote theme or style of current base and platforms.
	private int baseLength;//current length of the base (since last gap)

	#region Patterns
	public GameObject PatternStart;
	public GameObject PatternStraight8;
	public GameObject PatternNew;
	public GameObject PatternOld;
	public GameObject PatternRampDown8;
	public GameObject PatternSwing16;
	public GameObject PatternPlatfStartKickOff16;
	public GameObject PatternPlatStraight8;
	public GameObject PatternPlatDown16;
	#endregion

	// Use this for initialization
	void Start () 
	{
		player = findPlayer();//in case we grab the feet first
		lastPatternBaseEnd = new Vector3(player.transform.position.x, player.GetComponent<Collider2D>().bounds.min.y, 0);
		gameScreen = new Rect(player.transform.position.x - Settings.gameBackScreenX,
		                      player.transform.position.y - Settings.gameScreenY / 2,
		                      Settings.gameScreenX,
		                      Settings.gameScreenY);
		lastPatternPlatEnd = new Vector3();

		if(levelSeed.Equals(0)) levelSeed = (int)((0.5f - Random.value) * int.MaxValue);
		Debug.Log(levelSeed);
		levelRandom = new System.Random(levelSeed);
		platformHeight = -1;
		platformLength = 0;
		platformEnding = false;
		baseLength = 0;
		genstate = GenState.ground;

		levelGenerateInitial(5);
		timer = 0;
	}
	/// <summary>
	/// Gets the top level player tagged object in the scene, aka the main player gameobject
	/// </summary>
	/// <returns>The player.</returns>
	public static GameObject findPlayer()
	{
		foreach(GameObject go in GameObject.FindGameObjectsWithTag("Player"))
		{
			if(go.layer.Equals(LayerMask.NameToLayer("Default")) && go.activeInHierarchy)
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
		if(started) //waiting on first click
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

	/// <summary>
	/// Moves the active gamescreen and increases the timer.
	/// </summary>
	private void gameUpdate()
	{
		gameScreen = new Rect(player.transform.position.x - Settings.gameBackScreenX,
		                      player.transform.position.y - Settings.gameScreenY / 2,
		                      Settings.gameScreenX,
		                      Settings.gameScreenY);

		timer += Time.deltaTime;
	}

	void OnDrawGizmos()
	{
		if(Settings.devLevel)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(
				lastPatternBaseEnd,1f);
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(
				new Vector3(lastPatternBaseEnd.x,patternHeight),0.25f);
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(
				lastPatternPlatEnd,0.5f);
		}
	}

	void OnGUI ()
	{
		if(Settings.devLevel)
		{
			GUI.Label(new Rect(Screen.width/2, 0, Screen.width, Screen.height), "Seed: "+levelSeed);
			GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "pattern height: "+patternHeight);
			GUI.Label(new Rect(0, 10, Screen.width, Screen.height), "platform height: "+platformHeight);
			GUI.Label(new Rect(0, 20, Screen.width, Screen.height), "gen state: "+genstate.ToString());
		}
	}

	/// <summary>
	/// Generates the initial x pieces of the level.
	/// </summary>
	/// <param name="length">Number of parts to generate</param>
	private void levelGenerateInitial(int length)
	{
		placePrefab(PatternStart);
		for (int i = 0; i < length; i++)
		{
			levelGenerate();
		}
	}
	private void levelGenerate()
	{
		bool patternTick = false;
		bool platTick = false;
		if(lastPatternBaseEnd.x < gameScreen.max.x) patternTick = true;
		if(lastPatternPlatEnd.x < gameScreen.max.x) platTick = true;

		levelGenerateSetGenState(patternTick, platTick);
		levelGeneratePlacement(patternTick, platTick);

	}

	private void levelGenerateSetGenState(bool patternTick, bool platTick)
	{
		//make gaps in ground
		if(genstate == GenState.ground && patternTick)
		{	//start chance is base change, reduces with each check (platform length)
			if(levelRandom.Next(Settings.levelPlatformBaseStartChance + platformLength) == 0)
			{	//reset platform height and length to starting active value
				platformLength = 0;
				genstate = GenState.platformStart;
			}
			else
			{	//increase chances
				platformLength--;
				genstate = GenState.ground;
			}
		}
		//check/end length of current platform
		if(genstate == GenState.groundPlatform && platTick)
		{	//roll for ending limit using same rules as starting one (see above)
			if(levelRandom.Next(Settings.levelPlatformBaseEndChance - platformLength) == 0)
			{
				//platformEnding = true;
				genstate = GenState.platformEnding;
			}
			else
			{
				platformLength++;
			}
		}
		if(genstate == GenState.platformStart && patternTick)
		{
			if(platformHeight != -1)
			{
				genstate = GenState.groundPlatform;
			}
		}
		if(platTick &&
		   (genstate == GenState.platformEnding && platformHeight <= Settings.levelPlatformMinHeight))
		{
			if(platformHeight <= Settings.levelPlatformMinHeight)
			{
				platformHeight = -1;
				platformLength = 0;
				genstate = GenState.ground;
			}
		}
	}
	private void levelGeneratePlacement(bool patternTick, bool platTick)
	{
		if(patternTick)
		{
			if(baseLength > Settings.levelPatternMinLength
			   && levelRandom.Next(Settings.levelPatternMaxLength - baseLength) == 0)
			{	//places an edge prefab, followed by another reversed edge prefab in a random style
				baseLength = 0;
				placeEdgePrefab(PickEdgePrefabs(false), PickEdgePrefabs(true));
			}
			else
			{	//places a prefab now. platform height&length being 0 causes a platform start
				baseLength++;
				placePrefab(PickPrefab());
			}
		}
		if(platTick && 
		   (genstate == GenState.groundPlatform || genstate == GenState.platformEnding))
		{
			placePrefabPlat(pickPlatform());
		}
	}



	private void levelGenerateOld()
	{
		//no active platform layer
		if(platformHeight.Equals(-1))
		{	//there is a gap between the last pattern and the gamescreen edge that needs to be filled
			if(lastPatternBaseEnd.x < gameScreen.max.x)
			{	//safetycheck
				if(platformLength <= 0)
				{	//start chance is base change, reduces with each check (platform length)
					if(levelRandom.Next(Settings.levelPlatformBaseStartChance + platformLength) == 0)
					{
						//reset platform height and length to starting active value
						platformHeight = 0;
						platformLength = 0;
					}
					else
					{
						//increase chances
						platformLength--;
					}
				}

				if(baseLength > Settings.levelPatternMinLength
					&& levelRandom.Next(Settings.levelPatternMaxLength - baseLength) == 0)
				{
					//places an edge prefab, followed by another reversed edge prefab in a random style
					baseLength = 0;
					placeEdgePrefab(PickEdgePrefabs(false), PickEdgePrefabs(true));
				}
				else
				{
					//places a prefab now. platform height&length being 0 causes a platform start
					baseLength++;
					placePrefab(PickPrefab());
				}
			}
		}
		//active platform layer, where the platform height is above the minimum
		if ( platformHeight >= 0
			|| (platformEnding && platformHeight > Settings.levelPlatformMinHeight))
		{
			//place base part.
			if(lastPatternBaseEnd.x < gameScreen.max.x)
			{
				if(baseLength > Settings.levelPatternMinLength
				   && levelRandom.Next(Settings.levelPatternMaxLength - baseLength) == 0)
				{
					//places an edge prefab, followed by another reversed edge prefab in a random style
					baseLength = 0;
					placeEdgePrefab(PickEdgePrefabs(false), PickEdgePrefabs(true));
				}
				else
				{
					//places a prefab now. platform height&length being 0 causes a platform start
					baseLength++;
					placePrefab(PickPrefab());
				}
			}
			//place a platform.
			if(lastPatternPlatEnd.x < gameScreen.max.x)
			{
				//roll for ending limit using same rules as starting one (see above)
				if(platformEnding || levelRandom.Next(Settings.levelPlatformBaseEndChance - platformLength) == 0)
				{
					platformEnding = true;
				}
				else
				{
					platformLength++;
				}

				//places a prefab, will pick downard only ones once platform ending is set to true
				placePrefabPlat(pickPlatform());
			}
		}
		//platform is under min height and platform is ending.
		if(platformEnding && platformHeight <= Settings.levelPlatformMinHeight)
		{
			platformHeight = -1;
			platformLength = 0;
			platformEnding = false;
		}


	}


	/// <summary>
	/// Places the designated pattern onto the end of the previous pattern, 
	/// using AttachStart and AttachEnd tags, as well as PlatformEnd Tag.
	/// </summary>
	/// <param name="pattern">Pattern.</param>
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
			Transform child = go.transform.GetChild(i);
			if(child.tag.Equals("Swing")) patternHeight = Mathf.Max(patternHeight, child.position.y);
			try
			{
				Collider2D col = child.GetComponent<Collider2D>();
				patternHeight = Mathf.Max(patternHeight, col.bounds.max.y);
			}
			catch{}
		}
		lastPatternBaseStart = attachStart.position;
		lastPatternBaseEnd = attachEnd.position;
		if(attachPlatform != null) 
		{
				lastPatternPlatEnd = attachPlatform.position;
				platformHeight = lastPatternPlatEnd.y - lastPatternBaseEnd.y;
		}
	}

	/// <summary>
	/// Places two patterns with a feasibly jumpable gap between them.
	/// </summary>
	/// <param name="pattern1">Current ending pattern</param>
	/// <param name="pattern2">New starting pattern</param>
	private void placeEdgePrefab(GameObject pattern1, GameObject pattern2)
	{
		placePrefab(pattern1);
		lastPatternBaseEnd = lastPatternBaseEnd
			+ new Vector3(levelRandom.Next(Settings.minJumpClearance, Settings.maxJumpClearance + 1),
			              levelRandom.Next(Settings.bottomJumpHeight, Settings.topJumpHeight + 1));
		placePrefab(pattern2);
	}

	/// <summary>
	/// Places the designated platform pattern onto the end of the previous platform,
	/// using platform start and platform end.
	/// </summary>
	/// <param name="platform">Platform.</param>
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
		platformHeight = lastPatternPlatEnd.y - lastPatternBaseEnd.y;

		//remove overlapping platform on larger/taller bases
		for(int i = 0; i < go.transform.childCount; i++)
		{
			try
			{
				Collider2D col = go.transform.GetChild(i).GetComponent<Collider2D>();
				if(col.bounds.max.y < patternHeight)
				{
					//Debug.Log(go.name + " height " + col.bounds.max.y + " < " + patternHeight);
					//go.transform.position += new Vector3(0,0.5f);
					//break;
					Destroy(go);
					return;
				}
			}
			catch{}
		}
	}

	/// <summary>
	/// Returns a random prefab decided using the current conditions
	/// </summary>
	/// <returns>The prefab selected</returns>
	private GameObject PickPrefab()
	{
		//Debug.Log(platformHeight + " | " + platformLength + " | " + platformEnding + " | " + patternHeight);

		//if(platformHeight.Equals(0) && platformLength.Equals(0))
		if(genstate == GenState.platformStart)
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

	private GameObject PickEdgePrefabs(bool end)
	{
		if(!end)
		{
			return PatternOld;
		}
		else
		{
			patternStyle = 0;
			return PatternNew;
		}
	}

	/// <summary>
	/// Returns a random platform prefab decided using the current conditions
	/// </summary>
	/// <returns>The prefab selected</returns>
	private GameObject pickPlatform()
	{
		//if(platformHeight > Settings.levelPlatformMaxHeight || platformEnding)
		if(platformHeight > Settings.levelPlatformMaxHeight || genstate == GenState.platformEnding)
		{
			return PatternPlatDown16;
		}
		return PatternPlatStraight8;
	}

	/// <summary>
	/// Removes all game objects that are past the left and bottom side of the game screen.
	/// </summary>
	private void cleanUpGameObjects()
	{
		foreach(GameObject go in GameObject.FindObjectsOfType<GameObject>())
		{
			if(go.activeInHierarchy && 
			   (go.transform.position.x < gameScreen.x
			   ||go.transform.position.y < gameScreen.yMin)
				)
			{
				//Debug.Log("destroying " + go.name + " at location " + go.transform.position.x + "/" + gameScreen.x);
				//Debug.Log("destroying " + go.name + " at location " + go.transform.position.y + "/" + gameScreen.y);
				Destroy(go);
			}
		}
	}
}
