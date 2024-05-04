using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using EDBR.Data;

namespace EDBR
{
    public static class API
    {
        //Use this to split the array into pages whose lengths fit the API limits.
        static T[][] SplitArray<T>(T[] array, int size)
        {
            int numOfArrays = (int)Math.Ceiling((double)array.Length / size);
            T[][] result = new T[numOfArrays][];

            for (int i = 0; i < numOfArrays; i++)
            {
                int elementsInThisArray = Math.Min(size, array.Length - i * size);
                result[i] = new T[elementsInThisArray];
                Array.Copy(array, i * size, result[i], 0, elementsInThisArray);
            }

            return result;
        }

        const string faction_url = "https://elitebgs.app/api/ebgs/v5/factions?systemDetails=true";
        const string system_url = "https://elitebgs.app/api/ebgs/v5/systems?factionDetails=true";
        const string station_url = "https://elitebgs.app/api/ebgs/v5/stations?";
        const string tick_url = "https://elitebgs.app/api/ebgs/v5/ticks?";

        //Request multiple factions' data
        public static IEnumerator GetFactionData(string[] faction_names)
        {
            //Elite BGS will only allow us to query for 10 factions at a time, so split the string array into pages
            string[][] pages = SplitArray(faction_names, 10);

            foreach (string[] sa in pages)
            {
                string url = ($"{faction_url}");
                foreach(string s in sa)
                {
                    string query = ($"&name={HttpUtility.UrlEncode(s)}");
                    url += query;
                }

                using (UnityWebRequest request = UnityWebRequest.Get(url))
                {
                    //Make the request
                    yield return request.SendWebRequest();

                    if (request.result == UnityWebRequest.Result.ConnectionError)
                    {
                        GameManager.Events.requestError.Invoke(request.error);
                    }
                    else
                    {
                        //Process the requested data
                        List<_faction> factions = new List<_faction>();
                        if (GameManager.Events.factionDataReceived != null)
                        {
                            var jsonObject = JObject.Parse(request.downloadHandler.text);
                            JArray docsArray = (JArray)jsonObject["docs"];

                            foreach (JToken t in docsArray)
                            {
                                string value = t.ToString();
                                factions.Add(Conversions.FactionFromJson(value));
                            }

                            GameManager.Events.factionDataReceived.Invoke(factions.ToArray());
                        }
                    }
                }
            }
        }

        //Request multiple systems' data
        public static IEnumerator GetSystemData(string[] system_names)
        {
            //Elite BGS will only allow us to query for 10 systems at a time, so split the string array into pages
            string[][] pages = SplitArray(system_names, 10);

            int p = 1;
            foreach (string[] sa in pages)
            {
                string url = ($"{system_url}");
                foreach (string s in sa)
                {
                    string query = ($"&name={HttpUtility.UrlEncode(s)}");
                    url += query;
                }

                GameManager.Events.statusUpdated.Invoke($"REQUESTING DATA {p} of {pages.Length}");

                using (UnityWebRequest request = UnityWebRequest.Get(url))
                {
                    //Make the request
                    yield return request.SendWebRequest();

                    if (request.result == UnityWebRequest.Result.ConnectionError)
                    {
                        GameManager.Events.requestError.Invoke(request.error);
                    }
                    else
                    {
                        //Process the requested data
                        List<_system> systems = new List<_system>();
                        if (GameManager.Events.factionDataReceived != null)
                        {
                            var jsonObject = JObject.Parse(request.downloadHandler.text);
                            JArray docsArray = (JArray)jsonObject["docs"];

                            foreach (JToken t in docsArray)
                            {
                                string value = t.ToString();
                                systems.Add(Conversions.SystemFromJson(value));
                            }

                            if(systems.Count > 0)
                                GameManager.Session.updateSystemFactionInfluence(systems.ToArray());

                            if (p == pages.Length)
                                GameManager.Events.statusUpdated.Invoke("READY");
                        }
                    }
                }
                p++;
            }
        }
    }
}