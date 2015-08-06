using UnityEngine;
using System.Collections;

public class GlobalInput : MonoBehaviour {
	/// <summary>
	/// Returns true if there is a tap or click input
	/// </summary>
	public static bool Click()
	{
		if(Input.touchCount > 0)
		{
			return true;
		}
		if(Input.GetMouseButton(0))
		{
			return true;
		}
		return false;
	}
	/// <summary>
	/// Returns true on the frame that there is a tap or click input
	/// </summary>
	public static bool ClickDown()
	{
		if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
		{
			return true;
		}
		if(Input.GetMouseButtonDown(0))
		{
			return true;
		}
		return false;
	}
	/// <summary>
	/// Returns true on the frame that a tap or click input finishes
	/// </summary>
	public static bool ClickUp()
	{
		if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
		{
			return true;
		}
		if(Input.GetMouseButtonUp(0))
		{
			return true;
		}
		return false;
	}
}
