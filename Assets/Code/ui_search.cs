using EDBR;
using EDBR.Data;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static EDBR.Data.Data;

public class ui_search : MonoBehaviour
{
    #region public variables
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
    #endregion

    void Awake()
    {
        GameManager.Events.factionDataReceived.AddListener(factionDataReceived);
        GameManager.Events.factionsUpdated.AddListener(factionsUpdated);

        InitSearchComponents();
    }

    private void InitSearchComponents()
    {
        search_bar.onValueChanged.AddListener((value) => PerformLocalFactionSearch(value));
    }

    private void factionDataReceived(_faction[] data)
    {
        _faction faction = data[0];
        faction_details.SetActive(true);
        spinner.SetActive(false);

        details_name.text = faction.name;
        details_home.text = ($"Home: {faction.faction_presence[0].system_name}");
        details_allegiance.text = ($"Allegiance: {faction.allegiance}");
        details_government.text = ($"Government: {faction.government}");
        details_presence.text = ($"Presence: {faction.faction_presence.Count()} systems(s)");

        track.onClick.RemoveAllListeners();
        track.onClick.AddListener(() => GameManager.Session.addTrackedFactions(new _faction[] { faction }));
        track.onClick.AddListener(() => track.interactable = false);

        track.interactable = !GameManager.Session.isTrackingFaction(faction);

        logo_alliance.enabled = faction.allegiance == "alliance";
        logo_empire.enabled = faction.allegiance == "empire";
        logo_federation.enabled = faction.allegiance == "federation";
        logo_independent.enabled = faction.allegiance == "independent";
    }
    //Populate tracked factions
    private void factionsUpdated()
    {
        //Clear current list
        foreach (Transform child in listParent)
        {
            Destroy(child.gameObject);
        }

        //Populate new data
        foreach (TrackedFaction tf in GameManager.Session.trackedFactions)
        {
            //Search list
            GameObject searchFactionObject = Instantiate(faction_object_prefab);
            searchFactionObject.transform.Find("$FACTION_NAME").GetComponent<TMP_Text>().text = tf.faction.name;
            searchFactionObject.transform.Find("$FACTION_HOME").GetComponent<TMP_Text>().text = tf.faction.faction_presence[0].system_name;
            searchFactionObject.transform.Find("$FACTION_PRESENCE").GetComponent<TMP_Text>().text = ($"{tf.faction.faction_presence.Count} SYSTEMS");
            searchFactionObject.transform.Find("$FACTION_COLOR").GetComponent<Image>().color = tf.color;
            searchFactionObject.transform.SetParent(listParent);
            searchFactionObject.GetComponent<Button>().onClick.AddListener(() => GameManager.Session.setSelectedFaction(tf.faction.name));
        }
    }

    private void GetFactionDetails(string r)
    {
        faction_details.SetActive(false);
        spinner.SetActive(true);
        StartCoroutine(API.GetFactionData(new string[] { r }));
    }

    private void PerformLocalFactionSearch(string value)
    {
        string[] matches = DB.Factions.FindPartialMatches(value);
        UpdateSearchResults(matches);
    }

    private void UpdateSearchResults(string[] results)
    {
        //Clear current search results
        foreach (Transform child in search_results.transform)
        {
            Destroy(child.gameObject);
        }
        //Add new results
        foreach (string r in results)
        {
            GameObject newSearchResult = Instantiate(search_result_prefab);
            newSearchResult.GetComponentInChildren<TMP_Text>().text = r;
            newSearchResult.GetComponent<Button>().onClick.AddListener(() => GetFactionDetails(r));
            newSearchResult.transform.SetParent(search_results.transform);
        }

        no_matches_found.enabled = !(results.Length > 0);
    }
}
