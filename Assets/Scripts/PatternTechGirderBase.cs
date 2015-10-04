using UnityEngine;
using System.Collections;

public class PatternTechGirderBase : MonoBehaviour, PatternStyle
{
	private Random r;
	public GameObject EdgeRight;
	public GameObject EdgeLeft;
	public GameObject Straight;
	public GameObject KickOff;
	public GameObject RampDown;
	public GameObject KickSwing;

	public string ping()
	{
		return "Tech Grid Base";
	}

	public GameObject getPrefabRandom(int rand)
	{
		if(rand % 5 == 0)
		{
			return RampDown;
		}
		if(rand % 3 == 0)
		{
			return KickSwing;
		}
		return Straight;
	}
	public GameObject getPrefabStartPlatform(int rand)
	{
		return KickOff;
	}
	public GameObject getPrefabEdgeRight(int rand)
	{
		return EdgeRight;
	}
	public GameObject getPrefabEdgeLeft(int rand)
	{
		return EdgeLeft;
	}
}
