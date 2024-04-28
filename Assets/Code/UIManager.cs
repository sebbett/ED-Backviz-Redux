using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        Debug.Log(value);
    }

    private void UpdateUI(UIState state)
    {
        Debug.Log($"UPDATING STATE: {state.ToString()}");
        uiState = state;
        UIState[] searchStates = { UIState.search };

        search.canvas.enabled = (searchStates.Any(x => x == uiState));
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
