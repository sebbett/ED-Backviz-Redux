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
        GameManager.Events.factionDataReceived.AddListener(factionDataReceived);
        GameManager.Events.requestError.AddListener((error) => Debug.LogError(error));
        GameManager.Events.factionsUpdated.AddListener(factionsUpdated);
        GameManager.Events.systemSelected.AddListener(systemSelected);
        GameManager.Events.systemsUpdated.AddListener(systemsUpdated);
        GameManager.Events.statusUpdated.AddListener((status) => statusUpdated(status));
        GameManager.Events.factionSelected.AddListener((f) => factionSelected(f));
    }

    #region Variables
    public Header header;
    public Search search;
    public About about;
    public Details details;
    public Factions factions;
    public Camera mainCamera;
    #endregion

    void Start()
    {
        mainCamera = Camera.main;
        InitHeaderComponents();
        InitSearchComponents();
        InitFactionComponents();
        InitAboutComponents();
        InitDetailComponents();
        UpdateUI(UIState.main);
    }

    private void Update()
    {
        GetEscapeKey();
        UpdateSystemDetails();

        UpdateFactionScreen();
    }

    private void UpdateFactionScreen()
    {
        factions.color_update.interactable = (selectedFactionID.Length > 0);
        factions.untrack.interactable = (selectedFactionID.Length > 0);
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

        details.current_x = Mathf.Lerp(details.current_x, details.wanted_x, details.speed);

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
    private void InitSearchComponents()
    {
        search.close.onClick.AddListener(() => UpdateUI(UIState.main));
        search.search_bar.onValueChanged.AddListener((value) => PerformLocalFactionSearch(value));
    }
    private void InitAboutComponents()
    {
        about.button_close.onClick.AddListener(() => UpdateUI(UIState.main));
        about.button_discord.onClick.AddListener(() => Application.OpenURL("https://discord.gg/TzmJfrPFK2"));
        about.button_github.onClick.AddListener(() => Application.OpenURL("https://github.com/sebbett/ED-Backviz-Redux"));
        about.button_kofi.onClick.AddListener(() => Application.OpenURL("https://ko-fi.com/sebinspace"));
    }
    private void InitDetailComponents()
    {
        details.conflicts.onClick.AddListener(() => ToggleConflictsPanel());
    }
    private void InitFactionComponents()
    {
        factions.red_slider.onValueChanged.AddListener(value => { UpdateColorPreview(); });
        factions.green_slider.onValueChanged.AddListener(value => { UpdateColorPreview(); });
        factions.blue_slider.onValueChanged.AddListener(value => { UpdateColorPreview(); });
        factions.color_update.onClick.AddListener(()=>SetFactionColor());
        factions.untrack.onClick.AddListener(() => Untrack());
    }

    private void Untrack()
    {
        GameManager.Session.UntrackFaction(selectedFactionID);

        foreach (Transform child in factions.system_object_parent)
        {
            Destroy(child.gameObject);
        }

        selectedFactionID = "";
        selectedSystemID = "";
    }

    Color new_color = Color.cyan;
    private void UpdateColorPreview()
    {
        float r, g, b;
        r = factions.red_slider.value;
        g = factions.green_slider.value;
        b = factions.blue_slider.value;

        new_color = new Color(r, g, b, 1);
        factions.colorPreview.color = new_color;
    }
    private void SetFactionColor()
    {
        if (selectedFactionID.Length > 0)
            GameManager.Session.setFactionColor(selectedFactionID, new_color);
    }
    #endregion

    private UIState uiState;
    private void UpdateUI(UIState state)
    {
        uiState = state;
        UIState[] cameraUnlockedStates = { UIState.main };

        search.canvas.enabled = (uiState == UIState.search);
        about.canvas.enabled = (uiState == UIState.about);
        details.canvas.enabled = (uiState == UIState.main && selectedSystemID.Length > 0);
        factions.canvas.enabled = (uiState == UIState.factions);
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

    //Populate search window faction details
    private void factionDataReceived(_faction[] data)
    {
        switch (uiState)
        {
            case UIState.search:
                _faction faction = data[0];
                search.faction_details.SetActive(true);
                search.spinner.SetActive(false);

                search.details_name.text = faction.name;
                search.details_home.text = ($"Home: {faction.faction_presence[0].system_name}");
                search.details_allegiance.text = ($"Allegiance: {faction.allegiance}");
                search.details_government.text = ($"Government: {faction.government}");
                search.details_presence.text = ($"Presence: {faction.faction_presence.Count()} systems(s)");

                search.track.onClick.RemoveAllListeners();
                search.track.onClick.AddListener(() => GameManager.Session.addTrackedFactions(new _faction[] {faction}));
                search.track.onClick.AddListener(() => search.track.interactable = false);

                search.track.interactable = !GameManager.Session.isTrackingFaction(faction);

                search.logo_alliance.enabled = faction.allegiance == "alliance";
                search.logo_empire.enabled = faction.allegiance == "empire";
                search.logo_federation.enabled = faction.allegiance == "federation";
                search.logo_independent.enabled = faction.allegiance == "independent";

                break;

            case UIState.main:
                break;
        }
    }

    //Populate tracked factions
    private void factionsUpdated()
    {
        //Clear current list
        foreach (Transform child in search.listParent)
        {
            Destroy(child.gameObject);
        }

        foreach(Transform child in factions.faction_object_parent)
        {
            Destroy(child.gameObject);
        }

        //Populate new data
        foreach (TrackedFaction tf in GameManager.Session.trackedFactions)
        {
            //Search list
            GameObject searchFactionObject = Instantiate(search.faction_object_prefab);
            searchFactionObject.transform.Find("$FACTION_NAME").GetComponent<TMP_Text>().text = tf.faction.name;
            searchFactionObject.transform.Find("$FACTION_HOME").GetComponent<TMP_Text>().text = tf.faction.faction_presence[0].system_name;
            searchFactionObject.transform.Find("$FACTION_PRESENCE").GetComponent<TMP_Text>().text = ($"{tf.faction.faction_presence.Count} SYSTEMS");
            searchFactionObject.transform.Find("$FACTION_COLOR").GetComponent<Image>().color = tf.color;
            searchFactionObject.transform.SetParent(search.listParent);
            searchFactionObject.GetComponent<Button>().onClick.AddListener(() => GameManager.Session.setSelectedFaction(tf.faction.name));

            //Faction list
            GameObject factionListObject = Instantiate(factions.faction_object_prefab);
            factionListObject.transform.Find("$FACTION_NAME").GetComponent<TMP_Text>().text = tf.faction.name;
            factionListObject.transform.Find("$FACTION_HOME").GetComponent<TMP_Text>().text = tf.faction.faction_presence[0].system_name;
            factionListObject.transform.Find("$FACTION_PRESENCE").GetComponent<TMP_Text>().text = ($"{tf.faction.faction_presence.Count} SYSTEMS");
            factionListObject.transform.Find("$FACTION_COLOR").GetComponent<Image>().color = tf.color;
            factionListObject.transform.SetParent(factions.faction_object_parent);
            factionListObject.GetComponent<Button>().onClick.AddListener(() => GameManager.Session.setSelectedFaction(tf.faction.name));
            
        }
    }

    private string selectedFactionID = "";
    private void factionSelected(TrackedFaction tf)
    {
        selectedFactionID = tf._id;

        factions.faction_name.text = tf.faction.name.ToString();
        factions.colorPreview.color = tf.color;

        foreach(Transform child in factions.system_object_parent)
        {
            Destroy(child.gameObject);
        }

        List<_faction.FactionPresence> sorted = tf.faction.faction_presence.OrderBy(obj => obj.system_name).ToList();

        foreach (_faction.FactionPresence i in sorted)
        {
            string inf()
            {
                string value = "LOADING";
                if (i.influence > 0)
                    value = ((float)i.influence * 100).ToString("##.##") + "%";
                return value;
            }

            GameObject newObject = Instantiate(factions.system_object_prefab);
            newObject.transform.Find("$SYSTEM_NAME").GetComponent<TMP_Text>().text = i.system_name;
            newObject.transform.Find("$INFLUENCE").GetComponent<TMP_Text>().text = inf();
            newObject.transform.Find("$STATE_COLOR").GetComponent<Image>().color = GetStateColor(i.state);
            newObject.transform.Find("$STATE_COLOR").transform.Find("$STATE_TEXT").GetComponent<TMP_Text>().text = i.state;
            newObject.GetComponent<Button>().onClick.AddListener(() => GameManager.Session.setSelectedSystem(i.system_id));
            newObject.transform.SetParent(factions.system_object_parent);
            if (i.system_name == tf.faction.faction_presence[0].system_name)
            {
                ColorBlock oldColor = newObject.GetComponent<Button>().colors;
                oldColor.normalColor = factions.home_system;
                newObject.GetComponent<Button>().colors = oldColor;
                newObject.transform.Find("$SYSTEM_NAME").GetComponent<TMP_Text>().color = Color.black;
                newObject.transform.Find("$INFLUENCE").GetComponent<TMP_Text>().color = Color.black;
            }
        }
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
            newFO.GetComponent<Button>().onClick.AddListener(() => GetFactionDetails(f.name));
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

    #region Faction search functionality
    private void PerformLocalFactionSearch(string value)
    {
        string[] matches = DB.Factions.FindPartialMatches(value);
        UpdateSearchResults(matches);
    }

    private void UpdateSearchResults(string[] results)
    {
        //Clear current search results
        foreach (Transform child in search.search_results.transform)
        {
            Destroy(child.gameObject);
        }
        //Add new results
        foreach (string r in results)
        {
            GameObject newSearchResult = Instantiate(search.search_result_prefab);
            newSearchResult.GetComponentInChildren<TMP_Text>().text = r;
            newSearchResult.GetComponent<Button>().onClick.AddListener(() => GetFactionDetails(r));
            newSearchResult.transform.SetParent(search.search_results.transform);
        }

        search.no_matches_found.enabled = !(results.Length > 0);
    }

    private void GetFactionDetails(string r)
    {
        search.faction_details.SetActive(false);
        search.spinner.SetActive(true);
        StartCoroutine(API.GetFactionData(new string[] {r}));
    }
    #endregion

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
        public Canvas canvas;
        public Button close;
        public TMP_InputField search_bar;
        public TMP_Text no_matches_found;
        public Image
            logo_alliance,
            logo_empire,
            logo_federation,
            logo_independent;

        public GameObject
            faction_details,
            search_results,
            search_result_prefab,
            spinner;

        //faction detail objects
        public TMP_Text
            details_name,
            details_home,
            details_allegiance,
            details_government,
            details_presence;

        public Button track;

        public GameObject faction_object_prefab;
        public Transform listParent;
    }

    [System.Serializable]
    public class About
    {
        public Canvas canvas;
        public Button
            button_close,
            button_discord,
            button_github,
            button_kofi;
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

    [Serializable]
    public class Factions
    {
        public Canvas canvas;
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
            home_system;
    }
    #endregion
}
