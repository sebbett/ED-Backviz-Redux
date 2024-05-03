using EDBR;
using EDBR.Data;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private void Awake()
    {
        //Event subscriptions
        GameManager.Events.factionDataReceived.AddListener(factionDataReceived);
        GameManager.Events.requestError.AddListener((error) => Debug.LogError(error));
        GameManager.Events.factionsUpdated.AddListener(factionsUpdated);
        GameManager.Events.systemSelected.AddListener(systemSelected);
    }

    



    #region Variables
    public Header header;
    public Search search;
    public About about;
    public Details details;
    public Camera mainCamera;
    #endregion

    void Start()
    {
        mainCamera = Camera.main;
        InitHeaderComponents();
        InitSearchComponents();
        InitAboutComponents();
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
    #endregion

    private UIState uiState;
    private void UpdateUI(UIState state)
    {
        uiState = state;
        UIState[] cameraUnlockedStates = { UIState.main };

        search.canvas.enabled = (uiState == UIState.search);
        about.canvas.enabled = (uiState == UIState.about);
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
                search.track.onClick.AddListener(() => GameManager.Session.addTrackedFaction(faction));
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

        //Populate new data
        foreach (TrackedFaction tf in GameManager.Session.trackedFactions)
        {
            GameObject newObject = Instantiate(search.trackedFactionPrefab);
            newObject.transform.Find("$FACTION_NAME").GetComponent<TMP_Text>().text = tf.faction.name;
            newObject.transform.Find("$FACTION_HOME").GetComponent<TMP_Text>().text = tf.faction.faction_presence[0].system_name;
            newObject.transform.Find("$FACTION_PRESENCE").GetComponent<TMP_Text>().text = ($"{tf.faction.faction_presence.Count} SYSTEMS");
            newObject.transform.Find("$FACTION_COLOR").GetComponent<Image>().color = tf.color;
            newObject.transform.SetParent(search.listParent);
            newObject.GetComponent<Button>().onClick.AddListener(() => GameManager.Session.setSelectedFaction(tf.faction.name));
        }
    }

    //Populate System Details
    private void systemSelected(_system s)
    {
        UpdateUI(UIState.main);
        details.canvas.enabled = true;
        details.system_label.text = s.name;
        details.copy.onClick.AddListener(() => CopySystemNameToClipboard(s.name));

        //Clear current faction objects
        foreach(Transform child in details.faction_object_parent)
        {
            Destroy(child.gameObject);
        }

        //Populate the list
        foreach(_system.Faction f in s.factions)
        {
            string name = f.name;
            string inf = GameManager.Session.getFactionInfluence(s.id, f.faction_id);
            string state = GameManager.Session.getFactionState(s.id, f.faction_id);

            GameObject newFO = Instantiate(details.faction_object_prefab);
            newFO.transform.Find("$FACTION_NAME").GetComponent<TMP_Text>().text = name;
            newFO.transform.Find("$INFLUENCE").GetComponent<TMP_Text>().text = inf;
            newFO.transform.Find("$STATE_COLOR").transform.Find("$STATE_TEXT").GetComponent<TMP_Text>().text = state;
            newFO.transform.Find("$STATE_COLOR").GetComponent<Image>().color = GetStateColor(state);
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
        foreach(_system.Conflict c in s.conflicts)
        {
            GameObject newCO = Instantiate(details.conflict_object_prefab);
            newCO.transform.Find("$CONFLICT_TYPE").GetComponent<TMP_Text>().text = c.type;
            newCO.transform.Find("$CONFLICT_STATUS").GetComponent<TMP_Text>().text = c.status;
            newCO.transform.Find("$SCORE_A").GetComponent<TMP_Text>().text = c.faction1.days_won.ToString();
            newCO.transform.Find("$SIDE_A_NAME").GetComponent<TMP_Text>().text = c.faction1.name.ToString();
            newCO.transform.Find("$SIDE_A_STAKE").GetComponent<TMP_Text>().text = c.faction1.stake.ToString();

            newCO.transform.Find("$SCORE_B").GetComponent<TMP_Text>().text = c.faction2.days_won.ToString();
            newCO.transform.Find("$SIDE_B_NAME").GetComponent<TMP_Text>().text = c.faction2.name.ToString();
            newCO.transform.Find("$SIDE_B_STAKE").GetComponent<TMP_Text>().text = c.faction2.stake.ToString();

            newCO.transform.SetParent(details.conflict_object_parent);
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
    //
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
        StartCoroutine(API.GetFactionData(r));
    }
    #endregion

    [System.Serializable]
    public enum UIState
    {
        main = 0,
        search = 1,
        about = 2
    }

    #region data types
    [System.Serializable]
    public class Header
    {
        public Canvas canvas;
        public Button search, about;
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

        public GameObject trackedFactionPrefab;
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
    #endregion
}
