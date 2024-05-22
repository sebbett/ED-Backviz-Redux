using Newtonsoft.Json;
using System;
using UnityEngine;

namespace bvData
{
    #region bvFaction
    [Serializable]
    public class bvFaction
    {
        [JsonProperty("_id")]
        public string id { get; set; }
        [JsonProperty("name_lower")]
        public string name { get; set; }
        [JsonProperty("allegiance")]
        public string allegiance { get; set; }
        [JsonProperty("government")]
        public string government { get; set; }

        public Color color { get; private set; }
        public void SetColor(Color color)
        {
            this.color = color;
        }

        //Influence, states, conflicts, etc. will be 
        //handled by the bvSystem class. FactionPresence
        //will only hold the id, name, and position of
        //the system for further querying.
        [JsonProperty("faction_presence")]
        public FactionPresence[] faction_presence;

        [Serializable]
        public class FactionPresence
        {
            //This describes the system where the faction is present
            [JsonProperty("system_id")]
            public string id { get; set; }
            [JsonProperty("system_name")]
            public string name { get; set; }
            [JsonProperty("x")]
            public double x;
            [JsonProperty("y")]
            public double y;
            [JsonProperty("z")]
            public double z;

            public Vector3 position
            {
                get
                {
                    return new Vector3((float)x, (float)y, (float)z);
                }
            }
        }

        public static bvFaction fromJson(string json)
        {
            return JsonConvert.DeserializeObject<bvFaction>(json);
        }
    }
    #endregion

    #region bvSystem
    [Serializable]
    public class bvSystem
    {
        [JsonProperty("_id")]
        public string id;
        [JsonProperty("name_lower")]
        public string name;
        [JsonProperty("x")]
        public double x;
        [JsonProperty("y")]
        public double y;
        [JsonProperty("z")]
        public double z;
        [JsonProperty("allegiance")]
        public string allegiance;
        [JsonProperty("government")]
        public string government;
        [JsonProperty("primary_economy")]
        public string primary_econ;
        [JsonProperty("secondary_economy")]
        public string secondary_econ;
        [JsonProperty("security")]
        public string security;
        [JsonProperty("state")]
        public string state;
        [JsonProperty("controlling_minor_faction_id")]
        public string controlling_faction_id;

        [JsonProperty("factions")]
        public Faction[] factions;

        public Vector3 position
        {
            get
            {
                Vector3 pos = new Vector3();
                pos.x = (float)x;
                pos.y = (float)y;
                pos.z = (float)z;

                return pos;
            }
        }

        [Serializable]
        public class Faction
        {
            [JsonProperty("_id")]
            public string id { get; set; }
            [JsonProperty("name_lower")]
            public string name { get; set; }

            public float influence
            {
                get
                {
                    return details.influence.influence;
                }
            }

            [JsonProperty("faction_details")]
            public Details details { get; set; }

            [Serializable]
            public class Details
            {
                [JsonProperty("faction_presence")]
                public Influence influence { get; set; }

                [Serializable]
                public class Influence
                {
                    [JsonProperty("influence")]
                    public float influence { get; set; }
                }
            }
        }

        public static bvSystem fromJson(string json)
        {
            return JsonConvert.DeserializeObject<bvSystem>(json);
        }

        public bvSystem(bvFaction.FactionPresence presence)
        {
            id = presence.id;
            name = presence.name;
            x = presence.x;
            y = presence.y;
            z = presence.z;
        }
    }
    #endregion
}