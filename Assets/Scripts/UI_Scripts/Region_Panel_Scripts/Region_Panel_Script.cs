using TMPro;
using UnityEngine;

// TODO: All of the old code here needs refactored/cleaned
public class Region_Panel_Script : MonoBehaviour {
    public GameObject gameController; // TODO: Should not be using GetComponent throughout this class. Should instead have public references to Devil Controller etc. here.

    // TODO: Make texts abstract from the different factions.
    [SerializeField] private Unit_UI unitUI;
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
    [SerializeField] private Population_Bar populationBar;
  
    private Region_Controller currentRegion;

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
            currentRegion = region.GetComponent<Region_Controller>();
            if (currentRegion.isLocked()) {
                DisableUnitButtons();
            } else {
                EnableUnitButtons();
            }
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
            if (!button.activeSelf) {
                button.SetActive(true);
            }
        }
    }

    private void DisableUnitButtons() {
        foreach (GameObject button in unitButtons) {
            if (button.activeSelf) {
                button.SetActive(false);
            }
        }
    }

    // TODO: Below methods should have no reference to devil controller or god controller, or Evil/Good agents. It should be agnostic.
    public void AddAgent() {
        if (devilController.AvailableAgents > 0) {
            currentRegion.IncrementLocalEvilAgents();
            devilController.AvailableAgents--;

            string localEvilAgents = currentRegion.GetLocalEvilAgents().ToString();
            localPlayerAgentText.text = localEvilAgents;

            //add agent gameobject
            Vector3 desiredAgentLocation = gameController.GetComponent<Player_Controller>().GetRegionRandomLocale(); // TODO: Could this be a static method in a utility class?
            unitUI.AddAgent(Player_Controller.playerControlledFaction ,currentRegion.name, localEvilAgents, desiredAgentLocation);
        }
    }

    public void RemoveAgent() {
        ushort localEvilAgents = currentRegion.GetLocalEvilAgents();
        if (localEvilAgents > 0) {
            // Destroy demon game object
            unitUI.RemoveAgent(Player_Controller.playerControlledFaction, currentRegion.name, localEvilAgents);
            currentRegion.DecrementLocalEvilAgents();
            devilController.AvailableAgents++;
            localPlayerAgentText.text = currentRegion.GetLocalEvilAgents().ToString();
        }
    }

    public void AddSecondaryUnit() {
        if (devilController.AvailableSecondaryUnits > 0) {
            currentRegion.IncrementLocalEvilSecondaryUnits();
            devilController.AvailableSecondaryUnits--;

            ushort localEvilSecondaryUnits = currentRegion.GetLocalEvilSecondaryUnits();
            localPlayerSecondaryUnitText.text = localEvilSecondaryUnits.ToString();

            //add unit gameobject
            Vector3 desiredUnitLocation = gameController.GetComponent<Player_Controller>().GetRegionRandomLocale(); // TODO: Could this be a static method in a utility class?
            unitUI.AddSecondaryUnit(Player_Controller.playerControlledFaction, currentRegion.name, localEvilSecondaryUnits, desiredUnitLocation);
        }
    }

    public void RemoveSecondaryUnit() {
        ushort localEvilSecondaryUnits = currentRegion.GetLocalEvilSecondaryUnits();
        if (currentRegion.GetLocalEvilSecondaryUnits() > 0) {
            // Destroy unit game object
            unitUI.RemoveSecondaryUnit(Player_Controller.playerControlledFaction, currentRegion.name, localEvilSecondaryUnits);

            currentRegion.DecrementLocalEvilSecondaryUnits();
            devilController.AvailableSecondaryUnits++;
            localPlayerSecondaryUnitText.text = currentRegion.GetLocalEvilSecondaryUnits().ToString();
        }
    }
}
