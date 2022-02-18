using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationPanel : MonoBehaviour, IPanel
{
    public Text caseNumberText;
    public RawImage map;
    public InputField mapNotes;

    public void OnEnable()
    {
        caseNumberText.text = "CASE NUMBER 000" + UIManager.Instance.activeCase.caseID;
    }
    public void ProcessInfo()
    {

    }
}
