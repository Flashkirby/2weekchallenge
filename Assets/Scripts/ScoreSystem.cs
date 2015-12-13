using UnityEngine;
using System.Collections;

public class ScoreSystem 
{
	private int score;
	public int Score{get{return score;}}

	private float distanceTrack;

	public ScoreSystem()
	{
		resetScore(0);
	}

	public void resetScore(float posX)
	{
		score = 0;
		distanceTrack = posX;
	}

	public void incrementScore(float distanceTravelled)
	{
		score += (int)(Mathf.Log10(
			Mathf.Abs(distanceTravelled) + 1
			) * 100);
	}

	public void calculateScore(float currentDistance)
	{
		/*
		if(distanceTrack != startDistance)
		{
			distanceTrack = currentDistance;
			return;
		}
		*/
		incrementScore(currentDistance - distanceTrack);
		distanceTrack = currentDistance;
	}
}
