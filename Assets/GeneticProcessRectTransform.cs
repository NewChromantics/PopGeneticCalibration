using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticProcessRectTransform : MonoBehaviour {

	public RectTransform Object;

	public ImageExtractFromCamera Setup;
	public ImageCompare Tester;

	[RangeMinMax(-380, 380)]
	public Vector2 MinMax_X;
	[Range(1, 1000)]
	public int TestCount_X;

	[RangeMinMax(-380, 380)]
	public Vector2 MinMax_Y;
	[Range(1, 1000)]
	public int TestCount_Y;

	int Wave0Count { get { return TestCount_X; } }
	int Wave1Count { get { return TestCount_Y; } }
	int TotalTestCount { get { return Wave0Count * Wave1Count; } }

	[Range(0, 1)]
	public float MinScore = 0.01f;
	int? CurrentTestIndex = null;
	int? BestTestIndex = null;
	float BestTestScore;

	void SetTestParams(int Wave0,int Wave1)
	{
		var Wave0t = Wave0 / (float)Wave0Count;
		var x = Mathf.Lerp(MinMax_X.x, MinMax_X.y, Wave0t);

		var Wave1t = Wave1 / (float)Wave1Count;
		var y = Mathf.Lerp(MinMax_Y.x, MinMax_Y.y, Wave1t);

		var z = Object.localPosition.z;
		var xyz = new Vector3(x, y, z);
		Object.localPosition = xyz;
	}

	void OnEnable()
	{
		CurrentTestIndex = null;
	}

	void Update()
	{
		if (!CurrentTestIndex.HasValue)
			CurrentTestIndex = 0;
		else
			CurrentTestIndex++;

		try
		{
			var ThisScore = RunTest(CurrentTestIndex.Value);
			if (ThisScore > MinScore)
			{
				if (!BestTestIndex.HasValue)
				{
					BestTestIndex = CurrentTestIndex.Value;
					BestTestScore = ThisScore;
				}
				else if (ThisScore > BestTestScore)
				{
					BestTestIndex = CurrentTestIndex.Value;
					BestTestScore = ThisScore;
				}
			}
		}
		catch(System.Exception e)
		{
			Debug.LogException(e);
			//	error running test, show best
			if ( BestTestIndex.HasValue )
			{
				int Wave0, Wave1;
				GetWaveParams(BestTestIndex.Value, out Wave0, out Wave1 );
				SetTestParams(Wave0, Wave1);
			}
			this.enabled = false;
		}
	}

	void GetWaveParams(int TestIndex,out int Wave0,out int Wave1)
	{
		Wave0 = TestIndex % Wave0Count;
		Wave1 = TestIndex / Wave0Count;
	}

	float RunTest(int TestIndex)
	{
		if (CurrentTestIndex >= TotalTestCount)
			throw new System.Exception("Done all tests");

		int Wave0, Wave1;
		GetWaveParams(TestIndex, out Wave0, out Wave1);
		SetTestParams( Wave0, Wave1 );
		var Rendered = Setup.Render();
		var Score = Tester.CompareImage(Rendered);
		return Score;
	}
}
