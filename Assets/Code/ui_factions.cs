using bvData;
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
        bvCore.Events.TrackedFactionsUpdated.AddListener(factionsUpdated);
        bvCore.Events.FactionSelected.AddListener(f => factionSelected(f));

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

    private void factionSelected(bvFaction faction)
    {
        
    }
    private void factionsUpdated()
    {
        
    }

    private void SetFactionColor()
    {
        
    }
    private void Untrack()
    {

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
