using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// TODO: All of the old code here needs refactored/cleaned
public class Region_Panel_Script : MonoBehaviour{
    public GameObject gameController;
    public Player_Controller playerController;

    public GameObject [] unitButtons;

    public TextMeshProUGUI regionNameText;
    public TextMeshProUGUI evilPopulationText;
    public TextMeshProUGUI goodPopulationText;
    public TextMeshProUGUI neutralPopulationText;
    public TextMeshProUGUI localDemonsText;
    public TextMeshProUGUI localAngelsText;
    //public Text localInquisitors; -- unknown at game start
    public TextMeshProUGUI localBansheesText;
    //public Text conversionGood;
    public TextMeshProUGUI sinEfficencyText;

    public GameObject demonDotPrefab;
    public GameObject bansheeDotPrefab;

    public Population_Bar populationBar;

    private Region_Controller currentRegion;

    private void Start() {
        UpdateToWorldRegion();
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
            localDemonsText.text = currentRegion.GetLocalDemons().ToString();
            localAngelsText.text = currentRegion.GetLocalAngels().ToString();
            localBansheesText.text = currentRegion.GetLocalBanshees().ToString();
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

    // TODO: Currentregion shouldn't exist in this context since it may accidently be used below. So this should be in a different class or something.
    public void UpdateToWorldRegion() {
        regionNameText.text = "WORLD";

        World_Controller worldController = gameController.GetComponent<World_Controller>();
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
        Devil_Controller devilController = gameController.GetComponent<Devil_Controller>();
        sinEfficencyText.text = (devilController.sinEfficency * 100).ToString() + "%";
        //conversionGood.text = (regionController.GetConversionGood() * 100).ToString() + "%";
    }



    public void AddDemon() {
        if (gameController.GetComponent<Devil_Controller>().GetAvailableDemons() > 0) {
            print("+1 Demon");
            currentRegion.IncrementLocalDemons();
            localDemonsText.text = currentRegion.GetLocalDemons().ToString();
            gameController.GetComponent<Devil_Controller>().DecrementGlobalDemons();

            //add demon orange dot
            Vector3 dotLocation = new Vector3(0.0f, 0.0f, 0.0f);
            dotLocation = gameController.GetComponent<Player_Controller>().GetCountryRandomLocale();
            GameObject demonDot = Instantiate(demonDotPrefab, dotLocation, Quaternion.identity);

            //Name the orange dot
            demonDot.name = "demonPresence_" + gameController.GetComponent<Player_Controller>().GetCountryHit().name +
            currentRegion.GetLocalDemons().ToString();
        }
    }

    public void RemoveDemon() {
        if (currentRegion.GetLocalDemons() > 0) {
            print("-1 Demon");

            //delete demon orange dot
            Destroy(GameObject.Find("demonPresence_" + gameController.GetComponent<Player_Controller>().GetCountryHit().name + currentRegion.GetLocalDemons().ToString())); //TODO: Store it locally instead of Find().
            currentRegion.DecrementLocalDemons();
            localDemonsText.text = currentRegion.GetLocalDemons().ToString();
            //increment available demons on world controller
            gameController.GetComponent<Devil_Controller>().IncrementGlobalDemons();
        }
    }

    public void AddBanshee() {
        if (gameController.GetComponent<Devil_Controller>().GetAvailableBanshees() > 0) {
            print("+1 Banshee");
            currentRegion.IncrementLocalBanshees();
            localBansheesText.text = currentRegion.GetLocalBanshees().ToString();

            //decrement available banshees on world controller
            gameController.GetComponent<Devil_Controller>().DecrementGlobalBanshees();

            //add banshee dot
            Vector3 dotLocation = new Vector3(0.0f, 0.0f, 0.0f);
            dotLocation = gameController.GetComponent<Player_Controller>().GetCountryRandomLocale();
            GameObject bansheeDot = Instantiate(bansheeDotPrefab, dotLocation, Quaternion.identity);

            //Name the  dot (so it can be deleted when removed) //demonPresence UNITEDSTATES1
            bansheeDot.name = "bansheePresence_" + gameController.GetComponent<Player_Controller>().GetCountryHit().name + currentRegion.GetLocalDemons().ToString(); 
        }
    }

    public void RemoveBanshee() {
        if (currentRegion.GetLocalBanshees() > 0) {
            print("-1 Banshee");

            //delete banshee dot
            Destroy(GameObject.Find("bansheePresence_" + gameController.GetComponent<Player_Controller>().GetCountryHit().name + currentRegion.GetLocalDemons().ToString())); //TODO: Store it locally instead of Find().
            currentRegion.DecrementLocalBanshees();
            localBansheesText.text =currentRegion.GetLocalBanshees().ToString();
        }

        //increment available banshees on world controller
        gameController.GetComponent<Devil_Controller>().IncrementGlobalBanshees();
    }
}
