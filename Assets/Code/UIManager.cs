using EDBR;
using EDBR.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static EDBR.Data.Data;

public class UIManager : MonoBehaviour
{
    private void Awake()
    {
        //Event subscriptions
        GameManager.Events.requestError.AddListener((error) => Debug.LogError(error));
        GameManager.Events.systemSelected.AddListener(systemSelected);
        GameManager.Events.systemsUpdated.AddListener(systemsUpdated);
        GameManager.Events.statusUpdated.AddListener((status) => statusUpdated(status));
        GameManager.Events.factionSelected.AddListener((f) => factionSelected(f));
    }

    #region Variables
    public Header header;
    public Search search;
    public Details details;
    public Camera mainCamera;

    public Canvas
        search_canvas,
        faction_canvas,
        about_canvas;
    #endregion

    void Start()
    {
        mainCamera = Camera.main;
        InitHeaderComponents();
        InitDetailComponents();
        UpdateUI(UIState.main);
    }

    private void Update()
    {
        GetEscapeKey();
        UpdateSystemDetails();
    }

    private void GetEscapeKey()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            UpdateUI(UIState.main);
    }

    private void UpdateSystemDetails()
    {
        if (details.isOpen)
            details.wanted_x = details.open_x;
        else
            details.wanted_x = details.closed_x;

        details.current_x = Mathf.Lerp(details.current_x, details.wanted_x, details.speed * Time.deltaTime);

        RectTransform pos = details.panel.GetComponent<RectTransform>();

        Vector2 newPos = new Vector2(details.current_x, pos.anchoredPosition.y);

        details.panel.GetComponent<RectTransform>().anchoredPosition = newPos;
    }

    #region Init
    private void InitHeaderComponents()
    {
        header.search.onClick.AddListener(() => UpdateUI(UIState.search));
        header.about.onClick.AddListener(() => UpdateUI( UIState.about));
        header.factions.onClick.AddListener(() => UpdateUI(UIState.factions));
    }

    private void InitDetailComponents()
    {
        details.conflicts.onClick.AddListener(() => ToggleConflictsPanel());
    }
    #endregion

    private UIState uiState;
    private void UpdateUI(UIState state)
    {
        uiState = state;
        UIState[] cameraUnlockedStates = { UIState.main };

        search_canvas.enabled = (uiState == UIState.search);
        about_canvas.enabled = (uiState == UIState.about);
        details.canvas.enabled = ((uiState == UIState.main || uiState == UIState.factions) && selectedSystemID.Length > 0);
        faction_canvas.enabled = (uiState == UIState.factions);
        mainCamera.GetComponent<MouseOrbit>().lockControls = !(cameraUnlockedStates.Any(x => x == uiState));
    }

    private void ToggleConflictsPanel()
    {
        details.isOpen = !details.isOpen;
    }

    private void CopySystemNameToClipboard(string text)
    {
        GUIUtility.systemCopyBuffer = text;
    }

    private void statusUpdated(string status)
    {
        header.status.text = status;
    }

    private void factionSelected(TrackedFaction tf)
    {

    }

    //Populate System Details
    private string selectedSystemID = "";
    private void systemSelected(system_details s)
    {
        selectedSystemID = s._id;

        #region Search Screen
        details.system_label.text = s.name;
        details.copy.onClick.AddListener(() => CopySystemNameToClipboard(s.name));

        //Clear current faction objects
        foreach(Transform child in details.faction_object_parent)
        {
            Destroy(child.gameObject);
        }

        //Sort factions by influence
        system_details.Faction[] sortedFactions = s.factions.OrderByDescending(x => x.influence).ToArray();

        //Populate the list
        foreach(system_details.Faction f in sortedFactions)
        {
            string name = f.name;
            string inf()
            {
                string i = "LOADING";
                if(f.influence > 0)
                    i = ((float)f.influence * 100).ToString("##.##") + "%";
                return i;
            }
            string current_state = f.state;
            //string pending_state = "none";
            //Debug.Log(pending_state);

            GameObject newFO = Instantiate(details.faction_object_prefab);
            newFO.transform.Find("$FACTION_NAME").GetComponent<TMP_Text>().text = name;
            newFO.transform.Find("$INFLUENCE").GetComponent<TMP_Text>().text = inf();
            newFO.transform.Find("$STATE_COLOR").transform.Find("$STATE_TEXT").GetComponent<TMP_Text>().text = current_state;
            newFO.transform.Find("$STATE_COLOR").GetComponent<Image>().color = GetStateColor(current_state);
            newFO.GetComponent<Button>().onClick.AddListener(() => UpdateUI(UIState.search));
            //newFO.GetComponent<Button>().onClick.AddListener(() => GetFactionDetails(f.name));
            newFO.transform.SetParent(details.faction_object_parent);
        }

        string government = CleanText(s.government);
        string primary_econ = CleanText(s.primary_economy);
        string secondary_econ = CleanText(s.secondary_economy);
        string security = CleanText(s.security);
        string sys_state = CleanText(s.state);

        details.government_label.text = ($"Government: {government}");
        details.primary_econ_label.text = ($"Primary Economy: {primary_econ}");
        details.secondary_econ_label.text = ($"Secondary Economy: {secondary_econ}");
        details.security_label.text = ($"Security: {security}");
        details.state_label.text = ($"State: {sys_state}");

        //Clear conflict objects
        foreach(Transform child in details.conflict_object_parent)
        {
            Destroy(child.gameObject);
        }

        details.no_conflicts_label.gameObject.SetActive(s.conflicts.Count == 0);

        //Populate new conflicts
        foreach(system_details.Conflict c in s.conflicts)
        {
            string conflict_status = c.status;
            if (conflict_status == "")
                conflict_status = "concluded";

            GameObject newCO = Instantiate(details.conflict_object_prefab);
            newCO.transform.Find("$CONFLICT_TYPE").GetComponent<TMP_Text>().text = c.type;
            newCO.transform.Find("$CONFLICT_STATUS").GetComponent<TMP_Text>().text = conflict_status;
            newCO.transform.Find("$SCORE_A").GetComponent<TMP_Text>().text = c.faction1.days_won.ToString();
            newCO.transform.Find("$SIDE_A_NAME").GetComponent<TMP_Text>().text = c.faction1.name.ToString();
            newCO.transform.Find("$SIDE_A_STAKE").GetComponent<TMP_Text>().text = c.faction1.stake.ToString();

            newCO.transform.Find("$SCORE_B").GetComponent<TMP_Text>().text = c.faction2.days_won.ToString();
            newCO.transform.Find("$SIDE_B_NAME").GetComponent<TMP_Text>().text = c.faction2.name.ToString();
            newCO.transform.Find("$SIDE_B_STAKE").GetComponent<TMP_Text>().text = c.faction2.stake.ToString();

            newCO.transform.SetParent(details.conflict_object_parent);
        }
        #endregion
    }

    private void systemsUpdated()
    {
        if(selectedSystemID.Length > 0)
        {
            GameManager.Session.setSelectedSystem(selectedSystemID);
        }
    }

    public Color GetStateColor(string state)
    {
        state = state.ToLower();

        string[] greenStates = new string[] { "incursion", "infested" };
        string[] yellowStates = new string[] { "blight", "drought", "outbreak", "infrastructurefailure", "naturaldisaster", "revolution", "coldwar", "tradewar", "pirateattack", "terroristattack", "retreat", "unhappy", "bust", "civilunrest" };
        string[] blueStates = new string[] { "publicholiday", "technologicalleap", "historicevent", "colonisation", "expansion", "happy", "elated", "boom", "investment", "civilliberty" };
        string[] redStates = new string[] { "war", "civilwar", "elections", "despondent", "famine", "lockdown" };
        string[] greyStates = new string[] { "discontented", "none" };

        Color value = new Color();

        if (greenStates.Contains(state)) value = details.greenState;
        else if (yellowStates.Contains(state)) value = details.yellowState;
        else if (blueStates.Contains(state)) value = details.blueState;
        else if (redStates.Contains(state)) value = details.redState;
        else value = details.greyState;

        return value;
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

    [System.Serializable]
    public enum UIState
    {
        main = 0,
        search = 1,
        about = 2,
        factions = 3
    }

    #region data types
    [System.Serializable]
    public class Header
    {
        public Canvas canvas;
        public Button
            search,
            factions,
            about;
        public TMP_Text status;
    }
    [System.Serializable]
    public class Search
    {
        
    }

    [System.Serializable]
    public class Details
    {
        public bool isOpen = false;
        public float
            open_x,
            closed_x,
            wanted_x,
            current_x,
            speed;
        public Color
            greenState,
            yellowState,
            blueState,
            redState,
            greyState;
        public Canvas canvas;
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
    }
    #endregion
}
