using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
public class ApiHandler : MonoBehaviour
{

  
    

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitForServerResponse());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator WaitForServerResponse()
    {

        WWWForm form = new WWWForm();
        form.AddField("game_id", "51000");
        form.AddField("user_id", "645569424");
        form.AddField("room_id", "12989813");
        form.AddField("timestamp", "1660605906");
        form.AddField("hash", "b965bbb376cc4498added7e280e2e410");
        

      
        bool apiRequestSuccessful = false;

        using (UnityWebRequest req = UnityWebRequest.Post("https://mindplays.com/api/v1/multiplayer/info_user?", form))
        {

            yield return req.SendWebRequest();

            string[] pages = req.url.Split('/');
            int page = pages.Length - 1;


            switch (req.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + req.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + req.error + " " + name);
                    break;
                case UnityWebRequest.Result.Success:
                    apiRequestSuccessful = true;
                    print("working fine = " + req);
                 
                    break;
            }

         
            yield return new WaitForSeconds(1);

           

        }

    }



}
