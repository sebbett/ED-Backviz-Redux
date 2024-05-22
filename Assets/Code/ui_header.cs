using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class ui_header : MonoBehaviour
{
    public UIManager manager;
    public Canvas canvas;
    public Button
        search,
        factions,
        about;
    public TMP_Text status;

    private void Awake()
    {
        search.onClick.AddListener(() => manager.UpdateUI(UIState.search));
        about.onClick.AddListener(() => manager.UpdateUI(UIState.about));
        factions.onClick.AddListener(() => manager.UpdateUI(UIState.factions));
    }
}
