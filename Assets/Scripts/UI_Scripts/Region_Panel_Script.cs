using System.Collections.Generic;
using TMPro;
using UnityEngine;

// TODO: All of the old code here needs refactored/cleaned
public class Region_Panel_Script : MonoBehaviour {
    public GameObject gameController; // TODO: Should not be using GetComponent throughout this class. Should instead have public references to Devil Controller etc. here.

    // TODO: Make texts abstract from the different factions.
    [SerializeField]
    private Devil_Controller devilController;
    [SerializeField]
    private Player_Controller playerController;
    [SerializeField]
    private GameObject[] unitButtons;
    [SerializeField]
    private TextMeshProUGUI regionNameText;
    [SerializeField]
    private TextMeshProUGUI evilPopulationText;
    [SerializeField]
    private TextMeshProUGUI goodPopulationText;
    [SerializeField]
    private TextMeshProUGUI neutralPopulationText;
    [SerializeField]
    private TextMeshProUGUI localDemonsText; 
    [SerializeField]
    private TextMeshProUGUI localAngelsText;
    //public Text localInquisitors; -- unknown at game start
    [SerializeField]
    private TextMeshProUGUI localBansheesText;
    [SerializeField]
    private TextMeshProUGUI sinEfficencyText;
    [SerializeField]
    private GameObject demonDotPrefab;
    [SerializeField]
    private GameObject bansheeDotPrefab;
    [SerializeField]
    private Population_Bar populationBar;
    [SerializeField]
    private GameObject unitsContainerRef;

    private Region_Controller currentRegion;
    private Dictionary<string, GameObject> demonsByRegion = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> bansheesByRegion = new Dictionary<string, GameObject>();

    private void Start() {
        UpdateToWorldRegion();
    }

    // TODO: Currentregion shouldn't exist in this context since it may accidently be used below. So this should be in a different class or something.
    public void UpdateToWorldRegion() {
        regionNameText.text = "WORLD";

        Clock worldController = gameController.GetComponent<Clock>();
        ulong evilPop = worldController.GetTotalEvilPopulation();
        ulong goodPop = worldController.GetTotalGoodPopulation();
        ulong neutralPop = worldController.GetTotalNeutralPopulation();
        populationBar.SetFillAmounts(evilPop, goodPop, neutralPop);
        evilPopulationText.text = "Evil: " + evilPop.ToString();
        goodPopulationText.text = "Good: " + goodPop.ToString();
        neutralPopulationText.text = "Neutral: " + neutralPop.ToString();

        localDemonsText.text = worldController.GetDeployedDemons().ToString(); // TODO: This should actually just be the total number of placed demons.
        localAngelsText.text = worldController.GetDeployedDemons().ToString(); //TODO: Should I even be able to see angels?
        localBansheesText.text = worldController.GetDeployedBanshees().ToString();// TODO: This should actually just be the total number of placed banshees.
        sinEfficencyText.text = (devilController.SecondaryResourceGenerationEfficency * 100).ToString() + "%"; // TODO: This is the global sinEfficency rate, not the local.
                                                                                                               // Local is to be replaced with anarchy levels                                                                                                       //conversionGood.text = (regionController.GetConversionGood() * 100).ToString() + "%";
    }

    public void ExitRegionPanel() {
        // Disable the border
        currentRegion.borderRef.SetActive(false);
        // Disable the region panel
        this.gameObject.SetActive(false);
    }

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
            neutralPopulationText.text = "Neutral: " + currentRegion.GetNeutralPop().ToString("0");
            localDemonsText.text = currentRegion.GetLocalEvilAgents().ToString();
            localAngelsText.text = currentRegion.GetLocalGoodAgents().ToString();
            localBansheesText.text = currentRegion.GetLocalEvilSecondaryUnits().ToString();
            sinEfficencyText.text = (currentRegion.GetSinEfficency() * 100).ToString() + "%";
            //conversionGood.text = (regionController.GetConversionGood() * 100).ToString() + "%";
            populationBar.SetFillAmounts(currentRegion);
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

    // TODO: AddAgent() instead, and get the playerController.PlayerFaction. to increase abstraction.
    public void AddDemon() {
        if (devilController.AvailableAgents > 0) {
            currentRegion.IncrementLocalEvilAgents();
            devilController.AvailableAgents--;

            string localEvilAgents = currentRegion.GetLocalEvilAgents().ToString();
            localDemonsText.text = localEvilAgents;

            //add agent gameobject
            Vector3 dotLocation = gameController.GetComponent<Player_Controller>().GetRegionRandomLocale(); // TODO: Could this be a static method in a utility class?
            GameObject demonDot = Instantiate(demonDotPrefab, dotLocation, Quaternion.identity, unitsContainerRef.transform);
            demonDot.name = "demon_" + currentRegion.name + "_" + localEvilAgents;
            demonsByRegion.Add((currentRegion.name + localEvilAgents), demonDot);
        }
    }

    public void RemoveDemon() {
        if (currentRegion.GetLocalEvilAgents() > 0) {
            // Destroy demon game object
            string demonGOName = currentRegion.name + currentRegion.GetLocalEvilAgents().ToString();
            GameObject demonPrefabInstance = demonsByRegion[demonGOName];
            demonsByRegion.Remove(demonGOName);
            Destroy(demonPrefabInstance);

            currentRegion.DecrementLocalEvilAgents();
            devilController.AvailableAgents++;
            localDemonsText.text = currentRegion.GetLocalEvilAgents().ToString();
        }
    }

    public void AddBanshee() {
        if (devilController.AvailableSecondaryUnits > 0) {
            currentRegion.IncrementLocalEvilSecondaryUnits();
            devilController.AvailableSecondaryUnits--;

            string localEvilSecondaryUnits = currentRegion.GetLocalEvilSecondaryUnits().ToString();
            localBansheesText.text = localEvilSecondaryUnits;

            //add banshee gameobject
            Vector3 dotLocation = gameController.GetComponent<Player_Controller>().GetRegionRandomLocale(); // TODO: Could this be a static method in a utility class?
            GameObject bansheeDot = Instantiate(bansheeDotPrefab, dotLocation, Quaternion.identity, unitsContainerRef.transform);
            bansheeDot.name = "banshee_" + currentRegion.name + "_" + localEvilSecondaryUnits;
            bansheesByRegion.Add((currentRegion.name + localEvilSecondaryUnits), bansheeDot);
        }
    }

    public void RemoveBanshee() {
        if (currentRegion.GetLocalEvilSecondaryUnits() > 0) {
            // Destroy banshee game object
            string bansheeGOName = currentRegion.name + currentRegion.GetLocalEvilSecondaryUnits().ToString();
            GameObject bansheePrefabInstance = bansheesByRegion[bansheeGOName];
            bansheesByRegion.Remove(bansheeGOName);
            Destroy(bansheePrefabInstance);

            currentRegion.DecrementLocalEvilSecondaryUnits();
            devilController.AvailableSecondaryUnits++;
            localBansheesText.text = currentRegion.GetLocalEvilSecondaryUnits().ToString();
        }
    }
}
