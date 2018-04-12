using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticProcessRectTransform : MonoBehaviour {

	public RectTransform Object;

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
	int CurrentTestCount = 0;

	void SetTestParams(int Wave0,int Wave1)
	{
		var Wave0t = Wave0 / (float)Wave0Count;
		var x = Mathf.Lerp(MinMax_X.x, MinMax_X.y, Wave0t);

		var Wave1t = Wave1 / (float)Wave1Count;
		var y = Mathf.Lerp(MinMax_Y.x, MinMax_Y.y, Wave1t);
	}

	void OnEnable()
	{
		CurrentTestCount = 0;
	}

	void RunTest()
	{
		
	}
}
