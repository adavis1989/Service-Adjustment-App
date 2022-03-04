using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TakePhotoPanel : MonoBehaviour, IPanel
{
    public RawImage photoTaken;
    public InputField photoNotes;
    public Text caseNumberText;
	public GameObject overViewPanel;

    public void OnEnable()
    {
        caseNumberText.text = "CASE NUMBER 000" + UIManager.Instance.activeCase.caseID;
    }

    public void ProcessInfo()
    {
		UIManager.Instance.activeCase.photoTaken = photoTaken.texture;
		UIManager.Instance.activeCase.photoNotes = photoNotes.text;
		overViewPanel.SetActive(true);
    }

	public void TakePictureButton()
    {
		TakePicture(512);
    }
    private void TakePicture(int maxSize)
	{
		NativeCamera.Permission permission = NativeCamera.TakePicture((path) =>
		{
			Debug.Log("Image path: " + path);
			if (path != null)
			{
				// Create a Texture2D from the captured image
				Texture2D texture = NativeCamera.LoadImageAtPath(path, maxSize);
				if (texture == null)
				{
					Debug.Log("Couldn't load texture from " + path);
					return;
				}

				photoTaken.texture = texture;
				photoTaken.gameObject.SetActive(true);
			}
		}, maxSize);

		Debug.Log("Permission result: " + permission);
	}
}
