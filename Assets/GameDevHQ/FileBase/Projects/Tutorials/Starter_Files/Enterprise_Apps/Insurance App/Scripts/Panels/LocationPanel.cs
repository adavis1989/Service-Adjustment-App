using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LocationPanel : MonoBehaviour, IPanel
{
    public Text caseNumberText;
    public RawImage mapImg;
    public InputField mapNotes;

    public string apiKey;
    public float xCord, yCord;
    public int zoom;
    public int imgSize;
    public string url = "https://maps.googleapis.com/maps/api/staticmap?";

    public void OnEnable()
    {
        caseNumberText.text = "CASE NUMBER 000" + UIManager.Instance.activeCase.caseID;
    }

    public IEnumerator Start()
    {
        //Fetch GEO Date 
        Input.location.Start();

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1.0f);
            maxWait--;
        }

        if (maxWait < 1)
        {
            Debug.Log("Timed Out");
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to determine device location...");
        }
        else
        {
            xCord = Input.location.lastData.latitude;
            yCord = Input.location.lastData.longitude;
        }

        Input.location.Stop();

        StartCoroutine(GetLocationRoutine());

    }

    IEnumerator GetLocationRoutine()
    {
        // construct appropriate url
        url = url + "center=" + xCord + "," + yCord + "&zoom=" + zoom + "&size=" + imgSize + "x" + imgSize + "&key=" + apiKey;

        UnityWebRequest map = UnityWebRequestTexture.GetTexture(url);
        yield return map.SendWebRequest();

        if (map.error != null)
        {
            Debug.Log(map.error);
        }
        else
        {
            mapImg.texture = ((DownloadHandlerTexture)map.downloadHandler).texture;
        }
        //using (WWW map = new WWW(url))
        //{
        //    yield return map;

        //    if (map.error != null)
        //    {
        //        Debug.LogError("Map Error: " + map.error);
        //    }

        //    mapImg.texture = map.texture;

        //}
    }
    public void ProcessInfo()
    {
        if (string.IsNullOrEmpty(mapNotes.text) == false)
        {
            UIManager.Instance.activeCase.locationNotes = mapNotes.text;
        }
    }
}
