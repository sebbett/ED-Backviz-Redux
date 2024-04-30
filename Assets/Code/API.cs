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
                    if (GameManager.Events.requestError != null)
                    {
                        GameManager.Events.requestError(request.error);
                    }
                    Debug.Log(request.error);
                }
                else
                {
                    List<_faction> factions = new List<_faction>();

                    if (GameManager.Events.factionDataReceived != null)
                    {
                        var jsonObject = JObject.Parse(request.downloadHandler.text);
                        JArray docsArray = (JArray)jsonObject["docs"];
                        string value = docsArray[0].ToString();
                        factions.Add(Conversions.FactionFromJson(value));

                        GameManager.Events.factionDataReceived(factions.ToArray());
                    }
                }
            }
        }

        //Request for multiple factions at once
        public static IEnumerator GetFactionData(string[] faction_names)
        {
            //Elite BGS will only allow us to query for 10 factions at a time, so split the string array into pages
            //define how to actually do that
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

            //do the paging
            string[][] pages = SplitArray(faction_names, 10);

            foreach (string[] sa in pages)
            {
                string url = ($"{faction_url}");
                foreach(string s in sa)
                {
                    string query = ($"name={HttpUtility.UrlEncode(s)}&");
                    url += query;
                }

                using (UnityWebRequest request = UnityWebRequest.Get(url))
                {
                    //Make the request
                    yield return request.SendWebRequest();

                    if (request.result == UnityWebRequest.Result.ConnectionError)
                    {
                        if (GameManager.Events.requestError != null)
                        {
                            GameManager.Events.requestError(request.error);
                        }
                        Debug.Log(request.error);
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

                            GameManager.Events.factionDataReceived(factions.ToArray());
                        }
                    }
                }
            }
        }

        //Request System Data
        public static IEnumerator GetSystemData(string[] system_names)
        {
            //Elite BGS will only allow us to query for 10 factions at a time, so split the string array into pages
            //define how to actually do that
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

            //do the paging
            string[][] pages = SplitArray(system_names, 10);

            foreach (string[] sa in pages)
            {
                string url = ($"{faction_url}");
                foreach (string s in sa)
                {
                    string query = ($"name={HttpUtility.UrlEncode(s)}&");
                    url += query;
                }

                using (UnityWebRequest request = UnityWebRequest.Get(url))
                {
                    //Make the request
                    yield return request.SendWebRequest();

                    if (request.result == UnityWebRequest.Result.ConnectionError)
                    {
                        if (GameManager.Events.requestError != null)
                        {
                            GameManager.Events.requestError(request.error);
                        }
                        Debug.Log(request.error);
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

                            GameManager.Events.factionDataReceived(factions.ToArray());
                        }
                    }
                }
            }
        }
    }
}