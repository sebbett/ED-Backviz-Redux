using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EDBR.Data
{
    public static class Conversions
    {
        public static system_details SystemDetailsFromJson(string json)
        {
            return JsonConvert.DeserializeObject<system_details>(json);
        }
        
        public static _system SystemFromJson(string json)
        {
            return JsonConvert.DeserializeObject<_system>(json);
        }



        public static _faction FactionFromJson(string json)
        {
            return JsonConvert.DeserializeObject<_faction>(json);
        }
    }

    //Part of the new API calls
    [Serializable]
    public class system_details
    {
        public class Conflict
        {
            [JsonProperty("type")]
            public string type { get; set; }

            [JsonProperty("status")]
            public string status { get; set; }

            [JsonProperty("faction1")]
            public Faction1 faction1 { get; set; }

            [JsonProperty("faction2")]
            public Faction2 faction2 { get; set; }
        }

        public class Faction
        {
            [JsonProperty("name")]
            public string name { get; set; }

            [JsonProperty("name_lower")]
            public string name_lower { get; set; }

            [JsonProperty("faction_id")]
            public string faction_id { get; set; }

            public string state = "none";

            public float influence = 0;
        }

        public class Faction1
        {
            [JsonProperty("faction_id")]
            public string faction_id { get; set; }

            [JsonProperty("name")]
            public string name { get; set; }

            [JsonProperty("name_lower")]
            public string name_lower { get; set; }

            [JsonProperty("station_id")]
            public object station_id { get; set; }

            [JsonProperty("stake")]
            public string stake { get; set; }

            [JsonProperty("stake_lower")]
            public string stake_lower { get; set; }

            [JsonProperty("days_won")]
            public int days_won { get; set; }
        }

        public class Faction2
        {
            [JsonProperty("faction_id")]
            public string faction_id { get; set; }

            [JsonProperty("name")]
            public string name { get; set; }

            [JsonProperty("name_lower")]
            public string name_lower { get; set; }

            [JsonProperty("station_id")]
            public object station_id { get; set; }

            [JsonProperty("stake")]
            public string stake { get; set; }

            [JsonProperty("stake_lower")]
            public string stake_lower { get; set; }

            [JsonProperty("days_won")]
            public int days_won { get; set; }
        }

        [JsonProperty("_id")]
        public string _id { get; set; }

        [JsonProperty("__v")]
        public int __v { get; set; }

        [JsonProperty("allegiance")]
        public string allegiance { get; set; }

        [JsonProperty("conflicts")]
        public List<Conflict> conflicts { get; set; }

        [JsonProperty("controlling_minor_faction")]
        public string controlling_minor_faction { get; set; }

        [JsonProperty("controlling_minor_faction_cased")]
        public string controlling_minor_faction_cased { get; set; }

        [JsonProperty("controlling_minor_faction_id")]
        public string controlling_minor_faction_id { get; set; }

        [JsonProperty("eddb_id")]
        public string eddb_id { get; set; }

        [JsonProperty("factions")]
        public List<Faction> factions { get; set; }

        [JsonProperty("government")]
        public string government { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("name_lower")]
        public string name_lower { get; set; }

        [JsonProperty("population")]
        public string population { get; set; }

        [JsonProperty("primary_economy")]
        public string primary_economy { get; set; }

        [JsonProperty("secondary_economy")]
        public string secondary_economy { get; set; }

        [JsonProperty("security")]
        public string security { get; set; }

        [JsonProperty("state")]
        public string state { get; set; }

        [JsonProperty("system_address")]
        public string system_address { get; set; }

        [JsonProperty("updated_at")]
        public string updated_at { get; set; }

        [JsonProperty("x")]
        public double x { get; set; }

        [JsonProperty("y")]
        public double y { get; set; }

        [JsonProperty("z")]
        public double z { get; set; }

        [JsonProperty("name_aliases")]
        public List<object> name_aliases { get; set; }
    }

    [Serializable]
    public struct _faction
    {
        [JsonProperty("_id")]
        public string _id { get; set; }

        [JsonProperty("name_lower")]
        public string name_lower { get; set; }

        [JsonProperty("__v")]
        public int __v { get; set; }

        [JsonProperty("allegiance")]
        public string allegiance { get; set; }

        [JsonProperty("government")]
        public string government { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("eddb_id")]
        public string eddb_id { get; set; }

        [JsonProperty("faction_presence")]
        public List<FactionPresence> faction_presence { get; set; }

        [JsonProperty("updated_at")]
        public DateTime updated_at { get; set; }

        [global::System.Serializable]
        public class FactionPresence
        {
            [JsonProperty("system_name")]
            public string system_name { get; set; }

            [JsonProperty("system_name_lower")]
            public string system_name_lower { get; set; }

            [JsonProperty("system_id")]
            public string system_id { get; set; }

            [JsonProperty("state")]
            public string state { get; set; }

            [JsonProperty("influence")]
            public double influence { get; set; }

            [JsonProperty("happiness")]
            public string happiness { get; set; }

            [JsonProperty("active_states")]
            public List<object> active_states { get; set; }

            [JsonProperty("pending_states")]
            public List<object> pending_states { get; set; }

            [JsonProperty("recovering_states")]
            public List<object> recovering_states { get; set; }

            [JsonProperty("conflicts")]
            public List<object> conflicts { get; set; }

            [JsonProperty("updated_at")]
            public DateTime updated_at { get; set; }

            [JsonProperty("system_details")]
            public system_details details { get; set; }
        }
    }

    [Serializable]
    public struct _system
    {
        [JsonProperty("_id")]
        public string id { get; set; }

        public Vector3 position
        {
            get
            {
                return new Vector3(x, y, z);
            }
        }

        [JsonProperty("__v")]
        public int __v { get; set; }

        [JsonProperty("allegiance")]
        public string allegiance { get; set; }

        [JsonProperty("conflicts")]
        public List<Conflict> conflicts { get; set; }

        [JsonProperty("controlling_minor_faction")]
        public string controlling_minor_faction { get; set; }

        [JsonProperty("controlling_minor_faction_cased")]
        public string controlling_minor_faction_cased { get; set; }

        [JsonProperty("controlling_minor_faction_id")]
        public string controlling_minor_faction_id { get; set; }

        [JsonProperty("eddb_id")]
        public string eddb_id { get; set; }

        [JsonProperty("factions")]
        public List<Faction> factions { get; set; }

        [JsonProperty("government")]
        public string government { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("name_lower")]
        public string name_lower { get; set; }

        [JsonProperty("population")]
        public long population { get; set; }

        [JsonProperty("primary_economy")]
        public string primary_economy { get; set; }

        [JsonProperty("secondary_economy")]
        public string secondary_economy { get; set; }

        [JsonProperty("security")]
        public string security { get; set; }

        [JsonProperty("state")]
        public string state { get; set; }

        [JsonProperty("system_address")]
        public string system_address { get; set; }

        [JsonProperty("updated_at")]
        public DateTime updated_at { get; set; }

        [JsonProperty("x")]
        public float x { get; set; }

        [JsonProperty("y")]
        public float y { get; set; }

        [JsonProperty("z")]
        public float z { get; set; }

        [JsonProperty("name_aliases")]
        public List<object> name_aliases { get; set; }

        public class Faction
        {
            [JsonProperty("name")]
            public string name { get; set; }

            [JsonProperty("name_lower")]
            public string name_lower { get; set; }

            [JsonProperty("faction_id")]
            public string faction_id { get; set; }

            [JsonProperty("faction_details")]
            public FactionDetails faction_details { get; set; }
        }
        public class FactionDetails
        {
            [JsonProperty("_id")]
            public string _id { get; set; }

            [JsonProperty("__v")]
            public int __v { get; set; }

            [JsonProperty("allegiance")]
            public string allegiance { get; set; }

            [JsonProperty("eddb_id")]
            public string eddb_id { get; set; }

            [JsonProperty("faction_presence")]
            public FactionPresence faction_presence { get; set; }

            public class FactionPresence
            {
                [JsonProperty("system_name")]
                public string system_name { get; set; }

                [JsonProperty("system_name_lower")]
                public string system_name_lower { get; set; }

                [JsonProperty("system_id")]
                public string system_id { get; set; }

                [JsonProperty("state")]
                public string state { get; set; }

                [JsonProperty("influence")]
                public float influence { get; set; }

                [JsonProperty("happiness")]
                public string happiness { get; set; }

                [JsonProperty("active_states")]
                public List<object> active_states { get; set; }

                [JsonProperty("pending_states")]
                public List<object> pending_states { get; set; }

                [JsonProperty("recovering_states")]
                public List<object> recovering_states { get; set; }

                [JsonProperty("conflicts")]
                public List<object> conflicts { get; set; }

                [JsonProperty("updated_at")]
                public DateTime updated_at { get; set; }
            }

            [JsonProperty("government")]
            public string government { get; set; }

            [JsonProperty("name")]
            public string name { get; set; }

            [JsonProperty("name_lower")]
            public string name_lower { get; set; }

            [JsonProperty("updated_at")]
            public string updated_at { get; set; }
        }
        public class Conflict
        {
            [JsonProperty("type")]
            public string type { get; set; }

            [JsonProperty("status")]
            public string status { get; set; }

            [JsonProperty("faction1")]
            public ConflictFaction faction1 { get; set; }

            [JsonProperty("faction2")]
            public ConflictFaction faction2 { get; set; }
        }
        public class ConflictFaction
        {
            [JsonProperty("faction_id")]
            public string faction_id { get; set; }

            [JsonProperty("name")]
            public string name { get; set; }

            [JsonProperty("name_lower")]
            public string name_lower { get; set; }

            [JsonProperty("station_id")]
            public string station_id { get; set; }

            [JsonProperty("stake")]
            public string stake { get; set; }

            [JsonProperty("stake_lower")]
            public string stake_lower { get; set; }

            [JsonProperty("days_won")]
            public int days_won { get; set; }
        }
    }

    public class Data
    {
        public class TrackedFaction
        {
            public string _id;
            public Color color;
            public _faction faction;
            internal string name;

            public TrackedFaction(_faction faction)
            {
                _id = faction._id;
                color = new Color(Random.Range(0.1f, 0.9f), Random.Range(0.1f, 0.9f), Random.Range(0.1f, 0.9f), 1.0f);
                this.faction = faction;
            }

            public TrackedFaction(_faction faction, Color color)
            {
                _id = faction._id;
                this.color = color;
                this.faction = faction;
            }

            public void setColor(Color c)
            {
                color = c;
            }
        }
    }
}