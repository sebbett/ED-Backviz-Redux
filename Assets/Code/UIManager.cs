using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EDBR.DB;
using EDBR.Data;
using EDBR;

public class UIManager : MonoBehaviour
{
    private void Awake()
    {
        GameManager.Events.factionDataReceived += factionDataReceived;
        GameManager.Events.factionDataError += factionDataError;
    }

    public Header header;
    public Search search;  

    private UIState uiState;

    void Start()
    {
        InitHeaderComponents();
        InitSearchComponents();

    }

    private void InitHeaderComponents()
    {
        header.search.onClick.AddListener(() => UpdateUI(UIState.search));
        //header.tracked_factions.onClick.AddListener(() => UpdateUI(UIState.tracked_factions));
    }

    private void InitSearchComponents()
    {
        search.close.onClick.AddListener(() => UpdateUI(UIState.main));
        search.search_bar.onValueChanged.AddListener((value) => PerformLocalFactionSearch(value));
    }

    private void PerformLocalFactionSearch(string value)
    {
        string[] matches = Factions.FindPartialMatches(value);
        foreach(string s in matches) Debug.Log(s);
        UpdateSearchResults(matches);
    }

    private void UpdateUI(UIState state)
    {
        Debug.Log($"UPDATING STATE: {state.ToString()}");
        uiState = state;
        UIState[] searchStates = { UIState.search };

        search.canvas.enabled = (searchStates.Any(x => x == uiState));
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
        search.spinner.enabled = true;
        StartCoroutine(API.GetFactionData(r));
    }

    private void factionDataReceived(string data)
    {
        Faction faction = Conversions.FactionFromJson(data);
        search.faction_details.SetActive(true);
        search.spinner.enabled = false;

        search.details_name.text = faction.name;
        search.details_home.text = ($"Home: {faction.faction_presence[0].system_name}");
        search.details_allegiance.text = ($"Allegiance: {faction.allegiance}");
        search.details_government.text = ($"Government: {faction.government}");
        search.details_presence.text = ($"Presence: {faction.faction_presence.Count()} systems(s)");

        search.track.onClick.RemoveAllListeners();
        search.track.onClick.AddListener(() => addTrackedFaction(faction.name));
    }

    private void addTrackedFaction(string name)
    {
        Debug.Log($"addTrackedFaction({name})");
    }

    private void factionDataError(string error)
    {
        throw new NotImplementedException();
    }

    [System.Serializable]
    public enum UIState
    {
        main = 0,
        search = 1,
        tracked_factions = 2
    }

    [System.Serializable]
    public class Header
    {
        public Canvas canvas;
        public Button search, tracked_factions;
    }
    [System.Serializable]
    public class Search
    {
        public Canvas canvas;
        public Button close;
        public TMP_InputField search_bar;
        public TMP_Text no_matches_found;
        public Image spinner;
        public GameObject
            faction_details,
            search_results,
            search_result_prefab;

        //faction detail objects
        public TMP_Text
            details_name,
            details_home,
            details_allegiance,
            details_government,
            details_presence;

        public Button track;
    }
}
