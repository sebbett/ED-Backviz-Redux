using bvData;
using EDBR.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UIManager;

public class ui_system_panel : MonoBehaviour
{
    #region public variables
    public bool isOpen = false;
    public float
        open_x,
        closed_x,
        wanted_x,
        current_x,
        speed;
    public Color
        green_state,
        yellow_state,
        blue_state,
        red_state,
        grey_state;
    public GameObject panel;
    public Transform
        faction_object_parent,
        conflict_object_parent;
    public GameObject
        faction_object_prefab,
        conflict_object_prefab;
    public Button
        conflicts,
        copy;
    public TMP_Text
        system_label,
        government_label,
        primary_econ_label,
        secondary_econ_label,
        security_label,
        state_label,
        no_conflicts_label;
    #endregion

    public UIManager manager;

    private void Awake()
    {
        bvCore.Events.SystemSelected.AddListener(systemSelected);

        InitDetailComponents();
    }

    private void Update()
    {
        UpdateSystemDetails();
    }

    private void InitDetailComponents()
    {
        conflicts.onClick.AddListener(() => ToggleConflictsPanel());
    }

    private void UpdateSystemDetails()
    {
        if (isOpen)
            wanted_x = open_x;
        else
            wanted_x = closed_x;

        current_x = Mathf.Lerp(current_x, wanted_x, speed * Time.deltaTime);

        RectTransform pos = panel.GetComponent<RectTransform>();

        Vector2 newPos = new Vector2(current_x, pos.anchoredPosition.y);

        panel.GetComponent<RectTransform>().anchoredPosition = newPos;
    }

    private void ToggleConflictsPanel()
    {
        isOpen = !isOpen;
    }

    private void CopySystemNameToClipboard(string text)
    {
        GUIUtility.systemCopyBuffer = text;
    }

    private void systemSelected(bvSystem s)
    {

    }

    private string CleanText(string input)
    {
        // Define the pattern to match the desired word
        string pattern = @"[a-zA-Z]+;";

        // Create a Regex object with the pattern
        Regex regex = new Regex(pattern);

        // Match the input against the pattern
        Match match = regex.Match(input);

        // If there's a match, return the matched value without the semicolon
        if (match.Success)
        {
            return match.Value.TrimEnd(';');
        }
        else
        {
            // If no match found, return an empty string or handle the case as needed
            return "None";
        }
    }
    public Color GetStateColor(string state)
    {
        state = state.ToLower();

        string[] greenStates = new string[] { "incursion", "infested" };
        string[] yellowStates = new string[] { "blight", "drought", "outbreak", "infrastructurefailure", "naturaldisaster", "revolution", "coldwar", "tradewar", "pirateattack", "terroristattack", "retreat", "unhappy", "bust", "civilunrest" };
        string[] blueStates = new string[] { "publicholiday", "technologicalleap", "historicevent", "colonisation", "expansion", "happy", "elated", "boom", "investment", "civilliberty" };
        string[] redStates = new string[] { "war", "civilwar", "elections", "despondent", "famine", "lockdown" };

        Color value;

        if (greenStates.Contains(state)) value = green_state;
        else if (yellowStates.Contains(state)) value = yellow_state;
        else if (blueStates.Contains(state)) value = blue_state;
        else if (redStates.Contains(state)) value = red_state;
        else value = grey_state;

        return value;
    }
}
