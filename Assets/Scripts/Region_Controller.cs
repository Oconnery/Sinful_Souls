using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static System.Net.Mime.MediaTypeNames;

public class Region_Controller : MonoBehaviour {

    public enum Alignment{
        GOOD,
        NEUTRAL,
        EVIL
    }

    protected class PopulationFaction {
        private Region_Controller outerRef;
        public Alignment alignment;
        private ulong size; // TODO: Tie this in a function with death text, so you can only get this, and when you change it to a lower number the death text instantiates.
        public ulong diedToday; // should always be positive

        // Since I'm not using diedToday for neutral pop, I could create a class/struct that extends PopulationFaction 
        // which contains diedToday and should only be used for good/evil pop, or something similar to this.

        public PopulationFaction(Region_Controller outerRef, Alignment alignment) {
            this.outerRef = outerRef;
            this.alignment = alignment;
            size = 0;
            diedToday = 0;
        }
        
        public ulong GetSize() {
            return size;
        }

        public void Kill(ulong numberToKill) {
            size -= numberToKill;
            diedToday += numberToKill;
            outerRef.InstantiateDeathText(alignment, numberToKill);
            // TODO: Exception if numberToKill is larger than size
        }

        public void ReduceWithoutKilling(ulong reduceBy) {
            size -= reduceBy;
            // TODO: Exception if reduceBy is larger than size 
        }

        public void IncreaseSize(ulong increaseBy) {
            size += increaseBy;
        }
    }

    protected class RegionPopulation{
        public PopulationFaction evilPopulation;
        public PopulationFaction neutralPopulation;
        public PopulationFaction goodPopulation;

        public RegionPopulation(Region_Controller outerRef){
            evilPopulation = new PopulationFaction(outerRef, Alignment.EVIL);
            neutralPopulation = new PopulationFaction(outerRef, Alignment.NEUTRAL);
            goodPopulation = new PopulationFaction(outerRef, Alignment.GOOD);
        }

        public ulong GetTotalPopulation(){
            return (evilPopulation.GetSize() + goodPopulation.GetSize() + neutralPopulation.GetSize());
        }

        public PopulationFaction GetPopulationFactionByAlignment(Alignment alignment){
            switch (alignment){
                case Alignment.GOOD:
                    return goodPopulation;
                case Alignment.NEUTRAL:
                    return neutralPopulation;
                case Alignment.EVIL:
                    return evilPopulation;
                default:
                    throw new System.NotImplementedException("This alignment either has not been setup properly yet, or null was passed to GetPopulationFactionByAlignment function. Alignment: " + alignment.ToString());
            }
        }

        public void SetInitialPopulationAlignments(ulong initialTotalPopulation){
            evilPopulation.IncreaseSize(initialTotalPopulation / 10L); //10%
            goodPopulation.IncreaseSize(initialTotalPopulation / 10L); //10%
            neutralPopulation.IncreaseSize(initialTotalPopulation / 10L * 8L); //80%
        }

        public void Convert(ulong numberToConvert, Alignment from, Alignment to) {
            GetPopulationFactionByAlignment(from).ReduceWithoutKilling(numberToConvert);
            GetPopulationFactionByAlignment(to).IncreaseSize(numberToConvert);
        }
    }

    public Transform canvasRefTransform;

    public ulong initialTotalPopulation; // TODO: This should be placed in RegionPopulation, and it should be read in by the .txt file.
    RegionPopulation population;

    // Todo: actually give real world birth/death rate data. // TODO: Below should be private with getters/setters and should be set in world controller or similar.
    public double birthRate;
    public double deathRate;

    private float neutralSinRate; //starts at 1 (100%) 
    private float neutralPrayerRate; //starts at 1 (100%)
    private float evilSinRate; //starts at 1 (100%) 
    private float goodPrayerRate; //starts at 1 (100%)

    // Conversion rates - the effectiveness an angel or demon will have in this region.
    private float conversionRateToEvil;
    private float conversionRateToGood;

    private ushort localDemons;
    private ushort localAngels;
    private ushort localBanshees;
    private ushort localInquisitors;

    //Colours according to pop
    private Renderer rend;
    private UnityEngine.Color currentClr;
    private UnityEngine.Color goodClrMax;
    private UnityEngine.Color evilClrMax;

    public GameObject evilTextPrefab;
    public GameObject goodTextPrefab;

    public Vector3 evilDeathTextLocalPos;
    public Vector3 goodDeathTextLocalPos;

    // Use this for initialization
    void Start() {
        SetInitialRates();

        population = new RegionPopulation(this);

        // TODO: file should be read in instead
        population.SetInitialPopulationAlignments(initialTotalPopulation);
        SetAlignmentColors();

        //155-50 = 105 .. 1% change in pop should change colour by 1.05%
        rend = GetComponent<Renderer>();
        rend.material.color = currentClr;

        SetInitialLocalUnits();
    }

    private void SetInitialRates(){
        neutralSinRate = 0.25f;
        evilSinRate = 0.25f;
        neutralPrayerRate = 0.25f;
        goodPrayerRate = 0.25f;

        conversionRateToEvil = 1.0f;
        conversionRateToGood = 1.0f;
    }

    private void SetAlignmentColors(){
        goodClrMax = new UnityEngine.Color(0.0f, (155f / 255f), 0.0f);
        evilClrMax = new UnityEngine.Color((155f / 255f), 0.0f, 0.0f);
        currentClr = new UnityEngine.Color(0.0f, 0.0f, 0.0f, 1.0f);
    }

    private void SetInitialLocalUnits(){
        localAngels = 0;
        localDemons = 0;
        localInquisitors = 0;
        localBanshees = 0;
    }

    #region gets
    public ulong GetGoodPop() {
        return population.goodPopulation.GetSize();
    }

    public ulong GetEvilPop() {
        return population.evilPopulation.GetSize();
    }

    public ulong GetNeutralPop() {
        return population.neutralPopulation.GetSize();
    }

    public ulong GetTotalPop() {
        return population.GetTotalPopulation();
    }

    public ushort GetLocalDemons() {
        return localDemons;
    }

    public ushort GetLocalAngels(){
        return localAngels;
    }

    public void IncrementLocalAngels(){
        localAngels++;
    }

    public ushort GetLocalBanshees() {
        return localBanshees;
    }
    public ulong GetEvilDied(){
        return population.evilPopulation.diedToday;
    }

    public ulong GetGoodDied() {
        return population.goodPopulation.diedToday;
    }

    public float GetConversionEvil(){
        return conversionRateToEvil;
    }

    public float GetConversionGood(){
        return conversionRateToGood;
    }

    // TODO: World controller really shouldn't be doing this. 
    public void ResetDeathCounterEvil(){
        population.evilPopulation.diedToday = 0;
    }

    public void ResetDeathCounterGood(){
        population.goodPopulation.diedToday = 0;
    }

    void SetGoodConversionRate() {
        // Base rate
        float rate = 1.0f;
        // Apply Banshees 
        if (localBanshees >= 1) {
            rate = 0.6f;
        }

        // Apply anything else
        conversionRateToGood = rate;
    }

    void SetEvilConversionRate() {
        // Base rate
        float rate = 1.0f;
        // Apply Banshees 
        if (localInquisitors >= 1) {
            rate = 0.6f;
        }

        // Apply anything else
        conversionRateToEvil = rate;
    }

    #endregion

    #region increments
    public void IncrementLocalBanshees(){
        //called when + is clicked.
        //if the total available angels>0
        localBanshees++;
        //Decrement the total number of available angels
    }

    public void DecrementLocalBanshees(){
        //called when + is clicked.
        localBanshees--;
        //add 1 to the total number of available angels (world controller or player controller)
    }

    public void IncrementLocalDemons(){
        localDemons++;
    }

    public void DecrementLocalDemons(){
        localDemons--;
    }

    #endregion

    /// <summary>
    /// Calls functions to update populations and function to change the colour of the game object renderer based off of the population.
    /// </summary>
    /// <returns></returns>
    public void DailyCall(){
        PerformDailyBirthsAndDeaths();
        // TODO: Below two methods should effectively be called at the same time. I.e. Update Good Pop should be using the temp pop values from before UpdatePopEvil() ...
        UpdatePopEvil();
        UpdatePopGood();
        ChangeColour();
    }

    private void PerformDailyBirthsAndDeaths() {
        PopulationFactionBirthsAndDeaths(population.goodPopulation);
        PopulationFactionBirthsAndDeaths(population.neutralPopulation);
        PopulationFactionBirthsAndDeaths(population.evilPopulation);
    }

    /// <summary>
    /// Updates the local evil population based off of the number of demons and inquisitors in the local area.
    /// </summary>
    private void UpdatePopEvil(){
        SetEvilConversionRate();
        // NW: Each demon increases evilPop by 10,000 each day.
        // This is done by converting 8000 neutral people to evil, and 2000 good people to evil.
        ulong neutralNumberToConvert = (uint)(8000* conversionRateToEvil);
        ulong goodNumberToConvert = (uint)(2000* conversionRateToEvil); 

        for (int i=0; i< localDemons; i++){
            ulong evilPopulation = population.evilPopulation.GetSize();
            ulong neutralPopulation = population.neutralPopulation.GetSize();
            ulong goodPopulation = population.goodPopulation.GetSize();

            if (evilPopulation < population.GetTotalPopulation()){
                // Convert neutral people to evil.
                if (neutralPopulation > neutralNumberToConvert){
                    population.Convert(neutralNumberToConvert, Alignment.NEUTRAL, Alignment.EVIL);
                } else{
                    // Convert as many neutral people as there are.
                    population.Convert(neutralPopulation, Alignment.NEUTRAL, Alignment.EVIL);
                }

                // Convert good people to evil.
                if (goodPopulation > goodNumberToConvert){
                    population.Convert(goodNumberToConvert, Alignment.GOOD, Alignment.EVIL);
                }
                else if (population.goodPopulation.GetSize() > 0){
                    population.Convert(goodPopulation, Alignment.GOOD, Alignment.EVIL);
                }
            }//end if evil pop < pop check
            else {
                print("Demons in " + this.name + " aren't converting people");
            }
            //after make variables for effictiveness so the banshee and inquisitor can have effects 
        }
    }

    private void UpdatePopGood() {
        SetGoodConversionRate();

        // NW: Each angel increases goodPop by 10,000 each day.
        // This is done by converting 8000 neutral people to good, and 2000 evil people to good.
        ulong neutralNumberToConvert = (uint)(8000 * conversionRateToGood);
        ulong evilNumberToConvert = (uint)(2000 * conversionRateToGood);

        for (int i = 0; i < localAngels; i++) {
            ulong evilPopulation = population.evilPopulation.GetSize();
            ulong neutralPopulation = population.neutralPopulation.GetSize();
            ulong goodPopulation = population.goodPopulation.GetSize();

            if (goodPopulation < population.GetTotalPopulation()) {
                // Convert neutral people to good.
                if (neutralPopulation > neutralNumberToConvert) {
                    population.Convert(neutralNumberToConvert, Alignment.NEUTRAL, Alignment.GOOD);
                } else {
                    // Convert as many neutral people as there are.
                    population.Convert(neutralPopulation, Alignment.NEUTRAL, Alignment.GOOD);
                }

                // Convert evil people to good.
                if (evilPopulation > evilNumberToConvert) {
                    population.Convert(evilNumberToConvert, Alignment.EVIL, Alignment.GOOD);
                } else if (population.goodPopulation.GetSize() > 0) {
                    population.Convert(goodPopulation, Alignment.EVIL, Alignment.GOOD);
                }
            }
            else {
                print("Angels in " + this.name + " aren't converting people");
            }
        }
    }

    private void PopulationFactionBirthsAndDeaths(PopulationFaction populationFaction){
        double naturalCausesDeathsD = populationFaction.GetSize() * deathRate;
        ulong naturalCausesDeaths = (ulong) naturalCausesDeathsD;

        double dBirths = populationFaction.GetSize() * birthRate;
        ulong births = (ulong)dBirths;

        // Kill people who died of natural causes according to death rate
        KillPeople(populationFaction.alignment, naturalCausesDeaths);
        // Birth people according to the birth rate. // TODO: THESE PEOPLE SHOULD BE ASSIGNED AS CLOSE TO THE SAME FRACTION OF THE CURRENT POP AS IS EVIL/NEUTRAL/GOOD as is possible.
        populationFaction.IncreaseSize(births);
    }

    /// <summary>
    /// Changes the colour of the renderer for this game object depending on the proportions of good and evil people.
    /// </summary>
    private void ChangeColour(){
        ulong goodPopSize = population.goodPopulation.GetSize();
        ulong evilPopSize = population.evilPopulation.GetSize();

        if (evilPopSize < goodPopSize){
            //Color it the appropriate level of green.
            double value = ((double)goodPopSize / ((double)goodPopSize + (double)evilPopSize));
            value *= goodClrMax.g;
            float valueF = (float)value;
            currentClr = new UnityEngine.Color(0.0f, valueF, 0.0f);
            rend.material.color = currentClr;
        } else if (evilPopSize > goodPopSize){
            //Color it the appropriate level of red.
            double value = ((double)evilPopSize / ((double)goodPopSize + (double)evilPopSize));
            value *= evilClrMax.r;
            float valueF = (float)value;
            currentClr = new UnityEngine.Color(valueF, 0.0f, 0.0f);
            rend.material.color = currentClr;
        } else if (evilPopSize == goodPopSize){
            // Do nothing
        } else { throw new System.Exception("The evil population or good population has been changed while ChangeColour method was running."); }
    }

    private void InstantiateDeathText(Alignment alignment, ulong deaths){
        switch (alignment){
            case Alignment.GOOD:
                InstantiateGoodDeathText(deaths);
                break;
            case Alignment.EVIL:
                InstantiateEvilDeathText(deaths);
                break;
        }
    }

    private void InstantiateGoodDeathText(ulong deaths){
        Vector3 initialPos = transform.position + evilDeathTextLocalPos; 
        initialPos.z = 0;

        Debug.Log(name + " localPosition variable is: " + initialPos.x + ", " + initialPos.y + ", " + initialPos.z);
        // add the evilDeathTextLocalPos, or convert evilDeathTextLocalPos to local pos from world space and then add them?

        GameObject textGO = Instantiate(evilTextPrefab, initialPos, Quaternion.identity, canvasRefTransform);
        textGO.SetActive(false);
        textGO.GetComponent<TextMeshProUGUI>().text = deaths.ToString();
        textGO.name += "_" + name;
        textGO.SetActive(true);
    }

    private void InstantiateEvilDeathText(ulong deaths){
        Vector3 initialPos = transform.position + goodDeathTextLocalPos;
        initialPos.z = 0;

        Debug.Log(name + " localPosition variable is: " + initialPos.x + ", " + initialPos.y + ", " + initialPos.z);
        // add the evilDeathTextLocalPos, or convert evilDeathTextLocalPos to local pos from world space and then add them?

        GameObject textGO = Instantiate(goodTextPrefab, initialPos, Quaternion.identity, canvasRefTransform);
        textGO.SetActive(false);
        textGO.GetComponent<TextMeshProUGUI>().text = deaths.ToString();
        textGO.name += "_" + name;
        textGO.SetActive(true);
    }

    public ulong KillPeople(Alignment alignment, ulong numberToKill){
        PopulationFaction populationFaction = population.GetPopulationFactionByAlignment(alignment);

        ulong factionSize = populationFaction.GetSize();

        if (numberToKill > factionSize) {
            populationFaction.Kill(factionSize);            
            return factionSize;
        }
        else{
            populationFaction.Kill(numberToKill);
            return numberToKill;
        }
    }

    public ulong KillPeople(Alignment alignment, ulong numberToKillMinimum, ulong numberToKillMaximum){
        float numberToKillF = UnityEngine.Random.Range(numberToKillMinimum, numberToKillMaximum);
        ulong numberToKill = (ulong)numberToKillF;

        return KillPeople(alignment, numberToKill);
    }
}