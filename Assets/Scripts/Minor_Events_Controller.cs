using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Region_Controller;

public class Minor_Events_Controller : MonoBehaviour {
    //0-25 INCLUSIVE // maybe dont even need -- could just use to print name
    public string[] regionNames = {"UNITED STATES", "MEXICO", "CANADA", "ALASKA", "ARGENTINA", "BRAZIL", "COLOMBIA", "VENEZUALA", "EASTERN EUROPE", "WESTERN EUROPE", "ICELAND", "SCANDANAVIA", "CHINA", "INDIA", "JAPAN", "KHAZAKSTAN",
        "RUSSIA", "MIDDLEEAST", "THAILAND", "CENTRAL AFRICA", "SOUTH AFRICA", "EAST AFRICA", "NORTH AFRICA", "WEST AFRICA","INDONESIA", "AUSTRALIA"};

    public Text eventsContentText;
    private string eventsDescr;

    public Hud_Controller hudController;
    private Devil_Controller devil_Controller;
    private God_Controller god_Controller;

    void Start() {
        eventsDescr = "Default text";
        eventsContentText.text = eventsDescr;

        devil_Controller = gameObject.GetComponent<Devil_Controller>();
        god_Controller = gameObject.GetComponent<God_Controller>();
    }

    private double RoundToSignificantDigits(double d, int digits) {
        if (d == 0) {
            return 0;
        }

        double scale = System.Math.Pow(10, System.Math.Floor(System.Math.Log10(System.Math.Abs(d))) + 1);
        return scale * System.Math.Round(d / scale, digits);
    }

    private void SetDeathText(Vector3 location, Text textToChange, long deathCount) {
        textToChange.gameObject.SetActive(false);
        if (deathCount > 0) {
            textToChange.gameObject.transform.position = location;
            textToChange.text = ($"{deathCount}");
            textToChange.gameObject.SetActive(true);
        } 
    }

    public void ChanceToCallRandomMinorEvent() {
        // will probably need mutual exlucsion lock/sempahore so that only one event can run at a time. if I have major events etc.
        // 1/20 days mean avg it should do a minor event once every 20 days. Would be cool to do this based upon an "activity level
        // variable, so when the user has not made many actions, and there hasn't been an event in a while, the chance for one
        // increases.
        if (Random.value > .95f) { 
            ExecuteRandomMinorEvent();
        }
    }

    private void ExecuteRandomMinorEvent() {
        this.GetComponent<World_Controller>().Pause();

        int randomCountryInt = (int)(Random.Range(0.0f, 25.0f));
        Region_Controller currentRegion = this.GetComponent<World_Controller>().region_Controller[randomCountryInt];
        string regionName = regionNames[randomCountryInt];

        if (Random.value > 0.3f)
            RandomWar(currentRegion.GetComponent<Region_Controller>(), regionName);
        else MurderousCult(currentRegion.GetComponent<Region_Controller>(), regionName);

        // Set populations and give resources.
        devil_Controller.DailyShout();
        god_Controller.DailyShout();
    }

    public void RandomWar(Region_Controller regionController, string countryObj) {
        ulong goodPeopleKilled = regionController.KillPeople(Region_Controller.Alignment.GOOD,  1000, 300000);
        ulong neutralPeopleKilled = regionController.KillPeople(Region_Controller.Alignment.NEUTRAL, 1000, 300000);
        ulong evilPeopleKilled = regionController.KillPeople(Region_Controller.Alignment.EVIL, 1000, 300000);

        eventsDescr = $"A war has broken out in {countryObj}. {evilPeopleKilled} evil people, {goodPeopleKilled} good people and {neutralPeopleKilled} neutral people have died in the fighting!";
        eventsContentText.text = eventsDescr;
        hudController.SetEventsPanelActive();
    }

    public void MurderousCult(Region_Controller regionController, string countryName) {
        ulong goodPeopleKilled = regionController.KillPeople(Alignment.GOOD, 1, 10000);

        eventsDescr = $"An evil murderous cult has appeared in {countryName}! {goodPeopleKilled} good people have been massacred!";
        eventsContentText.text = eventsDescr;
        hudController.SetEventsPanelActive();
    }
}
