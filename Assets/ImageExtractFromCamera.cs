using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public class ImageExtractFromCamera : MonoBehaviour {

	[InspectorButton("Render")]
	public bool _Render;

	public Camera Source;
	public RenderTexture Target;

	public UnityEvent_Texture OnTargetChanged;

	public void	Render()
	{
		var OldTarget = Source.targetTexture;
		Source.targetTexture = Target;
		Source.Render();
		Source.targetTexture = OldTarget;
		OnTargetChanged.Invoke(Target);
	}

}
