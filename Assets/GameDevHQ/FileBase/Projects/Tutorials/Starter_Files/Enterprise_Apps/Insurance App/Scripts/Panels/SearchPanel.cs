using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchPanel : MonoBehaviour, IPanel
{
    public InputField caseNumberInput;
    public SelectPanel selectPanel;
    public void ProcessInfo()
    {
        //download list of all objects inside s3 storage (serviceappcasefilesad)

        AWSManager.Instance.GetList(caseNumberInput.text);

        //compare those to caseNumberInput by user

        //if we find a match
        //download that object

    }
}
