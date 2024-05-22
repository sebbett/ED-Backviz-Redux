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
    }

    private Camera mainCamera;

    public Canvas
        header_canvas,
        search_canvas,
        faction_canvas,
        system_panel_canvas,
        about_canvas;

    void Start()
    {
        mainCamera = Camera.main;
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

    private UIState uiState;
    public void UpdateUI(UIState state)
    {
        uiState = state;
        UIState[] cameraUnlockedStates = { UIState.main };

        header_canvas.enabled = (uiState == UIState.main);
        search_canvas.enabled = (uiState == UIState.search);
        about_canvas.enabled = (uiState == UIState.about);
        system_panel_canvas.enabled = ((uiState == UIState.main || uiState == UIState.factions) && bvCore.Session.selectedSystemID.Length > 0);
        faction_canvas.enabled = (uiState == UIState.factions);
        mainCamera.GetComponent<MouseOrbit>().lockControls = !(cameraUnlockedStates.Any(x => x == uiState));
    }

}

[Serializable]
public enum UIState
{
    main = 0,
    search = 1,
    about = 2,
    factions = 3
}
