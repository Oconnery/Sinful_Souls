using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Country_Info : MonoBehaviour{
    public Text countryName;

    public Text evilNumber;
    public Text goodNumber;
    public Text neutralNumber;

    public Text localDemons;
    public Text localAngels;

    public Text conversionGood;
    public Text conversionEvil;

    private GameObject worldContr;

    //public Text localInquisitors; -- unknown at game start
    public Text localBanshees;

    public GameObject demonDotPrefab;
    public GameObject bansheeDotPrefab;

    // Use this for initialization
    void Start(){
        countryName.text = "Country_Name";//this.gameObject.ToString();
        evilNumber.text = "XXX";
        goodNumber.text = "YYY";
        neutralNumber.text = "ZZZ";
        this.gameObject.SetActive(false);
        worldContr = GameObject.Find("GameController");
        conversionEvil.text = "100%";
        conversionGood.text = "100%";
    }

    public void ChangeCountryPanelText(GameObject country){
        countryName.text = country.name;
        evilNumber.text = "Evil: " + country.GetComponent<Region_Controller>().GetEvilPop().ToString("0");
        goodNumber.text = "Good: " + country.GetComponent<Region_Controller>().GetGoodPop().ToString("0");
        neutralNumber.text = "Neutral: " + country.GetComponent<Region_Controller>().GetNeutralPop().ToString("0");
        localDemons.text = country.GetComponent<Region_Controller>().GetLocalDemons().ToString();
        localAngels.text = country.GetComponent<Region_Controller>().GetLocalAngels().ToString();
        localBanshees.text = country.GetComponent<Region_Controller>().GetLocalBanshees().ToString();
        conversionEvil.text = (country.GetComponent<Region_Controller>().GetConversionEvil()*100).ToString() + "%";
        conversionGood.text = (country.GetComponent<Region_Controller>().GetConversionGood()*100).ToString() + "%";
    }


    public void addDemon(){
        //if theres any demons available
        if (worldContr.GetComponent<Devil_Controller>().GetAvailableDemons()>0) {
            print("+1 Demon");
            string country = this.countryName.text;
            //anything better than find? - maybe make more efficent in future
            GameObject countryObj = GameObject.Find(country);
            //Increment local demons
            countryObj.GetComponent<Region_Controller>().IncrementLocalDemons();
            //reload ui for demon .. 
            localDemons.text = countryObj.GetComponent<Region_Controller>().GetLocalDemons().ToString();

            //decrement the available demons
            //world controller demons --
            worldContr.GetComponent<Devil_Controller>().DecrementGlobalDemons();

            //add demon orange dot //instantiates prefab
            //new gameobj
            GameObject demonDot = demonDotPrefab;
            Vector3 dotLocation = new Vector3(0.0f, 0.0f, 0.0f);
            dotLocation = worldContr.GetComponent<Player_Controller>().GetRandomCountryLocale();
            demonDot = Instantiate(demonDot, dotLocation, Quaternion.identity);

            //Name the orange dot (so it can be deleted when removed) //demonPresence UNITEDSTATES1
            demonDot.name = "demonPresence_" + worldContr.GetComponent<Player_Controller>().GetCountryHit().name+ 
                countryObj.GetComponent<Region_Controller>().GetLocalDemons().ToString();
        }
    }

    public void takeAwayDemon(){
        string country = this.countryName.text;
        //anything better than find? - maybe make more efficent in future
        GameObject countryObj = GameObject.Find(country);

        if (countryObj.GetComponent<Region_Controller>().GetLocalDemons()>0){
            print("-1 Demon");

            //delete demon orange dot
            Destroy(GameObject.Find("demonPresence_" + worldContr.GetComponent<Player_Controller>().GetCountryHit().name +
            countryObj.GetComponent<Region_Controller>().GetLocalDemons().ToString()));

            //Decrement demons
            countryObj.GetComponent<Region_Controller>().DecrementLocalDemons();
            //reload ui
            localDemons.text = countryObj.GetComponent<Region_Controller>().GetLocalDemons().ToString();

            //decrement available demons on world controller
            worldContr.GetComponent<Devil_Controller>().IncrementGlobalDemons();
        }
    }

    public void addBanshee(){
        if (worldContr.GetComponent<Devil_Controller>().GetAvailableBanshees() > 0){
            print("+1 Banshee");
            string country = this.countryName.text;
            //anything better than find? - maybe make more efficent in future
            GameObject countryObj = GameObject.Find(country);
            countryObj.GetComponent<Region_Controller>().IncrementLocalBanshees();
            //reload ui 
            localBanshees.text = countryObj.GetComponent<Region_Controller>().GetLocalBanshees().ToString();

            //decrement available demons on world controller
            worldContr.GetComponent<Devil_Controller>().DecrementGlobalBanshees();

            //add banshee black dot //instantiates prefab at loc vec3 and default rot
            //new gameobj
            GameObject bansheeDot = bansheeDotPrefab;
            Vector3 dotLocation = new Vector3(0.0f, 0.0f, 0.0f);
            dotLocation = worldContr.GetComponent<Player_Controller>().GetRandomCountryLocale();
            bansheeDot = Instantiate(bansheeDot, dotLocation, Quaternion.identity);

            //Name the black dot (so it can be deleted when removed) //demonPresence UNITEDSTATES1
            bansheeDot.name = "bansheePresence_" + worldContr.GetComponent<Player_Controller>().GetCountryHit().name +
            countryObj.GetComponent<Region_Controller>().GetLocalDemons().ToString();
        }
    }

    public void TakeAwayBanshee(){
        string country = this.countryName.text;
        //anything better than find? - maybe make more efficent in future
        GameObject countryObj = GameObject.Find(country);

        if (countryObj.GetComponent<Region_Controller>().GetLocalBanshees() > 0){
            print("-1 Banshee");

            //delete banshee black dot
            Destroy(GameObject.Find("bansheePresence_" + worldContr.GetComponent<Player_Controller>().GetCountryHit().name +
            countryObj.GetComponent<Region_Controller>().GetLocalDemons().ToString()));

            countryObj.GetComponent<Region_Controller>().DecrementLocalBanshees();
            //reload ui 
            localBanshees.text = countryObj.GetComponent<Region_Controller>().GetLocalBanshees().ToString();
        }

        //increment available banshees on world controller
        worldContr.GetComponent<Devil_Controller>().IncrementGlobalBanshees();
    }
}
