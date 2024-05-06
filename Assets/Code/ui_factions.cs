using EDBR.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static EDBR.Data.Data;

public class ui_factions : MonoBehaviour
{
    #region public variables
    public Transform
        system_object_parent,
        faction_object_parent;
    public GameObject
        system_object_prefab,
        faction_object_prefab;
    public Image
        colorPreview;
    public Slider
        red_slider,
        green_slider,
        blue_slider;
    public Button
        color_update,
        untrack;
    public TMP_Text
        faction_name;
    public Color
        home_system,
        green_state,
        yellow_state,
        blue_state,
        red_state,
        grey_state;
    #endregion

    #region private variables
    private string selected_faction_id = "";
    private string selected_system_id = "";
    #endregion

    private void Awake()
    {
        //Event subscriptions
        GameManager.Events.factionsUpdated.AddListener(factionsUpdated);
        GameManager.Events.factionSelected.AddListener((f) => factionSelected(f));

        InitFactionComponents();
    }

    private void Update()
    {
        color_update.interactable = (selected_faction_id.Length > 0);
        untrack.interactable = (selected_faction_id.Length > 0);
    }

    private void InitFactionComponents()
    {
        red_slider.onValueChanged.AddListener(value => { UpdateColorPreview(); });
        green_slider.onValueChanged.AddListener(value => { UpdateColorPreview(); });
        blue_slider.onValueChanged.AddListener(value => { UpdateColorPreview(); });
        color_update.onClick.AddListener(() => SetFactionColor());
        untrack.onClick.AddListener(() => Untrack());
    }

    Color new_color = new Color();
    private void UpdateColorPreview()
    {
        float r, g, b;
        r = red_slider.value;
        g = green_slider.value;
        b = blue_slider.value;

        new_color = new Color(r, g, b, 1);
        colorPreview.color = new_color;
    }

    private void factionSelected(TrackedFaction tf)
    {
        selected_faction_id = tf._id;
        colorPreview.color = tf.color;

        //Clear system list
        foreach (Transform child in system_object_parent)
        {
            Destroy(child.gameObject);
        }

        //Populate new systems
        List<_faction.FactionPresence> sorted;
        sorted = tf.faction.faction_presence.OrderBy(obj => obj.system_name).ToList();

        foreach (_faction.FactionPresence i in sorted)
        {
            string inf()
            {
                string value = "LOADING";
                if (i.influence > 0)
                    value = ((float)i.influence * 100).ToString("##.##") + "%";
                return value;
            }

            GameObject newObject = Instantiate(system_object_prefab);
            newObject.transform.Find("$SYSTEM_NAME").GetComponent<TMP_Text>().text = i.system_name;
            newObject.transform.Find("$INFLUENCE").GetComponent<TMP_Text>().text = inf();
            newObject.transform.Find("$STATE_COLOR").GetComponent<Image>().color = GetStateColor(i.state);
            newObject.transform.Find("$STATE_COLOR").transform.Find("$STATE_TEXT").GetComponent<TMP_Text>().text = i.state;
            newObject.GetComponent<Button>().onClick.AddListener(() => GameManager.Session.setSelectedSystem(i.system_id));
            newObject.transform.SetParent(system_object_parent);
            if (i.system_name == tf.faction.faction_presence[0].system_name)
            {
                ColorBlock oldColor = newObject.GetComponent<Button>().colors;
                oldColor.normalColor = home_system;
                newObject.GetComponent<Button>().colors = oldColor;
            }
        }
    }
    private void factionsUpdated()
    {
        foreach (Transform child in faction_object_parent)
        {
            Destroy(child.gameObject);
        }
        foreach (TrackedFaction tf in GameManager.Session.trackedFactions)
        {
            //Faction list
            GameObject factionListObject = Instantiate(faction_object_prefab);
            factionListObject.transform.Find("$FACTION_NAME").GetComponent<TMP_Text>().text = tf.faction.name;
            factionListObject.transform.Find("$FACTION_HOME").GetComponent<TMP_Text>().text = tf.faction.faction_presence[0].system_name;
            factionListObject.transform.Find("$FACTION_PRESENCE").GetComponent<TMP_Text>().text = ($"{tf.faction.faction_presence.Count} SYSTEMS");
            factionListObject.transform.Find("$FACTION_COLOR").GetComponent<Image>().color = tf.color;
            factionListObject.transform.SetParent(faction_object_parent);
            factionListObject.GetComponent<Button>().onClick.AddListener(() => GameManager.Session.setSelectedFaction(tf.faction.name));
        }
    }

    private void SetFactionColor()
    {
        if (selected_faction_id.Length > 0)
            GameManager.Session.setFactionColor(selected_faction_id, new_color);
    }
    private void Untrack()
    {
        GameManager.Session.UntrackFaction(selected_faction_id);

        foreach (Transform child in system_object_parent)
        {
            Destroy(child.gameObject);
        }

        selected_faction_id = "";
        selected_system_id = "";
    }

    private Color GetStateColor(string state)
    {
        state = state.ToLower();

        string[] greenStates = new string[] { "incursion", "infested" };
        string[] yellowStates = new string[] { "blight", "drought", "outbreak", "infrastructurefailure", "naturaldisaster", "revolution", "coldwar", "tradewar", "pirateattack", "terroristattack", "retreat", "unhappy", "bust", "civilunrest" };
        string[] blueStates = new string[] { "publicholiday", "technologicalleap", "historicevent", "colonisation", "expansion", "happy", "elated", "boom", "investment", "civilliberty" };
        string[] redStates = new string[] { "war", "civilwar", "elections", "despondent", "famine", "lockdown" };
        string[] greyStates = new string[] { "discontented", "none" };

        Color value = new Color();

        if (greenStates.Contains(state)) value = green_state;
        else if (yellowStates.Contains(state)) value = yellow_state;
        else if (blueStates.Contains(state)) value = blue_state;
        else if (redStates.Contains(state)) value = red_state;
        else value = grey_state;

        return value;
    }
}
