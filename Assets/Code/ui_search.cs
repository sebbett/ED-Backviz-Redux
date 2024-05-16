using bv;
using bvData;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        bvCore.Events.TrackedFactionsUpdated.AddListener(factionsUpdated);

        InitSearchComponents();
    }

    private void InitSearchComponents()
    {
        search_bar.onValueChanged.AddListener((value) => GetSearchMatches(value));
    }

    private void factionDataReceived(bvFaction[] data)
    {
        bvFaction faction = data[0];
        faction_details.SetActive(true);
        spinner.SetActive(false);

        details_name.text = faction.name;
        details_home.text = ($"Home: {faction.faction_presence[0].name}");
        details_allegiance.text = ($"Allegiance: {faction.allegiance}");
        details_government.text = ($"Government: {faction.government}");
        details_presence.text = ($"Presence: {faction.faction_presence.Count()} systems(s)");

        track.onClick.RemoveAllListeners();
        track.onClick.AddListener(() => bvCore.Session.RequestFactions(new string[] { faction.name }));
        track.onClick.AddListener(() => track.interactable = false);

        track.interactable = !bvCore.Session.factionIsTracked(faction);

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
        foreach (bvFaction faction in bvCore.Session.factions)
        {
            //Search list
            GameObject searchFactionObject = Instantiate(faction_object_prefab);
            searchFactionObject.transform.Find("$FACTION_NAME").GetComponent<TMP_Text>().text = faction.name;
            searchFactionObject.transform.Find("$FACTION_HOME").GetComponent<TMP_Text>().text = faction.faction_presence[0].name;
            searchFactionObject.transform.Find("$FACTION_PRESENCE").GetComponent<TMP_Text>().text = ($"{faction.faction_presence.Length} SYSTEMS");
            searchFactionObject.transform.Find("$FACTION_COLOR").GetComponent<Image>().color = faction.color;
            searchFactionObject.transform.SetParent(listParent);
            searchFactionObject.GetComponent<Button>().onClick.AddListener(() => GameManager.Session.setSelectedFaction(faction.name));
        }
    }

    private void GetFactionDetails(string r)
    {
        faction_details.SetActive(false);
        spinner.SetActive(true);
        //StartCoroutine(API.GetFactionData(new string[] { r }));
        StartCoroutine(bvAPI.bvAPI.GetFactionData(new string[] { r }, factionDataReceived));
    }

    private void GetSearchMatches(string value)
    {
        //Get a list of partial matches from the local faction DB
        string[] matches = Database.Factions.FindPartialMatches(value);
        
        //Clear current search results
        foreach (Transform child in search_results.transform)
        {
            Destroy(child.gameObject);
        }

        //Add new search results
        foreach (string match in matches)
        {
            GameObject newSearchResult = Instantiate(search_result_prefab);
            newSearchResult.GetComponentInChildren<TMP_Text>().text = match;
            newSearchResult.GetComponent<Button>().onClick.AddListener(() => GetFactionDetails(match));
            newSearchResult.transform.SetParent(search_results.transform);
        }

        //If no matches found, tell the user
        no_matches_found.enabled = !(matches.Length > 0);
    }
}
