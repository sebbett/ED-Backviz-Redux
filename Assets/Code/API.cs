using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EDBR
{
    public static class API
    {
        const string faction_url = "https://elitebgs.app/api/ebgs/v5/factions?";
        const string system_url = "https://elitebgs.app/api/ebgs/v5/systems?";
        const string station_url = "https://elitebgs.app/api/ebgs/v5/stations?";
        const string tick_url = "https://elitebgs.app/api/ebgs/v5/ticks?";

        public static IEnumerator GetFactionData(string faction_name)
        {
            string url = $"{faction_url}name={HttpUtility.UrlEncode(faction_name)}";
            Debug.Log($"REQUESTING {url}");

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();

                if(request.result == UnityWebRequest.Result.ConnectionError)
                {
                    if (GameManager.Events.factionDataError != null)
                    {
                        GameManager.Events.factionDataError(request.error);
                    }
                    Debug.Log(request.error);
                }
                else
                {
                    if (GameManager.Events.factionDataReceived != null)
                    {
                        var jsonObject = JObject.Parse(request.downloadHandler.text);
                        JArray docsArray = (JArray)jsonObject["docs"];
                        string value = docsArray[0].ToString();

                        GameManager.Events.factionDataReceived(value);
                    }
                }
            }
        }
    }
}