using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Region_Controller : MonoBehaviour {

    //statistics for the region
    //population
    public long population;
    public long evilPop;
    public long neutralPop;
    public long goodPop;

    public long evilDiedToday; //should always be positive
    public long goodDiedToday;

    // Rates at which people in the region sin/pray.
    // Neutral
    private float sinRateN; //starts at 1 (100%) 
    private float prayerRateN; //starts at 1 (100%)

    // Good and evil 
    private float sinRateE; //starts at 1 (100%) 
    private float prayerRateG; //starts at 1 (100%)

    // Conversion rates - the effectiveness an angel or demon will have in this region.
    private float evilConversionRate;
    private float goodConversionRate; //change this to conversionToGoodRate

    private short localDemons;
    private short localAngels;
    private short localBanshees;
    private short localInquisitors;

    // The strength of a banshee/inquisitor in this region.
    // Hasn't been added into code yet.
    private float inquisitorStength;
    private float bansheeStrength;

    //My world region pops
    /*
        7.5 billion people total

        579 million North America 
        //US = 330 million
        //Mexico = 211 million
        //Canada = 37 million
        //Alaska = 1 million

        422 million South America
        //Argentina = 91 million
        //Brazil = 209 million
        //Colombia = 88 million
        //Venezuala = 34 million

        1216 million Africa
        CENTRAL = 270 million
        SOUTH = 201 million
        EAST = 174 million
        NORTH = 180 million
        WEST = 391 million

        741 million Europe
        //Iceland = 5 million
        //Scandinavia = 50 million
        //Western = 425 million
        //Eastern = 261 million

        4463 million Asia
        //China = 1400 million
        //India = 1595 million
        //Japan = 203 million
        //Russia = 145 million
        //Thailand = 279 million
        //Khazakstan = 66 million 
        //MiddleEast = 775 million

        79 million Oceania 
        //Australia 35 million
        //Indonesia 44 million

        ==7.5 billion total
    */

    //Colours according to pop
    private Renderer rend;
    private Color currentClr;
    private Color goodClrMax;
    private Color evilClrMax;

    // Use this for initialization
    void Start() {
        sinRateN = 1.0f;
        sinRateE = 1.0f;
        prayerRateN = 1.0f;
        prayerRateG = 1.0f;

        evilConversionRate = 1.0f;
        goodConversionRate = 1.0f;

        evilPop = (population / 10); //10%
        goodPop = (population / 10); //10%
        neutralPop = (population / 10 * 8); //80%

        goodClrMax = new Color(0.0f, (155f / 255f), 0.0f);
        evilClrMax = new Color((155f / 255f), 0.0f, 0.0f);
        currentClr = new Color(0.0f, 0.0f, 0.0f, 1.0f);

        //155-50 = 105 .. 1% change in pop should change colour by 1.05%

        rend = GetComponent<Renderer>();
        rend.material.color = currentClr;

        //set the length to world controller length
        //dayLength = world_controller.getDayLength();

        localAngels = 0;
        localDemons = 0;
        localInquisitors = 0;
        localBanshees = 0;

        inquisitorStength = 0.4f;

        evilDiedToday = 0;
        goodDiedToday = 0;
    }

    #region gets
    public float GetGoodPop() {
        return goodPop;
    }

    public float GetEvilPop() {
        return evilPop;
    }

    public float GetNeutralPop() {
        return neutralPop;
    }

    public float GetTotalPop() {
        return population;
    }

    public short GetLocalDemons() {
        return localDemons;
    }

    public short GetLocalAngels() {
        return localAngels;
    }

    public short GetLocalBanshees() {
        return localBanshees;
    }
    public long GetEvilDied(){
        //this should return the number of evil people that died in one day. 
        //WHERE EVIL POP IS EVER CHANGED THE AMOUNT OF EVILDEAD SHOULD ALSO CHANGE
        return evilDiedToday;
    }

    public long GetGoodDied() {
        return goodDiedToday;
    }

    public float GetConversionEvil(){
        return evilConversionRate;
    }

    public float GetConversionGood(){
        return goodConversionRate;
    }

    public void ResetDeathCounterEvil(){
        evilDiedToday = 0;
    }

    public void ResetDeathCounterGood(){
        goodDiedToday = 0;
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
    public void CallDaily(){
        UpdatePopEvil();
        UpdatePopGood();
        ChangeColour();
        //return evil pop. its then added in world contr
    }

    /// <summary>
    /// Changes the colour of the renderer for this game object depending on the proportions of good and evil people.
    /// </summary>
    void ChangeColour(){
        if (evilPop<goodPop){
            //Color it the appropriate level of green.
            double value = ((double)goodPop / ((double)goodPop + (double)evilPop));
            value *= goodClrMax.g;
            float valueF =(float) value;
            currentClr = new Color(0.0f, valueF, 0.0f);
            rend.material.color = currentClr;
        } else if (evilPop>goodPop){
            //Color it the appropriate level of red.
            double value = ((double)evilPop /((double)goodPop+(double)evilPop));
            value *= evilClrMax.r;
            float valueF = (float)value;
            currentClr = new Color(valueF, 0.0f, 0.0f);
            rend.material.color = currentClr;
        } else if (evilPop == goodPop){ 
            // Do nothing?
        } else { throw new System.Exception("The evil population or good population has been changed while ChangeColour method was running."); }
    }

    /// <summary>
    /// Updates the local evil population based off of the number of demons and inquisitors in the local area.
    /// </summary>
    void UpdatePopEvil(){
        for (int i=0; i<localInquisitors; i++){
            evilConversionRate -= 0.4f; 
            if (evilConversionRate < 0){
              throw new System.Exception($"The evilConversionRate for {this.name} is below zero ({evilConversionRate}).");
            }
        }
        // NW: Each demon increases evilPop by 10,000 each day.
        // This is done by converting 8000 neutral people to evil, and 2000 good people to evil.
        int neutralNumberToConvert = (int)(8000* evilConversionRate);
        int goodNumberToConvert = (int)(2000* evilConversionRate); 

        for (int i=0; i< localDemons; i++){
            if (evilPop < population){
                // Convert neutral people to evil.
                if (neutralPop > neutralNumberToConvert){
                    // Decrease neutral. Increase evil.
                    neutralPop -= neutralNumberToConvert;
                    evilPop += neutralNumberToConvert;
                } else{
                    // Convert as many neutral people as there are.
                    long neutralPopTemp = neutralPop;
                    neutralPop = 0;
                    evilPop += neutralPopTemp;
                }

                // Convert good people to evil.
                if (goodPop > goodNumberToConvert){
                    // Decrease good. Increase evil.
                    goodPop -= goodNumberToConvert;
                    evilPop += goodNumberToConvert;
                }
                else if (goodPop > 0){
                    evilPop += goodPop;
                    goodPop = 0;
                }
            }//end if evil pop < pop check
            else {
                print("Demons in " + this.name + " aren't converting people");
            }
            //after make variables for effictiveness so the banshee and inquisitor can have effects 
        }
    }

    /// <summary>
    /// Updates the local good population based off of the number of angels and banshees in the lcoal area.
    /// </summary>
    void UpdatePopGood(){
        for (int i = 0; i < localBanshees; i++){
            goodConversionRate = 1.0f;
            goodConversionRate -= 0.4f;
            if (goodConversionRate < 0){
                throw new System.Exception($"The goodConversionRate for {this.name} is below zero ({goodConversionRate}).");
            }
        }
        // NW: Each angel increases goodPop by 10,000 each day.
        // This is done by converting 8000 neutral people to good, and 2000 evil people to good.
        int neutralNumberToConvert = (int)(8000 * goodConversionRate);
        int evilNumberToConvert = (int)(2000 * goodConversionRate);

        for (int i = 0; i < localAngels; i++){
            if (goodPop < population){
                // Convert neutral people to good.
                if (neutralPop > neutralNumberToConvert){
                    // Decrease neutral. Increase good.
                    neutralPop -= neutralNumberToConvert;
                    goodPop += neutralNumberToConvert;
                }
                else{
                    // Convert as many neutral people as there are.
                    long neutralPopTemp = neutralPop;
                    neutralPop = 0;
                    goodPop += neutralPopTemp;
                }

                // Convert evil people to good.
                if (evilPop > evilNumberToConvert){
                    // Decrease good. Increase evil.
                    evilPop -= evilNumberToConvert;
                    goodPop += evilNumberToConvert;
                }
                else if (evilPop > 0){
                    evilPop += goodPop;
                    goodPop = 0;
                }
            }//end if evil pop < pop check
            else{
                print("Demons in " + this.name + " aren't converting people");
            }
        }
    }

}