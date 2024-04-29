using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EDBR.DB;
using Unity.VisualScripting;

public class UIManager : MonoBehaviour
{

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

        foreach(string r in results)
        {
            GameObject newSearchResult = Instantiate(search.search_result_prefab);
            newSearchResult.GetComponentInChildren<TMP_Text>().text = r;
            newSearchResult.GetComponent<Button>().onClick.AddListener(() => GetFactionDetails(r));
            newSearchResult.transform.SetParent(search.search_results.transform);
        }
    }

    private void GetFactionDetails(string r)
    {
        Debug.Log($"REACHED GetFactionDetails({r})");
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
        public Button close, track;
        public TMP_InputField search_bar;
        public GameObject faction_details, search_results;
        public GameObject search_result_prefab;
    }
}
