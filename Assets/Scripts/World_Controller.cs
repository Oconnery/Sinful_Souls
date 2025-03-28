﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

// TODO: Change this from world controller to clock controller or similar. lots of things should be moved out of here.
public class World_Controller : MonoBehaviour {
    private const int numberOfCountries = 26;

    // 26 country references are stored here and accessed from methods in this class and the devil/god controller classes.
    public Region_Controller[] region_Controller = new Region_Controller[numberOfCountries];

    private const float DAY_LENGTH = 3.0f;
    private float daytimerMultiplier = 1.0f; // at 2.0f, this represents a double increase in speed. i.e. a 10 second day would now be a 5 second day, since the timer moves faster.
    private short day;
    private short month;
    private short year;
    
    // The number of days in each month
    // A data structure called dateTimer?
    // Make a daily caller class?
    private short[] daysInMonth;
    private float timer;
    private bool isTimePaused;

    public ulong hellDeathCount;
    public ulong heavenDeathCount;

    private Devil_Controller devil_Controller;
    private God_Controller god_Controller;

    public Text skullsSoulsText;
    public Text sinsPrayersText;
    public Text sinEfficencyText;
    public Text currentDateText;
    public Text hellCountText;
    public Text heavenCountText;

    public Text baseUnitCountText;
    public Text specialUnitCountText;

    // Multiple delegates/actions to enforce ordering.
    public static event Action OnDayPassedNotifyFirst;
    public static event Action OnDayPassedNotifySecond;
    public static event Action OnDayPassedNotifyThird;
    public static event Action OnDayPassedNotifyFourth;

    void Start () {
        // Find devil and god controllers.
        devil_Controller = this.gameObject.GetComponent<Devil_Controller>();
        god_Controller = this.gameObject.GetComponent<God_Controller>();

        day = (short) DateTime.Now.Day;
        month = (short) DateTime.Now.Month;
        year = (short) DateTime.Now.Year;
        
        // Don't bother with leap year for now.
        daysInMonth = new short[12] {31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        timer = 0.0f;

        //_evilDeathCount = 0;

        // Gives appropriate string names to the region controller which are assigned in the Unity editor.
        for (int i = 0; i < region_Controller.Length; i++){
            region_Controller[i].name = this.GetComponent<Minor_Events_Controller>().regionNames[i];
        }

        isTimePaused = false;

        // Below should probably be a in a HUD class.
        hellDeathCount = 0;
        heavenDeathCount = 0;

        // Setup main HUD.
        SetupHudText();
    }

    // Update is called once per frame 
    private void Update(){
        if (!isTimePaused){
            timer += Time.deltaTime * daytimerMultiplier;

            //update demon banshees text available every frame -- better to only do this whenever player clicks
            if (devil_Controller.isPlayerControlled){
                SetSkullsSoulsText(); // skulls need to update every frame because of events - TODO: better to just update after events click. + at day end.
                SetSinsPrayersText();// sins becuase of spending in research tree -- TODO: better to only do this after clicking research button and whenever people are killed.
            }
            else if (god_Controller.isPlayerControlled){
                // TODO
            }

            // So instead it should actually be in the faction class, or a similar interface. "Update resource texts". Or cant I just update the resource texts
            // whenever the resource is changed?

            if (timer >= DAY_LENGTH){
                // A day has passed.
                day++;
                Debug.Log("A day has passed.");
                timer = 0.0f;
                OnDayPassedNotifyFirst?.Invoke(); // The same as doing a null check
                OnDayPassedNotifySecond?.Invoke();
                OnDayPassedNotifyThird?.Invoke();
                OnDayPassedNotifyFourth?.Invoke();

                // TODO: This HUD stuff shouldn't be in this class.
                SetHUD();
                try{
                    if (day == daysInMonth[(month-1)]){
                        // New month.
                        day = 0;
                        if (month == 12){
                            // Happy new year.
                            month = 1;
                            year++;
                        }
                        else{
                            month += 1;
                        }
                    }// End month check if statment.
                } catch  (System.IndexOutOfRangeException E) {
                    Debug.LogError($"The public starting variables for day, month year need to be set in the World_Controller script on the gameobject {gameObject}");
                }
            }// End timer day length if statement.
        }// End if time is not paused.
    }// End fixedupdate.

    // TODO: Should these get total population and get deployed demons methods really be in this class? Probably not, specially if I rename it to clock controller.
    public ulong GetTotalEvilPopulation(){
        ulong evilPopulation = 0;

        for (int i = 0; i < region_Controller.Length; i++) {
            evilPopulation += region_Controller[i].GetEvilPop();
        }

        return evilPopulation;
    }

    public ulong GetTotalGoodPopulation(){
        ulong goodPopulation = 0;

        for (int i = 0; i < region_Controller.Length; i++) {
            goodPopulation += region_Controller[i].GetGoodPop();
        }

        return goodPopulation;
    }

    public ulong GetTotalNeutralPopulation(){
        ulong neutralPopulation = 0;

        for (int i = 0; i < region_Controller.Length; i++) {
            neutralPopulation += region_Controller[i].GetNeutralPop();
        }

        return neutralPopulation;
    }

    public ushort GetDeployedDemons() {
        ushort deployedDemons = 0;

        for (int i = 0; i < region_Controller.Length; i++) {
            deployedDemons += region_Controller[i].GetLocalDemons();
        }

        return deployedDemons;
    }

    public ushort GetDeployedBanshees() {
        ushort deployedBanshees = 0;

        for (int i = 0; i < region_Controller.Length; i++) {
            deployedBanshees += region_Controller[i].GetLocalBanshees();
        }

        return deployedBanshees;
    }

    public ushort GetDeployedAngels() {
        ushort deployedAngels = 0;

        for (int i = 0; i < region_Controller.Length; i++) {
            deployedAngels += region_Controller[i].GetLocalAngels();
        }

        return deployedAngels;
    }

    public bool GetIsTimePaused(){
        return isTimePaused;
    }

    /// <summary>
    /// Pauses the game.
    /// </summary>
    public void Pause(){
        isTimePaused = true;
    }

    /// <summary>
    /// Unpauses the game and resets the multiplier for daily clock timer speed.
    /// </summary>
    public void UnpauseResetSpeed(){
        daytimerMultiplier = 1.0f;
        isTimePaused = false;
    }

    /// <summary>
    /// Unpauses the game and resumes at the clock speed it was set to before the pause.
    /// </summary>
    public void Unpause(){
        isTimePaused = false;
    }

    public void FastForward(){
        // Increase game speed.
        if (daytimerMultiplier < 32.0f)
            daytimerMultiplier *= 2.0f;

        // If the game is paused, unpause.
        if (isTimePaused)
            isTimePaused = false;
    }

    // TODO: This definitely should not be here.
    /// <summary>
    /// Setup general HUD text and player faction HUD text.
    /// </summary>
    private void SetupHudText(){
        skullsSoulsText.text = "0";
        sinsPrayersText.text = "Sins: 0";
        sinEfficencyText.text = "100%";
        //newsFlashText.text = "NOTHING HAPPENED TODAY!";
        currentDateText.text = "0" + day + "/0" + month + "/" + year;

        if (devil_Controller.isPlayerControlled){
            // Initalizes the text values displayed on the main hud in the devil played scene.
            baseUnitCountText.text = ($"{devil_Controller.GetAvailableDemons()} / {devil_Controller.maxDeployableDemons}");
            specialUnitCountText.text = ($"{devil_Controller.GetAvailableBanshees()} / {devil_Controller.maxDeployableBanshees}");
        }
        else if (god_Controller.isPlayerControlled){
            baseUnitCountText.text = ($"{god_Controller.GetAvailableAngels()} / {god_Controller.maxDeployableAngels}");
            specialUnitCountText.text = ($"{god_Controller.GetAvailableInquisitors()} / {god_Controller.maxDeployableInquisitors}");
        }
        else throw new System.Exception("The isPlayerControlled boolean for both God and Devil controllers is set to false.");
    }


    //TODO: This neither.
    /// <summary>
    /// Start HUD methods.
    /// </summary>
    private void SetHUD(){
        SetDateText();
        SetHellText();
        SetHeavenText();
        SetSinsPrayersEfficencyText();
    }

    // TODO: Or this.
    private void SetDateText(){
        //Check if single digit = needs 0 
        if (day < 10){
            currentDateText.text = "0" + day + "/"; //+ month + "/" + year;
        }
        else{
            currentDateText.text = day + "/";
        }

        if (month < 10){
            currentDateText.text += "0" + month + "/";
        }
        else{
            currentDateText.text += month + "/";
        }
        currentDateText.text += year;
    }

    // TODO: Or this.
    public void SetSkullsSoulsText(){
    // Todo: instead of ifs like the below, I should do playerControllerFaction.IsPlayerControlled() or 
    // similar. 

        //Need to round like the sins
        if(devil_Controller.isPlayerControlled){
            skullsSoulsText.text = devil_Controller.GetSkulls().ToString();
        } else if (god_Controller.isPlayerControlled){

        } else{throw new System.Exception("The player isn't controlling the god or devil class."); }
    }

    // TODO: Or this.
    private void SetSinsPrayersText(){
        if (devil_Controller.isPlayerControlled){
            sinsPrayersText.text = "Sins: " + Math.Floor(devil_Controller.GetSins()) + " Million";
        }
        else if (god_Controller.isPlayerControlled){

        }
        else {throw new System.Exception("The player isn't controlling the god or devil class."); }
    }

    // TODO: Or this.
    private void SetSinsPrayersEfficencyText(){
        if (devil_Controller.isPlayerControlled){
            sinEfficencyText.text = (devil_Controller.sinEfficency).ToString("F0");
        } else if (god_Controller.isPlayerControlled){

        }
    }

    // TODO: Or this.
    private void SetHellText(){
        hellCountText.text = ($"Hell: {hellDeathCount}");
    }

    // TODO: Or this.
    private void SetHeavenText(){
        heavenCountText.text = ($"Heaven: {heavenDeathCount}");
    }

    // TODO: Or this.
    //TODO: These should not be in this class. Should probably be in the faction class.
    public void SetBaseUnitCountText(){
        baseUnitCountText.text = ($"{devil_Controller.GetAvailableDemons()} / {devil_Controller.maxDeployableDemons}");
    }

    // TODO: Or this.
    public void SetSpecialUnitCountText(){
        specialUnitCountText.text = ($"{devil_Controller.GetAvailableBanshees()} / {devil_Controller.maxDeployableBanshees}");
    }
    // End HUD methods
}
