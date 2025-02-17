using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class World_Controller : MonoBehaviour {
    // A script containing things that affect the world as a whole.
    private const int numberOfCountries = 26;

    // 26 country references are stored here and accessed from methods in this class and the devil/god controller classes.
    public Region_Controller[] region_Controller = new Region_Controller[numberOfCountries];

    private const float DAY_LENGTH = 3.0f;
    private short _day;
    private short _month;
    private short _year;
    
    // The number of days in each month
    // A data structure called dateTimer?
    // Make a daily caller class?
    private short[] _daysInMonth;
    private float _timer;
    public bool _isTimePaused;

    public long hellDeathCount;
    public long heavenDeathCount;

    // Reference to the god and devil controllers
    private Devil_Controller devil_Controller;
    private God_Controller god_Controller;

    //HUD references (assigned to the game object this class is implemented onto (game controller)
    public Text skullsSoulsText;
    public Text sinsPrayersText;
    public Text sinEfficencyText;
    //public Text newsFlashText;
    public Text currentDateText;
    public Text hellCountText;
    public Text heavenCountText;

    public Text baseUnitCountText;
    public Text specialUnitCountText;

    // Get a reference to all of the continent controllers.

    private AI_Controller ai_Controller;

    void Start () {
        // Find devil and god controllers.
        devil_Controller = this.gameObject.GetComponent<Devil_Controller>();
        god_Controller = this.gameObject.GetComponent<God_Controller>();

        ai_Controller = this.gameObject.GetComponent<AI_Controller>();

        _day = (short) DateTime.Now.Day;
        _month = (short)DateTime.Now.Month;
        _year = (short)DateTime.Now.Year;
        
        // Don't bother with leap year.
        _daysInMonth = new short[12] {31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        _timer = 0.0f;

        //_evilDeathCount = 0;

        // Gives appropriate string names to the region controller which are assigned in the Unity editor.
        for (int i = 0; i < region_Controller.Length; i++){
            region_Controller[i].name = this.GetComponent<Minor_Events_Controller>().regionNames[i];
        }

        _isTimePaused = false;

        hellDeathCount = 0;
        heavenDeathCount = 0;

        // Setup main HUD.
        SetupHudText();
    }

    // Update is called once per frame -- fixed == xseconds .. 0.02seconds is the default .. can change with Time.fixedDeltaTime
    private void FixedUpdate(){
        if (!_isTimePaused){
            _timer += Time.deltaTime;

            //update demon banshees text available every frame -- better to only do this whenever player clicks
            SetBaseUnitCountText();// only after clicking reaserch tree banshee ++ or when an event adds a banshee
            SetSpecialUnitCountText();// same .. except I could incorporate people dying (saying 1% of population die every year). -- so would need to be called every frame.

            if (devil_Controller.isPlayerControlled){
                SetSkullsSoulsText(); // skulls need to update every frame because of events - better to just update after events click. + at day end.
                SetSinsPrayersText();//sins becuase of spending in research tree -- better to only do this after clicking research button and whenever people are killed.
            }
            else if (god_Controller.isPlayerControlled){
                // Update
            }

            if (_timer >= DAY_LENGTH){
                if (_timer > (DAY_LENGTH+0.02f)){
                    throw new System.Exception($"The timer ({_timer})has exceeded the length of the day add allowance for minimal allowance ({DAY_LENGTH})");
                }
                // A day has passed.
                _day++;
                Debug.Log("A day has passed.");

                // Reset timer 
                _timer = 0.0f;

                // Update the populations in the whole world.
                UpdateAllRegionStatistics();

                // Set the ownership and control level of the continents 


                // Call the methods to set resources.
                devil_Controller.DailyShout();
                god_Controller.DailyShout();
                ai_Controller.AIDailyActions();

                // Possibility of a random minor event.
                this.GetComponent<Minor_Events_Controller>().ChanceToCallRandomMinorEvent();

                SetHUD();
                try{
                if (_day == _daysInMonth[(_month-1)]){
                    // New month.
                    _day = 0;
                    if (_month == 12){
                        // Happy new year.
                        _month = 1;
                        _year++;
                    }
                    else{
                        _month += 1;
                    }
                }// End month check if statment.
                }
                catch  (System.IndexOutOfRangeException E) {
                    Debug.LogError($"The public starting variables for day, month year need to be set in the World_Controller script on the gameobject {gameObject}");
                }
            }// End timer day length if statement.
        }// End if time is not paused.
    }// End fixedupdate.

    /// <summary>
    /// Updates the neutral, evil and good populations in the region.
    /// </summary>
    private void UpdateAllRegionStatistics(){
        for (int i = 0; i < region_Controller.Length; i++){
            region_Controller[i].CallDaily();
        }
    }

    private void UpdateContinentsClassifications() {
       
    }

    /// <summary>
    /// Pauses the game.
    /// </summary>
    public void Pause(){
        if (_isTimePaused)
        {
            throw new System.Exception("Pause was called whenever the game is already paused");
        }
        _isTimePaused = true;
    }

    /// <summary>
    /// Unpauses the game.
    /// </summary>
    public void UnPause(){
        if (!_isTimePaused)
        {
            throw new System.Exception("Unpause was called whenever the game is already unpaused");
        }
        _isTimePaused = false;
    }

    /// <summary>
    /// Setup general HUD text and player faction HUD text.
    /// </summary>
    private void SetupHudText(){
        skullsSoulsText.text = "0";
        sinsPrayersText.text = "Sins: 0";
        sinEfficencyText.text = "100%";
        //newsFlashText.text = "NOTHING HAPPENED TODAY!";
        currentDateText.text = "0" + _day + "/0" + _month + "/" + _year;

        if (devil_Controller.isPlayerControlled){
            // Initalizes the text values displayed on the main hud in the devil played scene.
            baseUnitCountText.text = ($"{devil_Controller.GetAvailableDemons()} / {devil_Controller._maxDeployableDemons}");
            specialUnitCountText.text = ($"{devil_Controller.GetAvailableBanshees()} / {devil_Controller._maxDeployableBanshees}");
        }
        else if (god_Controller.isPlayerControlled){
            baseUnitCountText.text = ($"{god_Controller._availableAngels} / {god_Controller._maxDeployableAngels}");
            specialUnitCountText.text = ($"{god_Controller._availableInquisitors} / {god_Controller._maxDeployableInquisitors}");
        }
        else throw new System.Exception("The isPlayerControlled boolean for both God and Devil controllers is set to false.");
    }

    /// <summary>
    /// Start HUD methods.
    /// </summary>
    private void SetHUD(){
        SetDateText();
        SetHellText();
        SetHeavenText();
        SetSinsPrayersEfficencyText();
    }

    private void SetDateText(){
        //Check if single digit = needs 0 
        if (_day < 10){
            currentDateText.text = "0" + _day + "/"; //+ month + "/" + year;
        }
        else{
            currentDateText.text = _day + "/";
        }

        if (_month < 10){
            currentDateText.text += "0" + _month + "/";
        }
        else{
            currentDateText.text += _month + "/";
        }

        currentDateText.text += _year;
    }

    private void SetSkullsSoulsText(){
        //Need to round like the sins
        if(devil_Controller.isPlayerControlled){
            skullsSoulsText.text = devil_Controller.GetSkulls().ToString();
        } else if (god_Controller.isPlayerControlled){

        } else{throw new System.Exception("The player isn't controlling the god or devil class."); }
    }

    private void SetSinsPrayersText(){
        if (devil_Controller.isPlayerControlled){
            sinsPrayersText.text = "Sins: " + Math.Floor(devil_Controller.GetSins()) + " Million";
        }
        else if (god_Controller.isPlayerControlled){

        }
        else {throw new System.Exception("The player isn't controlling the god or devil class."); }
    }

    private void SetSinsPrayersEfficencyText(){
        if (devil_Controller.isPlayerControlled){
            sinEfficencyText.text = (devil_Controller._sinEfficency).ToString("F0");
        } else if (god_Controller.isPlayerControlled){

        }
    }

    private void SetHellText(){
        hellCountText.text = ($"Hell: {hellDeathCount}");
    }

    private void SetHeavenText(){
        heavenCountText.text = ($"Heaven: {heavenDeathCount}");
    }

    private void SetBaseUnitCountText(){
        baseUnitCountText.text = ($"{devil_Controller.GetAvailableDemons()} / {devil_Controller._maxDeployableDemons}");
    }

    private void SetSpecialUnitCountText(){
        specialUnitCountText.text = ($"{devil_Controller.GetAvailableBanshees()} / {devil_Controller._maxDeployableBanshees}");
    }
    // End HUD methods
}
