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
        GameManager.Events.systemsUpdated.AddListener(systemsUpdated);
        GameManager.Events.systemSelected.AddListener(systemSelected);
        GameManager.Events.statusUpdated.AddListener((status) => statusUpdated(status));

        system_panel_canvas.GetComponent<ui_system_panel>().SetUIManager(this);
    }

    #region Variables
    public Header header;
    public Camera mainCamera;

    public Canvas
        search_canvas,
        faction_canvas,
        system_panel_canvas,
        about_canvas;
    #endregion

    void Start()
    {
        mainCamera = Camera.main;
        InitHeaderComponents();
        UpdateUI(UIState.main);
    }

    private void Update()
    {
        GetEscapeKey();
    }

    private void GetEscapeKey()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            UpdateUI(UIState.main);
    }

    #region Init
    private void InitHeaderComponents()
    {
        header.search.onClick.AddListener(() => UpdateUI(UIState.search));
        header.about.onClick.AddListener(() => UpdateUI( UIState.about));
        header.factions.onClick.AddListener(() => UpdateUI(UIState.factions));
    }
    #endregion

    private UIState uiState;
    public void UpdateUI(UIState state)
    {
        uiState = state;
        UIState[] cameraUnlockedStates = { UIState.main };

        search_canvas.enabled = (uiState == UIState.search);
        about_canvas.enabled = (uiState == UIState.about);
        system_panel_canvas.enabled = ((uiState == UIState.main || uiState == UIState.factions) && selected_system_id.Length > 0);
        faction_canvas.enabled = (uiState == UIState.factions);
        mainCamera.GetComponent<MouseOrbit>().lockControls = !(cameraUnlockedStates.Any(x => x == uiState));
    }

    private void statusUpdated(string status)
    {
        header.status.text = status;
    }

    //Populate System Details
    private string selected_system_id = "";
    private void systemsUpdated()
    {
        if(selected_system_id.Length > 0)
        {
            GameManager.Session.setSelectedSystem(selected_system_id);
        }
    }
    private void systemSelected(system_details s)
    {
        selected_system_id = s._id;
    }

        [Serializable]
    public enum UIState
    {
        main = 0,
        search = 1,
        about = 2,
        factions = 3
    }

    #region data types
    [Serializable]
    public class Header
    {
        public Canvas canvas;
        public Button
            search,
            factions,
            about;
        public TMP_Text status;
    }
    #endregion
}
