using UnityEngine;
using System.Collections;

public class Settings : MonoBehaviour {
	public static bool devPlayer = true;

	public static float plAccel = 0.1f;
	public static float plFriction = 0.99f;
	public static float plAirResistance = 0.8f;
	public static float plJumpPower = 10f;
	public static float plAngleUprightSpeed = 0.3f;

	public static float camXOffset = 6;
	public static float camYOffset = 0;
	public static float camZOffset = -10f;
}
