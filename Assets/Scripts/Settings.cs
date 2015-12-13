using UnityEngine;
using System.Collections;

public class Settings {
	#region dev variables
	public static bool devPlayer = false;
	public static bool devLevel = Application.isEditor;
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
	public static float plClimbRatio = 0.4f; //ratio of climbing action where speed is lost. Higher value means lower you can go and still keep speed
	public static float plClimbStepUpFlatVel = 3f; //velocity after climbing
	public static float plClimbStepUpBonusVel = 2f; //bonus velocity granted from step up
	public static float plClimbReachXMult = 0.7f; //distance from ledge the grab action must be, relative to player size.x
	public static float plClimbTimeMax = 1f; //maximum time it takes to climb, if triggered from the bottom, in seconds
	public static float plFallStumbleVel = 20f; //y velocity at which the player stumbles on landing
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
	public static float plFallDeathVel = 45f; //the velocity that will kill the player
	public static float plVaultCheckDist = 1.5f; //worldspace distance checked as a radius from center to edge
	public static float plVaultTimeMax = 0.5f; //checked distance for vault triggers. This gets faster with speed
	public static float plVaultFlatVel = 8f; //minimum velocity speed after vaulting
	public static float plVaultBonusVel = 1.2f; //bonus multiplier to the velocity
	public static float plKickJumpCheckDist = 1.5f; //worldspace distance checked as a radius from center to edge
	public static float plKickJumpTimeMax = 0.7f; //checked distance for vault triggers. This gets faster with speed
	public static float plKickJumpFlatVel = 8f; //minimum velocity speed after vaulting
	public static float plKickJumpBonusVel = 1.2f; //bonus multiplier to the velocity
	public static float plSwingSnapDist = 2.5f; //distance you can grab swing from
	public static float plSwingRadius = 2f; //distance from swing player maintains
	public static float plSwingAutoReleaseAng = -35; //angle that swing releases at, only valid between -90 and 90
	public static float plUprightAngled = 0.05f;//angle by default. May be angled upwards to prevent terrain sticking
	public static float plObstacleSlowdownMult = 0.5f;//slowdown when hitting objects
	#endregion

	#region camera
	public static float camXOffset = 6;
	public static float camYOffset = 0;
	public static float camZOffset = -20f;
	public static float camSize = 7;
	#endregion

	#region level gen
	public static float gameBackScreenX = 25f;//the x behind the player we concern with
	public static float gameScreenX = 75f;//the x in front of the player we concern with. use 30f for testing in front of player
	public static float gameScreenY = 50f;//the y around the player we concern with
	public static float levelPlatformMaxHeight = 20f;//height after which platforms will be forced downwards
	public static float levelPlatformMinHeight = 6f;//the height below which a platform can end
	public static int levelPatternMinLength = 1;//Minimum length before pattern ending
	public static int levelPatternMaxLength = 5;//Max length before pattern ending (including minlength)
	public static int levelPlatformBaseStartChance = 4;//1 in x chance of starting
	public static int levelPlatformBaseEndChance = 10;//1 in x chance of starting
	public static int minJumpClearance = 8;//min space between gaps
	public static int maxJumpClearance = 12;//max space between gaps
	public static int topJumpHeight = 1;//max upwards climb
	public static int bottomJumpHeight = -4;//max downward drop
	#endregion
}
