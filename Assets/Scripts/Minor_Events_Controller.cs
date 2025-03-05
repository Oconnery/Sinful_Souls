using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum Alignment { good, neutral, evil }

// Should be broken down into more classes.

public class Minor_Events_Controller : MonoBehaviour {
    //0-25 INCLUSIVE // maybe dont even need -- could just use to print name
    public string[] regionNames = {"UNITED STATES", "MEXICO", "CANADA", "ALASKA", "ARGENTINA", "BRAZIL", "COLOMBIA", "VENEZUALA", "EASTERN EUROPE", "WESTERN EUROPE", "ICELAND", "SCANDANAVIA", "CHINA", "INDIA", "JAPAN", "KHAZAKSTAN",
        "RUSSIA", "MIDDLEEAST", "THAILAND", "CENTRAL AFRICA", "SOUTH AFRICA", "EAST AFRICA", "NORTH AFRICA", "WEST AFRICA","INDONESIA", "AUSTRALIA"};

    public Text eventsContentText;
    private string eventsDescr;

    // TODO: These should be instantiated each time at the country level. It shouldn't be displayed here or have anything to do with this class.
    //public Text evilDeathText;
    //public Text goodDeathText;
    //public Text neutralDeathText;

    public Hud_Controller hudController;
    private Devil_Controller devil_Controller;
    private God_Controller god_Controller;

    void Start() {
        eventsDescr = "Default text";
        eventsContentText.text = eventsDescr;

        devil_Controller = gameObject.GetComponent<Devil_Controller>();
        god_Controller = gameObject.GetComponent<God_Controller>();
    }

    /*
    a function that takes in a float number arguement with a good/evil tag(will be called from events_controller script) and prints it to the players screen
    there could be multiple called at the same time so they shouldn't overlap. therefore a new game object should be created every time; 
    the location at which the game object spawns should depend on how many are spawning. it should fade and remove after a day in game.
    the function has to be called here in this class. this means there needs to be a reference to the hud class.
    
        
        //round the float
        static number in class x; 
        start {
            x=0;
        }

        //

        function should look like: printDeadCountry(float number, char alignment ){
            
        }

        override function should look like printDeadCountry(float number1, char alignment1, number2, char alignment2){
        //for good and evil 
            
        }

        -- cant create own data types in C#
        - in c# I dont actually need override keyword
    */

    /// <summary>
    /// Returns the given DOUBLE to a desired number of significant figures
    /// </summary>
    /// <param name="d"> The double for which a sig digit rounded is required </param>
    /// <param name="digits"> The number of digits to be rounded to </param>
    /// <returns>Double</returns>
    private double RoundToSignificantDigits(double d, int digits) {
        if (d == 0) {
            return 0;
        }

        double scale = System.Math.Pow(10, System.Math.Floor(System.Math.Log10(System.Math.Abs(d))) + 1);
        return scale * System.Math.Round(d / scale, digits);
    }

    // Prints the number of people who died, with floating and slowly dissapearing text (to be called for a specific country whenever people die).
    private void PrintCountryDeath(Vector3 location, long evilDeathCount, long goodDeathCount, long neutralDeathCount) {
        //// Change location and Set to active
        //SetDeathText(location, evilDeathText, evilDeathCount);
        //SetDeathText(location, goodDeathText, goodDeathCount);
        //SetDeathText(location, neutralDeathText, neutralDeathCount);
        ////double evilDead = (double)evilDeathCount;
        ////evilDead = RoundToSignificantDigits(evilDead, 3);

        ////BUG - The location is the location on the map, not the HUD/canvas.

    }

    private void SetDeathText(Vector3 location, Text textToChange, long deathCount) {
        textToChange.gameObject.SetActive(false);
        if (deathCount > 0) {
            textToChange.gameObject.transform.position = location;
            textToChange.text = ($"{deathCount}");
            textToChange.gameObject.SetActive(true);
        } 
    }

    /// <summary>
    /// 10% chance to call a random minor event.
    /// </summary>
    public void ChanceToCallRandomMinorEvent() {
        //will probably need mutual exlucsion lock/sempahore so that only one event can run at a time. if I have major events etc.
        if (Random.value > .95f) { //1/20 days mean avg it should do a minor event
            ExecuteRandomMinorEvent();
        }
    }

    /// <summary>
    /// Calls a random minor event.
    /// </summary>
    private void ExecuteRandomMinorEvent() {
        Region_Controller currentRegion;
        string regionName;

        // Pause the game 
        this.GetComponent<World_Controller>().Pause();

        // Randomly select the country in which an event will happen.
        int randomNumber = (int)(Random.Range(0.0f, 25.0f));
        print("random Number is: " + randomNumber);

        // Save info on random country locally.
        currentRegion = this.GetComponent<World_Controller>().region_Controller[randomNumber];
        regionName = regionNames[randomNumber];

        // Chances of different minor events. // This can be made much much better.
        if (Random.value > 0.3f)
            RandomWar(currentRegion.GetComponent<Region_Controller>(), regionName);
        else MurderousCult(currentRegion.GetComponent<Region_Controller>(), regionName);

        // Set populations and give resources.
        devil_Controller.DailyShout();
        god_Controller.DailyShout();
    }

    /// <summary>
    /// Kills a random amount of people in a specified country. configures and activates events panel.
    /// </summary>
    /// <param name="country">The country object of region_controller.</param>
    /// <param name="countryObj">The string name of the country.</param>
    public void RandomWar(Region_Controller country, string countryObj) {
        float killNeutralNumberF = Random.Range(1000.0f, 300000.0f);
        long killNeutralNumber = (long)killNeutralNumberF;

        float killEvilNumberF = Random.Range(1000f, 300000.0f);
        long killEvilNumber = (long)killEvilNumberF;

        float killGoodNumberF = Random.Range(1000f, 300000.0f);
        long killGoodNumber = (long)killGoodNumberF;

        KillPeople(Alignment.good, killGoodNumber, country);
        KillPeople(Alignment.neutral, killNeutralNumber, country);
        KillPeople(Alignment.evil, killEvilNumber, country);

        //set event panel
        hudController.SetEventsPanelActive();
        eventsDescr = $"A war has broken out in {countryObj}. {killEvilNumber} evil people, {killGoodNumber} good people and {killNeutralNumber} neutral people have died in the fighting!";
        eventsContentText.text = eventsDescr;

        PrintCountryDeath(country.gameObject.transform.position, killEvilNumber, killNeutralNumber, killGoodNumber);
    }

    /// <summary>
    /// Kills a bunch of good people in a given country. Appropriately configures and displays event panel.
    /// </summary>
    /// <param name="country">The country object of region_controller.</param>
    /// <param name="countryName">The string name of the country.</param>
    public void MurderousCult(Region_Controller country, string countryName) {
        //should probably these a method for use in more events .. CallFunction(float range from, float range to, country)
        float killGoodNumberF = Random.Range(1.0f, 10000.0f);
        long killGoodNumber = (long)killGoodNumberF;
        if (killGoodNumber > country.goodPop) { killGoodNumber = country.goodPop; }

        // TODO: None of this should be changed here. This should all be done in the regionController. goodPop, population and goodDiedToday should not be changeable outside of the one 
        country.goodPop -= killGoodNumber;
        country.population -= killGoodNumber;
        country.goodDiedToday = killGoodNumber;

        //set event panel
        hudController.SetEventsPanelActive();

        //configure events text (should maybe come before activating
        eventsDescr = $"An evil murderous cult has appeared in {countryName}! {killGoodNumber} good people have been massacred!";
        eventsContentText.text = eventsDescr;
    }


    /// <summary>
    /// Calls methods to kill people of a given alignment.
    /// </summary>
    /// <param name="alignment"> The alignment of the population to be killed.</param>
    /// <param name="numberToKill"> The number of people to be killed.</param>
    /// <param name="country"> The country in which the people will be killed.</param>
    private void KillPeople(Alignment alignment, long numberToKill, Region_Controller country) {
        if (numberToKill < 0) {
            throw new System.ArgumentException("The number of people to be killed is less than zero.");
        }
        // Calls the methods to kill a certain alignment of people.
        if (alignment == Alignment.good) {
            KillGoodPeople(numberToKill, country);
        } else if (alignment == Alignment.neutral) {
            KillNeutralPeople(numberToKill, country);
        } else if (alignment == Alignment.evil) {
            KillEvilPeople(numberToKill, country);
        }
    }

    /// <summary>
    /// Kill a number of good people in a country.
    /// </summary>
    /// <param name="numberToKill"> THe number of people to be killed.</param>
    /// <param name="country">The country in which the killing will take place.</param>
    private void KillGoodPeople(long numberToKill, Region_Controller country) {
        if (numberToKill < country.goodPop) {
            country.goodPop -= numberToKill;
            country.population -= numberToKill;
            country.goodDiedToday += numberToKill;
        }
        else {
            // The number to be killed is higher than the number of same aligned people in the country.
            long numberKilled = country.goodPop;
            country.goodPop = 0;
            country.population -= numberKilled;
            country.goodDiedToday += numberKilled;
        }
    }

    /// <summary>
    /// Kill a number of neutral people in a country.
    /// </summary>
    /// <param name="numberToKill"> The number of people to be killed.</param>
    /// <param name="country"> The country in which the killing is to take place.</param>
    private void KillNeutralPeople(long numberToKill, Region_Controller country) {
        if (numberToKill < country.neutralPop) {
            country.neutralPop -= numberToKill;
            country.population -= numberToKill;
            // No need for neutralDiedToday since it doesn't have effect on any resources.
        }
        else {
            //
            long numberKilled = country.neutralPop;
            country.neutralPop = 0;
            country.population -= numberKilled;
        }
    }

    /// <summary>
    /// Kill a number of evil people in a country.
    /// </summary>
    /// <param name="numberToKill"> The number of people to be killed.</param>
    /// <param name="country"> The country in which the killing is to take place.</param>
    private void KillEvilPeople(long numberToKill, Region_Controller country) {
        if (numberToKill < country.evilPop) {
            country.evilPop -= numberToKill;
            country.population -= numberToKill;
            country.evilDiedToday += numberToKill;
        }
        else {
            long numberKilled = country.evilPop;
            country.evilPop = 0;
            country.population -= numberKilled;
            country.evilDiedToday += numberToKill;
        }
    }
}