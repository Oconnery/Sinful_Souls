using System.Collections.Generic;
using UnityEngine;

public class Region_Controller : MonoBehaviour { 
    public enum Alignment{
        GOOD,
        NEUTRAL,
        EVIL,
        NONE
    }

    [SerializeField] private GameObject borderRef;

    public ulong initialTotalPopulation; // TODO: This should be placed in RegionPopulation, and it should be read in by the .txt file.
    RegionPopulation population;

    // Todo: actually give real world birth/death rate data. // TODO: Below should be private with getters/setters and should be set in world controller or similar.
    public double birthRate;
    public double deathRate;

    // CURRENTLY NOT USED /////////////////////////////////
    //private float neutralSinRate; //starts at 1 (100%) 
    //private float neutralPrayerRate; //starts at 1 (100%)
    //private float evilSinRate; //starts at 1 (100%) 
    //private float goodPrayerRate; //starts at 1 (100%)
    // ////////////////////////////////////////////////////

    // Conversion rates - the effectiveness an angel or demon will have in this region.
    private float conversionRateToEvil;
    private float conversionRateToGood;

    private double sinEfficency;

    private ushort localEvilAgents;
    private ushort localGoodAgents;
    private ushort localEvilSecondaryUnits;
    private ushort localGoodSecondaryUnits;

    //Colours according to pop 
    // TODO: New class. for this renderer colour change stuff + tidy it up //
    private Renderer renderer;
    private Color currentClr;
    private Color goodClrMax;
    private Color evilClrMax;

    public Vector3 deathTextOffset;
    [SerializeField] private Death_Text_Builder deathTextBuilder;

    public Devil_Controller devilController;
    public God_Controller godController;

    [SerializeField] private Hud_Controller hudController;
    [SerializeField] private Region_Panel_Script regionPanelScript;
    [SerializeField] private Unit_UI unitUI;


    // The alignment of the faction that has locked this region (if it is locked)
    private Alignment lockedAlignment = Alignment.NONE;

    void Awake() {
        Clock.OnDayPassedNotifyRegions += DailyCall;

        devilController.OnDailyShoutReturnPop += GetEvilPop;
        devilController.OnDailyShoutReturnDiedToday += GetEvilDied;
        devilController.OnDailyShoutResetDiedToday += ResetDeathCounterEvil;
        godController.OnDailyShoutReturnPop += GetGoodPop;
        godController.OnDailyShoutReturnDiedToday += GetGoodDied;
        godController.OnDailyShoutResetDiedToday += ResetDeathCounterGood;

        Global_Population_Viewer.OnTotalEvilPopulationRequest += GetEvilPop;
        Global_Population_Viewer.OnTotalGoodPopulationRequest += GetGoodPop;
        Global_Population_Viewer.OnTotalNeutralPopulationRequest += GetNeutralPop;

        Global_Units_Viewer.OnDeployedEvilAgentsRequest += GetLocalEvilAgents;
        Global_Units_Viewer.OnDeployedEvilSecondaryUnitsRequest += GetLocalEvilSecondaryUnits;
        Global_Units_Viewer.OnDeployedGoodAgentsRequest += GetLocalGoodAgents;
        Global_Units_Viewer.OnDeployedGoodSecondaryUnitsRequest += GetLocalGoodSecondaryUnits;

        SetInitialRates();

        population = new RegionPopulation(this);

        // TODO: file should be read in instead
        population.SetInitialPopulationAlignments(initialTotalPopulation);
        SetAlignmentColors();

        //155-50 = 105 .. 1% change in pop should change colour by 1.05%
        renderer = GetComponent<Renderer>();
        //renderer.material.color = currentClr;

        SetInitialLocalUnits();
    }

    private void SetInitialRates(){
        //neutralSinRate = 0.25f;
        //evilSinRate = 0.25f;
        //neutralPrayerRate = 0.25f;
        //goodPrayerRate = 0.25f;

        conversionRateToEvil = 1.0f;
        conversionRateToGood = 1.0f;

        sinEfficency = 1.0f;
    }

    private void SetAlignmentColors(){
        goodClrMax = new UnityEngine.Color(0.0f, (155f / 255f), 0.0f);
        evilClrMax = new UnityEngine.Color((155f / 255f), 0.0f, 0.0f);
        currentClr = new UnityEngine.Color(255f, 255f, 255f);
    }

    private void SetInitialLocalUnits(){
        localGoodAgents = 0;
        localEvilAgents = 0;
        localGoodSecondaryUnits = 0;
        localEvilSecondaryUnits = 0;
    }

    public bool isLocked(){
        return !lockedAlignment.Equals(Alignment.NONE);
    }

    #region getters
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

    public ushort GetLocalEvilAgents() {
        return localEvilAgents;
    }

    public ushort GetLocalGoodAgents(){
        return localGoodAgents;
    }

    public ushort GetLocalEvilSecondaryUnits() {
        return localEvilSecondaryUnits;
    }

    public ushort GetLocalGoodSecondaryUnits() {
        return localGoodSecondaryUnits;
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

    public double GetSinEfficency(){
        return sinEfficency;
    }
    #endregion

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
        if (localEvilSecondaryUnits >= 1) {
            rate = 0.6f;
        }

        // Apply anything else
        conversionRateToGood = rate;
    }

    void SetEvilConversionRate() {
        // Base rate
        float rate = 1.0f;
        // Apply Banshees 
        if (localGoodSecondaryUnits >= 1) {
            rate = 0.6f;
        }

        // Apply anything else
        conversionRateToEvil = rate;
    }

    public void ActivateBorder() {
        if (borderRef != null)
            borderRef.SetActive(true);

        transform.parent.GetComponent<Continent_Controller>().ActivateBorder();
    }

    public void DeactivateBorder() {
        if (borderRef != null)
            borderRef.SetActive(false);

        transform.parent.GetComponent<Continent_Controller>().DeactivateBorder();
    }

    #region increments and decrements
    public void IncrementLocalEvilAgents() {
        localEvilAgents++;
    }

    public void DecrementLocalEvilAgents() {
        localEvilAgents--;
    }

    public void IncrementLocalGoodAgents() {
        localGoodAgents++;
    }

    public void DecrementLocalGoodAgents() {
        localGoodAgents++;
    }

    public void IncrementLocalEvilSecondaryUnits() {
        localEvilSecondaryUnits++;
    }

    public void DecrementLocalEvilSecondaryUnits() {
        localEvilSecondaryUnits--;
    }

    public void IncrementLocalGoodSecondaryUnits() {
        localGoodSecondaryUnits++;
    }

    public void DecrementLocalGoodSecondaryUnits() {
        localGoodSecondaryUnits--;
    }

    #endregion

    /// <summary>
    /// Calls functions to update populations and function to change the colour of the game object renderer based off of the population.
    /// </summary>
    /// <returns></returns>
    public void DailyCall(){
        PerformDailyBirthsAndDeaths();

        // TODO: Below two methods should effectively be called at the same time. I.e. Update Good Pop should be using the temp pop values from before UpdatePopEvil() ...
        if (lockedAlignment == Alignment.NONE) {
            SetGoodConversionRate();
            SetEvilConversionRate();
            UpdatePopulation();
            // TODO: Put below into a delegate in a class called 'region renderer'.
            ChangeColour();
            // If the local population shares the same alignment, lock this region into that alignment. 
            LockRegionIfPopAlignmentIsHomogenous();
        }
    }

    private void PerformDailyBirthsAndDeaths() {
        PopulationFactionBirthsAndDeaths(population.goodPopulation);
        PopulationFactionBirthsAndDeaths(population.neutralPopulation);
        PopulationFactionBirthsAndDeaths(population.evilPopulation);
    }

    private void PopulationFactionBirthsAndDeaths(PopulationFaction populationFaction) {
        double naturalCausesDeathsD = populationFaction.GetSize() * deathRate;
        ulong naturalCausesDeaths = (ulong)naturalCausesDeathsD;

        double dBirths = populationFaction.GetSize() * birthRate;
        ulong births = (ulong)dBirths;

        // Kill people who died of natural causes according to death rate
        KillPeople(populationFaction.alignment, naturalCausesDeaths);
        // Birth people according to the birth rate. // TODO: THESE PEOPLE SHOULD BE ASSIGNED AS CLOSE TO THE SAME FRACTION OF THE CURRENT POP AS IS EVIL/NEUTRAL/GOOD as is possible.
        populationFaction.IncreaseSize(births);
    }

    private void UpdatePopulation() {
        ulong evilPopulation = population.evilPopulation.GetSize();
        ulong neutralPopulation = population.neutralPopulation.GetSize();
        ulong goodPopulation = population.goodPopulation.GetSize();

        Dictionary<Alignment, ulong> popUpdatesToEvil = CalculatePopUpdatesToEvil(neutralPopulation);
        Dictionary<Alignment, ulong> popUpdatesToGood = CalculatePopUpdatesToGood(neutralPopulation);

        // GOOD/EVIL UPDATE
        // Whichever is larger goes second
        if (popUpdatesToEvil[Alignment.GOOD] > popUpdatesToGood[Alignment.EVIL]) {
            // Find difference and just update good pop to evil.
            ulong numberToConvert = popUpdatesToEvil[Alignment.GOOD] - popUpdatesToGood[Alignment.EVIL];
            // TODO: the check vs max population should be done here. NOT below in the other methods.
            if (numberToConvert > goodPopulation){
                numberToConvert = goodPopulation;
            }
            population.Convert(numberToConvert, Alignment.GOOD, Alignment.EVIL);
        } else if (popUpdatesToEvil[Alignment.GOOD] < popUpdatesToGood[Alignment.EVIL]) {
            // Find difference and just update evil pop to good.
            ulong numberToConvert = popUpdatesToGood[Alignment.EVIL] - popUpdatesToEvil[Alignment.GOOD];
            if (numberToConvert > evilPopulation){
                numberToConvert = evilPopulation;
            }

            population.Convert(numberToConvert, Alignment.EVIL, Alignment.GOOD);
        }

        // NEUTRAL UPDATE
        if (neutralPopulation != 0) {
            if ((popUpdatesToEvil[Alignment.NEUTRAL] + popUpdatesToGood[Alignment.NEUTRAL]) < neutralPopulation) {
                population.Convert(popUpdatesToEvil[Alignment.NEUTRAL], Alignment.NEUTRAL, Alignment.EVIL);
                population.Convert(popUpdatesToGood[Alignment.NEUTRAL], Alignment.NEUTRAL, Alignment.GOOD);
            } else if (popUpdatesToEvil[Alignment.NEUTRAL] > popUpdatesToGood[Alignment.NEUTRAL]) {
                population.Convert(popUpdatesToGood[Alignment.NEUTRAL], Alignment.NEUTRAL, Alignment.GOOD);
                population.Convert(population.neutralPopulation.GetSize(), Alignment.NEUTRAL, Alignment.EVIL);
            } else {
                population.Convert(popUpdatesToEvil[Alignment.NEUTRAL], Alignment.NEUTRAL, Alignment.EVIL);
                population.Convert(population.neutralPopulation.GetSize(), Alignment.NEUTRAL, Alignment.GOOD);
            }
        }
    }

    private Dictionary<Alignment, ulong> CalculatePopUpdatesToEvil(ulong neutralPopulation) {
        // NW: Each evil agent increases evilPop by 10,000 each day.
        // This is done by converting 8000 neutral people to evil, and 2000 good people to evil.
        // TODO: create evilAgentEffectiveness and goodAgentEffectiveness variables. Use them in place of 8000 and 2000 below.
        ulong neutralConversionGoal = (uint)(8000 * localEvilAgents * conversionRateToEvil);
        ulong goodConversionGoal = (uint)(2000 * localEvilAgents * conversionRateToEvil);

        Dictionary<Alignment, ulong> populationChanges = new Dictionary<Alignment, ulong>();

        AddPopulationUpdateNeutralMapping(populationChanges, neutralPopulation, neutralConversionGoal);
        populationChanges.Add(Alignment.GOOD, goodConversionGoal);
        return populationChanges;
    }

    private Dictionary<Alignment, ulong> CalculatePopUpdatesToGood(ulong neutralPopulation) {
        ulong neutralConversionGoal = (uint)(8000 * localGoodAgents * conversionRateToGood);
        ulong evilConversionGoal = (uint)(2000 * localGoodAgents * conversionRateToGood);

        Dictionary<Alignment, ulong> populationChanges = new Dictionary<Alignment, ulong>();

        AddPopulationUpdateNeutralMapping(populationChanges, neutralPopulation, neutralConversionGoal);

        populationChanges.Add(Alignment.EVIL, evilConversionGoal);
        return populationChanges;
    }

    private Dictionary<Alignment, ulong> AddPopulationUpdateNeutralMapping(Dictionary<Alignment, ulong> populationChanges, ulong neutralPopulation, ulong neutralConversionGoal) {
        if (neutralPopulation > neutralConversionGoal || neutralConversionGoal == 0) {
            populationChanges.Add(Alignment.NEUTRAL, neutralConversionGoal);
        } else {
            populationChanges.Add(Alignment.NEUTRAL, neutralPopulation);
        }
        return populationChanges;
    }

    // If a factions population = 100%, return that alignment
    private bool LockRegionIfPopAlignmentIsHomogenous() {
        ulong totalPop = GetTotalPop();
        ulong evilPop = GetEvilPop();
        ulong goodPop = GetGoodPop();

        if (evilPop == totalPop) {
            LockRegion(Alignment.EVIL);
            return true;
        } else if (goodPop == totalPop) {
            LockRegion(Alignment.GOOD);
            return true;
        } else return false;
    }

    private void LockRegion(Alignment alignment) {
        lockedAlignment = alignment;
        ReturnAgents(alignment);
        EradicateOpposingAgents(alignment);
        // TODO: Remove this region from certain minor events?
        hudController.SetBaseUnitCountText();
    }

    private void ReturnAgents(Alignment alignment) {
        if (alignment == Alignment.GOOD) {
            ushort temp = localGoodAgents;
            localGoodAgents = 0;
            godController.ReturnLocalAgentsToGlobalPool(temp);
            if (Player_Controller.PlayingAsGod()) {
                unitUI.RemoveAllAgents(devilController, this.gameObject.name, temp);
            }
        } else if (alignment == Alignment.EVIL) {
            ushort temp = localEvilAgents;
            localEvilAgents = 0;
            devilController.ReturnLocalAgentsToGlobalPool(temp);
            if (Player_Controller.PlayingAsDevil()) {
                unitUI.RemoveAllAgents(devilController, this.gameObject.name, temp);
            }
        }
    }

    private void EradicateOpposingAgents(Alignment alignment) {
        if (alignment == Alignment.GOOD) {
            ushort temp = localGoodAgents;
            localGoodAgents = 0;
            devilController.EradicateAgents(temp);
        } else if (alignment == Alignment.EVIL) {
            ushort temp = localEvilAgents;
            localEvilAgents = 0;
            godController.EradicateAgents(temp);
        }
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
            renderer.material.color = currentClr;
        } else if (evilPopSize > goodPopSize){
            //Color it the appropriate level of red.
            double value = ((double)evilPopSize / ((double)goodPopSize + (double)evilPopSize));
            value *= evilClrMax.r;
            float valueF = (float)value;
            currentClr = new UnityEngine.Color(valueF, 0.0f, 0.0f);
            renderer.material.color = currentClr;
        } else if (evilPopSize == goodPopSize){
            // Do nothing
        } else { throw new System.Exception("The evil population or good population has been changed while ChangeColour method was running."); }
    }

    private void InstantiateDeathText(Alignment alignment, ulong deaths){
        if (deaths > 0) {
            switch (alignment) {
                case Alignment.GOOD:
                    //deathTextBuilder.InstantiateGoodDeathText(deaths, gameObject, deathTextOffset); // Only print out evil text for now. Is good text useful info. if player is devil?
                    break; // TODO: Get player faction and only instantiate death text if faction == playerFaction.
                case Alignment.EVIL:
                    deathTextBuilder.InstantiateEvilDeathText(deaths, gameObject, deathTextOffset);
                    break;
            }
        }
    }
    public ulong KillPeople(Alignment alignment, ulong numberToKill){
        PopulationFaction populationFaction = population.GetPopulationFactionByAlignment(alignment);

        ulong factionSize = populationFaction.GetSize();

        if (numberToKill > factionSize) {
            populationFaction.Kill(factionSize);            
            return factionSize;
        } else{
            populationFaction.Kill(numberToKill);
            return numberToKill;
        }
    }

    public ulong KillPeople(Alignment alignment, ulong numberToKillMinimum, ulong numberToKillMaximum){
        float numberToKillF = UnityEngine.Random.Range(numberToKillMinimum, numberToKillMaximum);
        ulong numberToKill = (ulong)numberToKillF;

        return KillPeople(alignment, numberToKill);
    }

    protected class RegionPopulation {
        public PopulationFaction evilPopulation;
        public PopulationFaction neutralPopulation;
        public PopulationFaction goodPopulation;

        public RegionPopulation(Region_Controller outerRef) {
            evilPopulation = new PopulationFaction(outerRef, Alignment.EVIL);
            neutralPopulation = new PopulationFaction(outerRef, Alignment.NEUTRAL);
            goodPopulation = new PopulationFaction(outerRef, Alignment.GOOD);
        }

        public ulong GetTotalPopulation() {
            return (evilPopulation.GetSize() + goodPopulation.GetSize() + neutralPopulation.GetSize());
        }

        public PopulationFaction GetPopulationFactionByAlignment(Alignment alignment) {
            switch (alignment) {
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

        public void SetInitialPopulationAlignments(ulong initialTotalPopulation) {
            evilPopulation.IncreaseSize(initialTotalPopulation / 10L); //10%
            goodPopulation.IncreaseSize(initialTotalPopulation / 10L); //10%
            neutralPopulation.IncreaseSize(initialTotalPopulation / 10L * 8L); //80%
        }

        public void Convert(ulong numberToConvert, Alignment from, Alignment to) {
            if (numberToConvert > 0) {
                GetPopulationFactionByAlignment(from).ReduceWithoutKilling(numberToConvert);
                GetPopulationFactionByAlignment(to).IncreaseSize(numberToConvert);
            }
        }
    }

    protected class PopulationFaction {
        private Region_Controller outerRef;
        public Alignment alignment;
        private ulong size; // TODO: Tie this in a function with death text, so you can only get this, and when you change it to a lower number the death text instantiates.
        public ulong diedToday;

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
            if (numberToKill < size) {
                size -= numberToKill;
                diedToday += numberToKill;
                outerRef.InstantiateDeathText(alignment, numberToKill);
            } else if (numberToKill == 0) {
                return;
            } else {
                ulong maxKillable = size;
                size = 0;
                diedToday += maxKillable;
                outerRef.InstantiateDeathText(alignment, maxKillable);

                Debug.LogError("The number to kill is larger than the size of this population. Not allowed. Region: " + outerRef.name + "numberToKill:  " + numberToKill + ", popSize: " + size);
            }
        }

        public void ReduceWithoutKilling(ulong reduceBy) {
            size -= reduceBy;
            // TODO: Exception if reduceBy is larger than size 
        }

        public void IncreaseSize(ulong increaseBy) {
            size += increaseBy;
        }
    }
}
