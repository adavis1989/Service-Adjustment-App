using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using System.IO;
using System;
using Amazon.S3.Util;
using System.Collections.Generic;
using Amazon.CognitoIdentity;
using Amazon;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

public class AWSManager : MonoBehaviour
{
    private static AWSManager _instance;
    public static AWSManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log("AWS Manager is NULL!!");
            }
            return _instance;
        }
    }


    public string S3Region = RegionEndpoint.USEast1.SystemName;
    private RegionEndpoint _S3Region
    {
        get { return RegionEndpoint.GetBySystemName(S3Region); }
    }

    private AmazonS3Client _s3Client;

    public AmazonS3Client S3Client
    {
        get
        {
            if (_s3Client == null)
            {
                _s3Client = new AmazonS3Client(new CognitoAWSCredentials(
                "us-east-1:a467aab7-109c-40ff-ac86-506dc3dca1eb", // Identity Pool ID
                RegionEndpoint.USEast1 // Region
                ), _S3Region);
            }

            return _s3Client;
        }
    }

    private void Awake()
    {
        _instance = this;

        UnityInitializer.AttachToGameObject(this.gameObject);
        AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;
    } 

    public void UploadToS3(string path, string caseID)
    {
        FileStream stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

        PostObjectRequest request = new PostObjectRequest()
        {
            Bucket = "serviceappcasefilesad",
            Key = "Case#" + caseID,
            InputStream = stream,
            CannedACL = S3CannedACL.Private,
            Region = _S3Region
        };

        S3Client.PostObjectAsync(request, (responsObj) =>
        {
            if (responsObj.Exception == null)
            {
                Debug.Log("Succeessfully posted to bucket");
                SceneManager.LoadScene(0);
            }
            else
            {
                Debug.Log("Exception occured during uploading: " + responsObj.Exception);
            }
        });
    }

    public void GetList(string caseNumber)
    {
        string target = "Case#" + caseNumber;

        var request = new ListObjectsRequest()
        {
            BucketName = "serviceappcasefilesad",
        };

        S3Client.ListObjectsAsync(request, (responseObj) =>
        {
            if (responseObj.Exception == null)
            {
                bool caseFound = responseObj.Response.S3Objects.Any(obj => obj.Key == target);
                
                if (caseFound == true)
                {
                    Debug.Log("Case Found!");
                    S3Client.GetObjectAsync("serviceappcasefilesad", target, (responseObject) =>
                    {
                        //read the data and apply it to a case object to be used

                        //check if response stream is null
                        if (responseObject.Response.ResponseStream != null)
                        {
                            //byte array to store data from file
                            byte[] data = null;

                            //use streamreader to read response data
                            using (StreamReader reader = new StreamReader(responseObject.Response.ResponseStream))
                            {
                                //access a memory stream
                                using (MemoryStream memory = new MemoryStream())
                                {
                                    //populate data byte array with memstream data
                                    var buffer = new byte[512];
                                    var bytesRead = default(int);

                                    while ((bytesRead = reader.BaseStream.Read(buffer, 0, buffer.Length)) > 0)
                                    {
                                        memory.Write(buffer, 0, bytesRead);
                                    }
                                    data = memory.ToArray();
                                }
                            }
                            //convert those bytes to a Case(Object)
                            using (MemoryStream memory = new MemoryStream(data))
                            {
                                BinaryFormatter bf = new BinaryFormatter();
                                Case downloadedCase = (Case)bf.Deserialize(memory);
                                Debug.Log("Downloaded Case Name: " + downloadedCase.name);
                                UIManager.Instance.activeCase = downloadedCase;
                            }

                        }




                    });
                }

            }
            else
            {
                Debug.Log("Error getting list of items from S3: " + responseObj.Exception);
            };

        });
    }





    //public void GetList(string caseNumber, Action onComplete = null)
    //{
    //    string target = "Case#" + caseNumber;

    //    Debug.Log("AWSManager::GetList()");

    //    var request = new ListObjectsRequest()
    //    {
    //        BucketName = "serviceappcasefilesad",
    //    };

    //    S3Client.ListObjectsAsync(request, (responseObj) =>
    //    {
    //        if (responseObj.Exception == null)
    //        {
    //            bool caseFound = responseObj.Response.S3Objects.Any(obj => obj.Key == target);

    //            if (caseFound == true)
    //            {
    //                Debug.Log("Case Found!");
    //                S3Client.GetObjectAsync("serviceappcasefilesad", target, (responseObject) =>
    //                {
    //                    read the data and apply it to a case (object)to be used

    //                    check if response stream is null
    //                    if (responseObject.Response.ResponseStream != null)
    //                    {
    //                        byte array to store data from file
    //                        byte[] data = null;

    //                        use streamreader to read response data
    //                        using (StreamReader reader = new StreamReader(responseObject.Response.ResponseStream))
    //                        {
    //                            access a memory stream
    //                            using (MemoryStream memory = new MemoryStream())
    //                            {
    //                                populate data byte array with memstream date
    //                                var buffer = new byte[512];
    //                                var bytesRead = default(int);

    //                                while ((bytesRead = reader.BaseStream.Read(buffer, 0, buffer.Length)) > 0)
    //                                {
    //                                    memory.Write(buffer, 0, bytesRead);
    //                                }
    //                                data = memory.ToArray();
    //                            }
    //                        }
    //                        convert those bytes to a Case(Object)
    //                        using (MemoryStream memory = new MemoryStream(data))
    //                        {
    //                            BinaryFormatter bf = new BinaryFormatter();
    //                            Case downloadedCase = (Case)bf.Deserialize(memory);
    //                            Debug.Log("Downloaded Case Name: " + downloadedCase.name);
    //                            UIManager.Instance.activeCase = downloadedCase;

    //                            if (onComplete != null)
    //                            {
    //                                onComplete();
    //                            }
    //                        }
    //                    }
    //                });
    //            }
    //            else
    //            {
    //                Debug.Log("Case Not Found!");
    //            }
    //        }
    //        else
    //        {
    //            Debug.Log("Error getting List of Items from S3: " + responseObj.Exception);
    //        }
    //    });

    //}
}
