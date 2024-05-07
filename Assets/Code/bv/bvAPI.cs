using System.Collections;
using System.Diagnostics;
using System.Web;
using bvUtils;
using bvData;
using UnityEngine.Networking;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace bvAPI
{
    public static class bvAPI
    {
        const string faction_url = "https://elitebgs.app/api/ebgs/v5/factions?systemDetails=true";
        const string system_url = "https://elitebgs.app/api/ebgs/v5/systems?factionDetails=true";
        const string station_url = "https://elitebgs.app/api/ebgs/v5/stations?";
        const string tick_url = "https://elitebgs.app/api/ebgs/v5/ticks?";

        public static IEnumerator GetFactionData(string[] faction_names, APICallback callback)
        {
            //Split the input into pages of length 10
            string[][] pages = Utilities.SplitArray(faction_names, 10);

            string url = ($"{faction_url}");
            foreach (string[] page in pages)
            {
                //Compile each page into a full request URL
                foreach (string line in page)
                {
                    url += ($"&name={HttpUtility.UrlEncode(line)}");
                }

                //Make the request
                using (UnityWebRequest request = UnityWebRequest.Get(url))
                {
                    yield return request.SendWebRequest();

                    if(request.result != UnityWebRequest.Result.ConnectionError)
                    {
                        List<bvFaction> factions = new List<bvFaction>();

                        var jsonObject = JObject.Parse(request.downloadHandler.text);
                        JArray docs = (JArray)jsonObject["docs"];

                        foreach(JToken token in docs)
                        {
                            string data = token.ToString();
                            factions.Add(bvFaction.fromJson(data));
                        }

                        callback.Invoke(factions.ToArray());
                    }
                    else
                    {
                        bvCore.Events.APIError.Invoke(request.error);
                    }
                }
            }
        }
    }
}