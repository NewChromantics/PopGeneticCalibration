using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageCompare : MonoBehaviour {

	public Texture GroundTruthTexture;
	public List<Vector2> GroundTruthPoints;

	[Range(0, 1000)]
	public float MaxDistance = 100;

	static public List<Vector2> ExtractPoints(Texture Image)
	{
		//	find the middle of the white pixels
		return null;
	}

	public void CompareImage(Texture Match)
	{
		//	get ground truth points
		if (GroundTruthPoints == null)
			GroundTruthPoints = ExtractPoints(GroundTruthTexture);

		var MatchPoints = ExtractPoints(Match);

		var Score = GetScore(GroundTruthPoints, MatchPoints);
		Debug.Log("Score: " + Score.ToString("0.00"));
	}

	float GetScore(List<Vector2> PointsSrc,List<Vector2> PointsDst)
	{
		if (PointsDst.Count != PointsSrc.Count)
			throw new System.Exception("Mismatch of points");
		if (PointsDst.Count == 0)
			throw new System.Exception("no points!");

		//	score each point
		System.Func<Vector2,Vector2,float> GetPointScore = (a,b) =>
		{
			var Distance = Vector2.Distance(a, b);
			Distance = Mathf.Min(Distance, MaxDistance);
			Distance /= MaxDistance;
			return 1 - Distance;
		};

		//	get average score
		float Score = 0;
		for (int i = 0; i < PointsDst.Count;	i++ )
		{
			Score += GetPointScore(PointsDst[i], PointsSrc[i]);
		}
		Score /= (float)PointsDst.Count;

		return Score;
	}




}
