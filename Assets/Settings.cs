using UnityEngine;
using System.Collections;

public class Settings {
	#region dev variables
	public static bool devPlayer = Application.isEditor;
	#endregion

	#region player variables
	//fastest ground speed of 9.7
	public static float plAccel = 0.3f; //acceleration per update
	public static float plFriction = 0.97f; //speed lost on ground
	public static float plAirResistance = 0.999f; //speed lost in air
	//this jump power and height combination gives a jump height
	//with a minimum of 0.99201
	//and a maximum of 2.09446
	public static float plJumpPower = 8f; //max velocity given (start of the jump)
	public static float plJumpMax = 0.6f; //max jump up time in seconds

	public static float plGraceInputMaxTime = 0.2f; //the grace period where taps are accepted before the event
	public static float plClimbRatio = 0.6f; //ratio of climbing action where speed is lost
	public static float plClimbReachXMult = 0.7f; //distance from ledge the grab action must be, relative to player size.x
	public static float plClimbTimeMax = 1f; //

	#endregion

	#region camera
	public static float camXOffset = 6;
	public static float camYOffset = 0;
	public static float camZOffset = -20f;
	public static float camSize = 12;
	#endregion
}
