using System.Collections.Generic;
using TMPro;
using UnityEngine;

// TODO: All of the old code here needs refactored/cleaned
public class Region_Panel_Script : MonoBehaviour {
    public GameObject gameController; // TODO: Should not be using GetComponent throughout this class. Should instead have public references to Devil Controller etc. here.

    // TODO: Make texts abstract from the different factions.
    [SerializeField] private Devil_Controller devilController;
    [SerializeField] private Player_Controller playerController;
    [SerializeField] private GameObject[] unitButtons;
    [SerializeField] private TextMeshProUGUI regionNameText;
    [SerializeField] private TextMeshProUGUI evilPopulationText;
    [SerializeField] private TextMeshProUGUI goodPopulationText;
    [SerializeField] private TextMeshProUGUI neutralPopulationText;
    [SerializeField] private TextMeshProUGUI localPlayerAgentText; 
    [SerializeField] private TextMeshProUGUI localEnemyAgentText;
    [SerializeField] private TextMeshProUGUI localPlayerSecondaryUnitText;
    [SerializeField] private TextMeshProUGUI secondayResourceEfficencyText;
    [SerializeField] private GameObject demonDotPrefab;
    [SerializeField] private GameObject bansheeDotPrefab;
    // TODO: Prefabs for God faction.
    [SerializeField] private Population_Bar populationBar;
    [SerializeField] private GameObject unitsContainerRef;

    private Region_Controller currentRegion;
    private Dictionary<string, GameObject> playerAgentsByRegion = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> playerSecondaryUnitsByRegion = new Dictionary<string, GameObject>();

    private void Start() {
        UpdateToWorldRegion();
        Clock.OnDayPassedNotifyHUD += UpdateRegionPanel;
    }

    // TODO: Currentregion shouldn't exist in this context since it may accidently be used below. So this should be in a different class or something.
    public void UpdateToWorldRegion() {
        regionNameText.text = "WORLD";

        ulong evilPop = Global_Population_Viewer.GetTotalEvilPopulation();
        ulong goodPop = Global_Population_Viewer.GetTotalGoodPopulation();
        ulong neutralPop = Global_Population_Viewer.GetTotalNeutralPopulation();

        evilPopulationText.text = "Evil: " + evilPop.ToString();
        goodPopulationText.text = "Good: " + goodPop.ToString();
        neutralPopulationText.text = "Neutral: " + neutralPop.ToString();
        populationBar.SetFillAmounts(evilPop, goodPop, neutralPop);

        localPlayerAgentText.text = Global_Units_Viewer.GetDeployedEvilAgents().ToString(); // TODO: This should actually just be the total number of placed demons.
        localEnemyAgentText.text = Global_Units_Viewer.GetDeployedGoodAgents().ToString(); //TODO: Should I even be able to see angels?
        localPlayerSecondaryUnitText.text = Global_Units_Viewer.GetDeployedEvilSecondaryUnits().ToString();// TODO: This should actually just be the total number of placed banshees.
        secondayResourceEfficencyText.text = (devilController.SecondaryResourceGenerationEfficency * 100).ToString() + "%"; // TODO: This is the global sinEfficency rate, not the local.
                                                                                                               // Local is to be replaced with anarchy levels                                                                                                       //conversionGood.text = (regionController.GetConversionGood() * 100).ToString() + "%";
    }

    // TODO: Exit region panel when opening the research tree.
    public void ExitRegionPanel() {
        currentRegion.GetComponent<Region_Controller>().DeactivateBorder();
        this.gameObject.SetActive(false);
    }

    private void UpdateRegionPanel(){
        GameObject region = (currentRegion == null)? null : currentRegion.gameObject;
        UpdateRegionPanel(region);
    }

    // TODO: shouldn't be doing GetComponent. Can just store as a variable.
    public void UpdateRegionPanel(GameObject region) {
        if (region == null) {
            // Display world statistics.
            currentRegion = null;
            DisableUnitButtons();
            UpdateToWorldRegion();
        } else {
            EnableUnitButtons();
            currentRegion = region.GetComponent<Region_Controller>();
            regionNameText.text = region.name + ", " + region.transform.parent.name;
            evilPopulationText.text = "Evil: " + currentRegion.GetEvilPop().ToString("0");
            goodPopulationText.text = "Good: " + currentRegion.GetGoodPop().ToString("0");
            populationBar.SetFillAmounts(currentRegion);
            neutralPopulationText.text = "Neutral: " + currentRegion.GetNeutralPop().ToString("0");
            localPlayerAgentText.text = currentRegion.GetLocalEvilAgents().ToString();
            localEnemyAgentText.text = currentRegion.GetLocalGoodAgents().ToString();
            localPlayerSecondaryUnitText.text = currentRegion.GetLocalEvilSecondaryUnits().ToString();
            secondayResourceEfficencyText.text = (currentRegion.GetSinEfficency() * 100).ToString() + "%";
        }
    }

    private void EnableUnitButtons() {
        foreach (GameObject button in unitButtons) {
            button.SetActive(true);
        }
    }

    private void DisableUnitButtons() {
        foreach (GameObject button in unitButtons) {
            button.SetActive(false);
        }
    }

    // TODO: Get the playerController.PlayerFaction. If statement for God faction (when playable)
    public void AddAgent() {
        if (devilController.AvailableAgents > 0) {
            currentRegion.IncrementLocalEvilAgents();
            devilController.AvailableAgents--;

            string localEvilAgents = currentRegion.GetLocalEvilAgents().ToString();
            localPlayerAgentText.text = localEvilAgents;

            //add agent gameobject
            Vector3 dotLocation = gameController.GetComponent<Player_Controller>().GetRegionRandomLocale(); // TODO: Could this be a static method in a utility class?
            GameObject demonDot = Instantiate(demonDotPrefab, dotLocation, Quaternion.identity, unitsContainerRef.transform);
            demonDot.name = "demon_" + currentRegion.name + "_" + localEvilAgents;
            playerAgentsByRegion.Add((currentRegion.name + localEvilAgents), demonDot);
        }
    }

    // TODO: Get the playerController.PlayerFaction. If statement for God faction (when playable)
    public void RemoveAgent() {
        if (currentRegion.GetLocalEvilAgents() > 0) {
            // Destroy demon game object
            string demonGOName = currentRegion.name + currentRegion.GetLocalEvilAgents().ToString();
            GameObject demonPrefabInstance = playerAgentsByRegion[demonGOName];
            playerAgentsByRegion.Remove(demonGOName);
            Destroy(demonPrefabInstance);

            currentRegion.DecrementLocalEvilAgents();
            devilController.AvailableAgents++;
            localPlayerAgentText.text = currentRegion.GetLocalEvilAgents().ToString();
        }
    }

    // TODO: Get the playerController.PlayerFaction. If statement for God faction (when playable)
    public void AddSecondaryUnit() {
        if (devilController.AvailableSecondaryUnits > 0) {
            currentRegion.IncrementLocalEvilSecondaryUnits();
            devilController.AvailableSecondaryUnits--;

            string localEvilSecondaryUnits = currentRegion.GetLocalEvilSecondaryUnits().ToString();
            localPlayerSecondaryUnitText.text = localEvilSecondaryUnits;

            //add banshee gameobject
            Vector3 dotLocation = gameController.GetComponent<Player_Controller>().GetRegionRandomLocale(); // TODO: Could this be a static method in a utility class?
            GameObject bansheeDot = Instantiate(bansheeDotPrefab, dotLocation, Quaternion.identity, unitsContainerRef.transform);
            bansheeDot.name = "banshee_" + currentRegion.name + "_" + localEvilSecondaryUnits;
            playerSecondaryUnitsByRegion.Add((currentRegion.name + localEvilSecondaryUnits), bansheeDot);
        }
    }

    // TODO: Get the playerController.PlayerFaction. If statement for God faction (when playable)
    public void RemoveSecondaryUnit() {
        if (currentRegion.GetLocalEvilSecondaryUnits() > 0) {
            // Destroy banshee game object
            string bansheeGOName = currentRegion.name + currentRegion.GetLocalEvilSecondaryUnits().ToString();
            GameObject bansheePrefabInstance = playerSecondaryUnitsByRegion[bansheeGOName];
            playerSecondaryUnitsByRegion.Remove(bansheeGOName);
            Destroy(bansheePrefabInstance);

            currentRegion.DecrementLocalEvilSecondaryUnits();
            devilController.AvailableSecondaryUnits++;
            localPlayerSecondaryUnitText.text = currentRegion.GetLocalEvilSecondaryUnits().ToString();
        }
    }
}
