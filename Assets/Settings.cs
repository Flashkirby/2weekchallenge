﻿using UnityEngine;
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
	public static float plGraceInputCoolTime = 0.1f; //the cooldown period where taps are not accepted after the event
	public static float plClimbRatio = 0.6f; //ratio of climbing action where speed is lost
	public static float plClimbStepUpFlatVel = 3f; //velocity after climbing
	public static float plClimbStepUpBonusVel = 2f; //bonus velocity granted from step up
	public static float plClimbReachXMult = 0.7f; //distance from ledge the grab action must be, relative to player size.x
	public static float plClimbTimeMax = 1f; //maximum time it takes to climb, if triggered from the bottom, in seconds
	public static float plFallStumbleVel = 15f; //y velocity at which the player stumbles on landing
	public static float plFallStumbleTimeMax = 0.4f; //stumble animation - acceleration is halved during it
	public static float plFallStumbleSlowDown = 0.25f; //mult of velocity remaining after landing
	public static float plFallStumbleAccel = 0.5f; //ratio fo acceleration applied when stumbling
	public static float plFallHardVel = 30f; //y velocity at which the player stops completely on landing
	public static float plFallHardTimeMax = 0.2f; //stop animation time
	public static float plRollCheckDist = 1f; //the worldspace distance of the height a roll becomes valid
	public static float plRollShortTimeMax = 0.4f; //short "roll" animation - acceleration is increased during it
	public static float plRollShortAccel = 1f; //accel during short rolls
	public static float plRollTimeMax = 0.4f; //rolling animation - acceleration is increased during it
	public static float plRollSlowDown = 0.75f; //mult of velocity remaining after landing
	public static float plRollAccel = 0.5f; //accel during hard rolls

	#endregion

	#region camera
	public static float camXOffset = 6;
	public static float camYOffset = 0;
	public static float camZOffset = -20f;
	public static float camSize = 12;
	#endregion
}
