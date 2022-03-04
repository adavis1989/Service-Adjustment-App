using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("The UI Manager is NULL!!");
            }

            return _instance;
        }
    }

    public Case activeCase;
    public ClientInfoPanel clientInfoPanel;
    public GameObject borderPanel;

    private void Awake()
    {
        _instance = this;
    }

    public void CreateNewCase()
    {
        activeCase = new Case();
        int randomCaseID = Random.Range(1, 1000);
        activeCase.caseID = "" + randomCaseID;

        clientInfoPanel.gameObject.SetActive(true);
        borderPanel.SetActive(true);
    }

    public void SubmitButton()
    {
        //creat a new case to save
        //populate the case data
        //open a data stream to turn that object (case) into a file
        //begin aws process

        Case aweCase = new Case();
        aweCase.caseID = activeCase.caseID;
        aweCase.name = activeCase.name;
        aweCase.date = activeCase.date;
        aweCase.locationNotes = activeCase.locationNotes;
        aweCase.photoTaken = activeCase.photoTaken;
        aweCase.photoNotes = activeCase.photoNotes;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/case#" + aweCase.caseID + ".dat");
        bf.Serialize(file, aweCase);
        file.Close();

        Debug.Log("Application Data Path: " + Application.persistentDataPath);

    }

}
