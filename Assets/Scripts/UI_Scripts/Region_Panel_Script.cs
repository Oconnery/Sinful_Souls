using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// TODO: All of the old code here needs refactored/cleaned
public class Region_Panel_Script : MonoBehaviour{
    public GameObject worldController;
    public Player_Controller playerController;

    public Text countryName;
    public Text evilNumber;
    public Text goodNumber;
    public Text neutralNumber;
    public Text localDemons;
    public Text localAngels;
    //public Text localInquisitors; -- unknown at game start
    public Text localBanshees;
    public Text conversionGood;
    public Text conversionEvil;

    public GameObject demonDotPrefab;
    public GameObject bansheeDotPrefab;

    private Region_Controller currentCountry;



    void Start() {
        countryName.text = "Country_Name";//this.gameObject.ToString();
        evilNumber.text = "XXX";
        goodNumber.text = "YYY";
        neutralNumber.text = "ZZZ";
        this.gameObject.SetActive(false);
        conversionEvil.text = "100%";
        conversionGood.text = "100%";
    }

    public void SetCurrentCountry(Region_Controller currentCountry) {
        this.currentCountry = currentCountry;
    }

    public void ExitRegionPanel() {
        // Disable the border
        currentCountry.borderRef.SetActive(false);
        // Disable the region panel
        this.gameObject.SetActive(false);
    }

    public void ChangeCountryPanelText(GameObject country) {
        Region_Controller regionController = country.GetComponent<Region_Controller>();
        countryName.text = country.name;
        evilNumber.text = "Evil: " + regionController.GetEvilPop().ToString("0");
        goodNumber.text = "Good: " + regionController.GetGoodPop().ToString("0");
        neutralNumber.text = "Neutral: " + regionController.GetNeutralPop().ToString("0");
        localDemons.text = regionController.GetLocalDemons().ToString();
        localAngels.text = regionController.GetLocalAngels().ToString();
        localBanshees.text = regionController.GetLocalBanshees().ToString();
        conversionEvil.text = (regionController.GetConversionEvil() * 100).ToString() + "%";
        conversionGood.text = (regionController.GetConversionGood() * 100).ToString() + "%";
    }

    public void addDemon() {
        //if theres any demons available
        if (worldController.GetComponent<Devil_Controller>().GetAvailableDemons() > 0) {
            print("+1 Demon");
            string country = this.countryName.text;
            //anything better than find? - maybe make more efficent in future
            GameObject countryObj = GameObject.Find(country); // TODO: This is terrible old code that needs fixed.
            //Increment local demons
            countryObj.GetComponent<Region_Controller>().IncrementLocalDemons();
            //reload ui for demon .. 
            localDemons.text = countryObj.GetComponent<Region_Controller>().GetLocalDemons().ToString();

            //decrement the available demons
            //world controller demons --
            worldController.GetComponent<Devil_Controller>().DecrementGlobalDemons();

            //add demon orange dot //instantiates prefab
            //new gameobj
            GameObject demonDot = demonDotPrefab;
            Vector3 dotLocation = new Vector3(0.0f, 0.0f, 0.0f);
            dotLocation = worldController.GetComponent<Player_Controller>().GetCountryRandomLocale();
            demonDot = Instantiate(demonDot, dotLocation, Quaternion.identity);

            //Name the orange dot (so it can be deleted when removed) //demonPresence UNITEDSTATES1
            demonDot.name = "demonPresence_" + worldController.GetComponent<Player_Controller>().GetCountryHit().name +
            countryObj.GetComponent<Region_Controller>().GetLocalDemons().ToString();
        }
    }

    public void removeDemon() {
        string country = this.countryName.text;
        //anything better than find? - maybe make more efficent in future
        GameObject countryObj = GameObject.Find(country); // TODO: This is terrible old code that needs fixed.

        if (countryObj.GetComponent<Region_Controller>().GetLocalDemons() > 0) {
            print("-1 Demon");

            //delete demon orange dot
            Destroy(GameObject.Find("demonPresence_" + worldController.GetComponent<Player_Controller>().GetCountryHit().name +
            countryObj.GetComponent<Region_Controller>().GetLocalDemons().ToString()));

            //Decrement demons
            countryObj.GetComponent<Region_Controller>().DecrementLocalDemons();
            //reload ui
            localDemons.text = countryObj.GetComponent<Region_Controller>().GetLocalDemons().ToString();

            //decrement available demons on world controller
            worldController.GetComponent<Devil_Controller>().IncrementGlobalDemons();
        }
    }

    public void addBanshee() {
        if (worldController.GetComponent<Devil_Controller>().GetAvailableBanshees() > 0) {
            print("+1 Banshee");
            string country = this.countryName.text;
            //anything better than find? - maybe make more efficent in future
            GameObject countryObj = GameObject.Find(country);
            countryObj.GetComponent<Region_Controller>().IncrementLocalBanshees();
            //reload ui 
            localBanshees.text = countryObj.GetComponent<Region_Controller>().GetLocalBanshees().ToString();

            //decrement available demons on world controller
            worldController.GetComponent<Devil_Controller>().DecrementGlobalBanshees();

            //add banshee black dot //instantiates prefab at loc vec3 and default rot
            //new gameobj
            GameObject bansheeDot = bansheeDotPrefab;
            Vector3 dotLocation = new Vector3(0.0f, 0.0f, 0.0f);
            dotLocation = worldController.GetComponent<Player_Controller>().GetCountryRandomLocale();
            bansheeDot = Instantiate(bansheeDot, dotLocation, Quaternion.identity);

            //Name the black dot (so it can be deleted when removed) //demonPresence UNITEDSTATES1
            bansheeDot.name = "bansheePresence_" + worldController.GetComponent<Player_Controller>().GetCountryHit().name +
            countryObj.GetComponent<Region_Controller>().GetLocalDemons().ToString();
        }
    }

    public void removeBanshee() {
        string country = this.countryName.text;
        //anything better than find? - maybe make more efficent in future
        GameObject countryObj = GameObject.Find(country);

        if (countryObj.GetComponent<Region_Controller>().GetLocalBanshees() > 0) {
            print("-1 Banshee");

            //delete banshee black dot
            Destroy(GameObject.Find("bansheePresence_" + worldController.GetComponent<Player_Controller>().GetCountryHit().name +
            countryObj.GetComponent<Region_Controller>().GetLocalDemons().ToString()));

            countryObj.GetComponent<Region_Controller>().DecrementLocalBanshees();
            //reload ui 
            localBanshees.text = countryObj.GetComponent<Region_Controller>().GetLocalBanshees().ToString();
        }

        //increment available banshees on world controller
        worldController.GetComponent<Devil_Controller>().IncrementGlobalBanshees();
    }
}
