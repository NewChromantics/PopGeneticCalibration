using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class UnityEvent_Texture : UnityEvent<Texture> { }

public class ImageExtractFromCamera : MonoBehaviour {

	public Camera Source;
	public RenderTexture Target;

	public UnityEvent_Texture OnTargetChanged;

	public void	Render()
	{
		Source.targetTexture = Target;
		Source.Render();
		OnTargetChanged.Invoke(Target);
	}

}
