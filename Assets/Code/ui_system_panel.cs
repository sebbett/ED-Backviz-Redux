using EDBR.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UIManager;

public class ui_system_panel : MonoBehaviour
{
    #region public variables
    public bool isOpen = false;
    public float
        open_x,
        closed_x,
        wanted_x,
        current_x,
        speed;
    public Color
        green_state,
        yellow_state,
        blue_state,
        red_state,
        grey_state;
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
    #endregion

    private UIManager ui_manager;

    private void Awake()
    {
        GameManager.Events.systemSelected.AddListener(systemSelected);

        InitDetailComponents();
    }

    private void Update()
    {
        UpdateSystemDetails();
    }

    private void InitDetailComponents()
    {
        conflicts.onClick.AddListener(() => ToggleConflictsPanel());
    }

    private void UpdateSystemDetails()
    {
        if (isOpen)
            wanted_x = open_x;
        else
            wanted_x = closed_x;

        current_x = Mathf.Lerp(current_x, wanted_x, speed * Time.deltaTime);

        RectTransform pos = panel.GetComponent<RectTransform>();

        Vector2 newPos = new Vector2(current_x, pos.anchoredPosition.y);

        panel.GetComponent<RectTransform>().anchoredPosition = newPos;
    }

    private void ToggleConflictsPanel()
    {
        isOpen = !isOpen;
    }

    private void CopySystemNameToClipboard(string text)
    {
        GUIUtility.systemCopyBuffer = text;
    }

    string selected_system_id = "";
    private void systemSelected(system_details s)
    {
        selected_system_id = s._id;

        system_label.text = s.name;
        copy.onClick.AddListener(() => CopySystemNameToClipboard(s.name));

        //Clear current faction objects
        foreach (Transform child in faction_object_parent)
        {
            Destroy(child.gameObject);
        }

        //Sort factions by influence
        system_details.Faction[] sortedFactions = s.factions.OrderByDescending(x => x.influence).ToArray();

        //Populate the list
        foreach (system_details.Faction f in sortedFactions)
        {
            string name = f.name;
            string inf()
            {
                string i = "LOADING";
                if (f.influence > 0)
                    i = ((float)f.influence * 100).ToString("##.##") + "%";
                return i;
            }
            string current_state = f.state;
            //string pending_state = "none";
            //Debug.Log(pending_state);

            GameObject newFO = Instantiate(faction_object_prefab);
            newFO.transform.Find("$FACTION_NAME").GetComponent<TMP_Text>().text = name;
            newFO.transform.Find("$INFLUENCE").GetComponent<TMP_Text>().text = inf();
            newFO.transform.Find("$STATE_COLOR").transform.Find("$STATE_TEXT").GetComponent<TMP_Text>().text = current_state;
            newFO.transform.Find("$STATE_COLOR").GetComponent<Image>().color = GetStateColor(current_state);
            newFO.GetComponent<Button>().onClick.AddListener(() => ui_manager.UpdateUI(UIState.search));
            //newFO.GetComponent<Button>().onClick.AddListener(() => GetFactionDetails(f.name));
            newFO.transform.SetParent(faction_object_parent);
        }

        string government = CleanText(s.government);
        string primary_econ = CleanText(s.primary_economy);
        string secondary_econ = CleanText(s.secondary_economy);
        string security = CleanText(s.security);
        string sys_state = CleanText(s.state);

        government_label.text = ($"Government: {government}");
        primary_econ_label.text = ($"Primary Economy: {primary_econ}");
        secondary_econ_label.text = ($"Secondary Economy: {secondary_econ}");
        security_label.text = ($"Security: {security}");
        state_label.text = ($"State: {sys_state}");

        //Clear conflict objects
        foreach (Transform child in conflict_object_parent)
        {
            Destroy(child.gameObject);
        }

        no_conflicts_label.gameObject.SetActive(s.conflicts.Count == 0);

        //Populate new conflicts
        foreach (system_details.Conflict c in s.conflicts)
        {
            string conflict_status = c.status;
            if (conflict_status == "")
                conflict_status = "concluded";

            GameObject newCO = Instantiate(conflict_object_prefab);
            newCO.transform.Find("$CONFLICT_TYPE").GetComponent<TMP_Text>().text = c.type;
            newCO.transform.Find("$CONFLICT_STATUS").GetComponent<TMP_Text>().text = conflict_status;
            newCO.transform.Find("$SCORE_A").GetComponent<TMP_Text>().text = c.faction1.days_won.ToString();
            newCO.transform.Find("$SIDE_A_NAME").GetComponent<TMP_Text>().text = c.faction1.name.ToString();
            newCO.transform.Find("$SIDE_A_STAKE").GetComponent<TMP_Text>().text = c.faction1.stake.ToString();

            newCO.transform.Find("$SCORE_B").GetComponent<TMP_Text>().text = c.faction2.days_won.ToString();
            newCO.transform.Find("$SIDE_B_NAME").GetComponent<TMP_Text>().text = c.faction2.name.ToString();
            newCO.transform.Find("$SIDE_B_STAKE").GetComponent<TMP_Text>().text = c.faction2.stake.ToString();

            newCO.transform.SetParent(conflict_object_parent);
        }
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
    public Color GetStateColor(string state)
    {
        state = state.ToLower();

        string[] greenStates = new string[] { "incursion", "infested" };
        string[] yellowStates = new string[] { "blight", "drought", "outbreak", "infrastructurefailure", "naturaldisaster", "revolution", "coldwar", "tradewar", "pirateattack", "terroristattack", "retreat", "unhappy", "bust", "civilunrest" };
        string[] blueStates = new string[] { "publicholiday", "technologicalleap", "historicevent", "colonisation", "expansion", "happy", "elated", "boom", "investment", "civilliberty" };
        string[] redStates = new string[] { "war", "civilwar", "elections", "despondent", "famine", "lockdown" };

        Color value;

        if (greenStates.Contains(state)) value = green_state;
        else if (yellowStates.Contains(state)) value = yellow_state;
        else if (blueStates.Contains(state)) value = blue_state;
        else if (redStates.Contains(state)) value = red_state;
        else value = grey_state;

        return value;
    }

    public void SetUIManager(UIManager m)
    {
        ui_manager = m;
    }
}
