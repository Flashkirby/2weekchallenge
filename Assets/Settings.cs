using UnityEngine;
using System.Collections;

public class Settings {
	#region dev variables
	public static bool devPlayer = Application.isEditor;
	#endregion

	#region player variables
	//average ground speed of 9.68
	public static float plAccel = 0.3f;
	public static float plFriction = 0.97f;
	public static float plAirResistance = 0.999f;
	//this jump power and height combination gives a jump height
	//with a minimum of 0.99201
	//and a maximum of 2.09446
	public static float plJumpPower = 8f;
	public static float plJumpMax = 0.6f;

	public static float plGraceInputMaxTime = 0.2f;
	public static float plClimbReachXMult = 1.2f;
	public static float plClimbTimeMax = 1f;
	#endregion

	#region camera
	public static float camXOffset = 6;
	public static float camYOffset = 0;
	public static float camZOffset = -20f;
	public static float camSize = 12;
	#endregion
}
