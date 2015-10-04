using UnityEngine;
using System.Collections;

public interface PatternStyle
{
	string ping();
	GameObject getPrefabRandom(int rand);// { return null;}
	GameObject getPrefabStartPlatform(int rand);// { return null;}
	GameObject getPrefabEdgeRight(int rand);// { return null;}
	GameObject getPrefabEdgeLeft(int rand);// { return null;}
}
