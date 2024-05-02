using EDBR;
using EDBR.Data;
using System.Linq;
using TMPro;
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
    }

    

    #region Variables
    public Header header;
    public Search search;
    public Camera mainCamera;
    #endregion

    void Start()
    {
        mainCamera = Camera.main;
        InitHeaderComponents();
        InitSearchComponents();
        UpdateUI(UIState.main);
    }

    #region Init
    private void InitHeaderComponents()
    {
        header.search.onClick.AddListener(() => UpdateUI(UIState.search));
    }
    private void InitSearchComponents()
    {
        search.close.onClick.AddListener(() => UpdateUI(UIState.main));
        search.search_bar.onValueChanged.AddListener((value) => PerformLocalFactionSearch(value));
    }
    #endregion

    private UIState uiState;
    private void UpdateUI(UIState state)
    {
        uiState = state;
        UIState[] searchStates = { UIState.search };
        UIState[] cameraUnlockedStates = { UIState.main };

        search.canvas.enabled = (searchStates.Any(x => x == uiState));
        mainCamera.GetComponent<MouseOrbit>().lockControls = !(cameraUnlockedStates.Any(x => x == uiState));
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
    }

    #region data types
    [System.Serializable]
    public class Header
    {
        public Canvas canvas;
        public Button search;
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
    #endregion
}
