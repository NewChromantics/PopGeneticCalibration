using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class PopColour
{
	public static void GetHsv(this Color rgb, ref Vector3 hsv)
	{
		Color.RGBToHSV(rgb,out hsv.x, out hsv.y, out hsv.z);
	}

	public static Vector3 GetHsv(this Color rgb)
	{
		var hsv = new Vector3();
		rgb.GetHsv(ref hsv);
		return hsv;
	}

	public static float GetHSVDistance(this Color RgbSource, Color RgbMatch)
	{
		var HsvSource = RgbSource.GetHsv();
		var HsvMatch = RgbMatch.GetHsv();
		var Distance = Vector3.Distance(HsvSource, HsvMatch) / 3.0f;
		return Distance;
	}
}

public class ImageCompare : MonoBehaviour {

	public Texture GroundTruthTexture;
	public List<Vector2> GroundTruthPoints;

	[Range(0, 1)]
	public float MaxDistance = 0.1f;

	static public List<Vector2> ExtractPoints(Texture Image)
	{
		//	find the middle of the white pixels
		var Texture2 = SaveTextureToPng.GetTexture2D(Image, false);
		var Pixels = Texture2.GetPixels();
		var MatchColour = Color.white;

		System.Func<Color, Color, float> GetColourMatchScore = (Source, Match) =>
		{
			var Distance = Source.GetHSVDistance(Match);
			return 1 - Distance;
		};

		System.Func<int,int,float> GetWhiteScore = (x,y) =>
		{
			var i = x + y * Texture2.width;
			var Colour = Pixels[i];
			return GetColourMatchScore(Colour, MatchColour);
		};

		float? BestScore = null;
		int BestX=0, BestY=0;
		using ( var Progress = new ScopedProgressBar("Extracting pixel scores"))
		{
			var NotifyRate = 1;
			var maxi = Texture2.height * Texture2.width;
			for (int y = 0; y < Texture2.height;	y++ )
			{
				for (int x = 0; x < Texture2.width; x++)
				{
					var i = x + y * Texture2.width;
					if ( x == 0 )
						Progress.SetProgress("Extracting pixels", i, maxi, NotifyRate);
					var Score = GetWhiteScore(x, y);
					if (!BestScore.HasValue || Score > BestScore.Value && Score > 0 )
					{
						BestScore = Score;
						BestX = x;
						BestY = y;
					}
				}
			}
		}

		//	convert best to uv
		var Bestu = BestX / (float)Texture2.width;
		var Bestv = BestY / (float)Texture2.height;
		var Points = new List<Vector2>();
		Points.Add( new Vector2(Bestu,Bestv) );

		return Points;
	}

	public float CompareImage(Texture Match)
	{
		//	get ground truth points
		if (GroundTruthPoints == null || GroundTruthPoints.Count == 0)
			GroundTruthPoints = ExtractPoints(GroundTruthTexture);

		var MatchPoints = ExtractPoints(Match);

		var Score = GetScore(GroundTruthPoints, MatchPoints);
		Debug.Log("Score: " + Score.ToString("0.00"));
		return Score;
	}

	float GetScore(List<Vector2> PointsSrc,List<Vector2> PointsDst)
	{
		if (PointsDst.Count != PointsSrc.Count)
			throw new System.Exception("Mismatch of points (" + PointsDst.Count + " vs " + PointsSrc.Count + ")");
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
