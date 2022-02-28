using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TakePhotoPanel : MonoBehaviour, IPanel
{
    public RawImage photoTaken;
    public InputField photoNotes;
    public Text caseNumberText;

    public void OnEnable()
    {
        caseNumberText.text = "CASE NUMBER 000" + UIManager.Instance.activeCase.caseID;
    }

    public void ProcessInfo()
    {
    }
}
