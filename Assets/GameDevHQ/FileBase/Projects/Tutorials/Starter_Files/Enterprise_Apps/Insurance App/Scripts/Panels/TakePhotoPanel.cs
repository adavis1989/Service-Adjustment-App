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
	string _imgPath;

    public void OnEnable()
    {
        caseNumberText.text = "CASE NUMBER 000" + UIManager.Instance.activeCase.caseID;
    }

    public void ProcessInfo()
    {
		//create a 2D Texture
		//apply the texture from image path
		//encode to PNG
		//store bytes to PhotoTaken (active case)
		byte[] imgData = null;

		if (string.IsNullOrEmpty(_imgPath) == false)
        {
			Texture2D img = NativeCamera.LoadImageAtPath(_imgPath, 512, false);
			imgData = img.EncodeToPNG();
		}

		UIManager.Instance.activeCase.photoTaken = imgData;
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
				Texture2D texture = NativeCamera.LoadImageAtPath(path, maxSize, false);
				if (texture == null)
				{
					Debug.Log("Couldn't load texture from " + path);
					return;
				}

				photoTaken.texture = texture;
				photoTaken.gameObject.SetActive(true);
				_imgPath = path;
			}
		}, maxSize);

		Debug.Log("Permission result: " + permission);
	}
}
