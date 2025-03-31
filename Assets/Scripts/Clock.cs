using UnityEngine;
using System;

// SINGLETON //
public class Clock : MonoBehaviour {
    //public static Clock instance { get; private set; }

    private const int numberOfCountries = 26;

    // TODO: Clock shouldnt have the below.
    // 26 country references are stored here and accessed from methods in this class and the devil/god controller classes.
    public Region_Controller[] region_Controller = new Region_Controller[numberOfCountries];

    private const float DAY_LENGTH = 3.0f;
    private static float timerMultiplier = 1.0f; // at 2.0f, this represents a double increase in speed. i.e. a 10 second day would now be a 5 second day, since the timer moves faster.
    public static ushort Day { get; private set; }
    public static ushort Month { get; private set; }
    public static ushort Year { get; private set; }

    // Don't bother with leap year for now.
    private readonly short[] daysInMonths = new short[12] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
    private static float timer;
    private static bool isTimePaused;

    // Multiple delegates/actions to enforce ordering.
    public static event Action OnDayPassedNotifyRegions;
    public static event Action OnDayPassedNotifyFactions;
    public static event Action OnDayPassedNotifyAI;
    public static event Action OnDayPassedNotifyMinorEvents;
    public static event Action OnDayPassedNotifyHUD;

    private void Awake() {
        //// If there is an instance, and it's not me, delete myself.
        //if (instance != null && instance != this) {
        //    Destroy(this);
        //} else {
        //    instance = this;
        //}
    }

    private void Start () {
        Day = (ushort) DateTime.Now.Day;
        Month = (ushort) DateTime.Now.Month;
        Year = (ushort) DateTime.Now.Year;
        
        timer = 0.0f;

        // Gives appropriate string names to the region controller which are assigned in the Unity editor.
        // TODO: This is terrible. I don't know why I did this way back then but it's terrible.
        for (int i = 0; i < region_Controller.Length; i++){
            region_Controller[i].name = this.GetComponent<Minor_Events_Controller>().regionNames[i];
        }

        isTimePaused = false;
    }

    private void Update(){
        if (!isTimePaused){
            timer += Time.deltaTime * timerMultiplier;

            if (timer >= DAY_LENGTH){
                timer = 0.0f;
                OnDayPassedNotifyRegions?.Invoke(); // The same as doing a null check 
                OnDayPassedNotifyFactions?.Invoke();
                OnDayPassedNotifyAI?.Invoke();
                OnDayPassedNotifyMinorEvents?.Invoke();
                OnDayPassedNotifyHUD?.Invoke();

                try {
                    if (Day == daysInMonths[(Month-1)]){
                        // New month.
                        Day = 1;
                        if (Month == 12){
                            // Happy new year.
                            Month = 1;
                            Year++;
                        }
                        else{
                            Month += 1;
                        }
                    } else {
                        // A day has passed.
                        Day++;
                    }
                } catch  (System.IndexOutOfRangeException E) {
                    Debug.LogError($"The public starting variables for day, month year need to be set in the World_Controller script on the gameobject {gameObject}");
                }
            }
        }
    }

    /// <summary>
    /// Pauses the game.
    /// </summary>
    public static void Pause() {
        isTimePaused = true;
    }

    /// <summary>
    /// Unpauses the game and resets the multiplier for daily clock timer speed.
    /// </summary>
    public static void UnpauseResetSpeed() {
        timerMultiplier = 1.0f;
        isTimePaused = false;
    }

    /// <summary>
    /// Unpauses the game and resumes at the clock speed it was set to before the pause.
    /// </summary>
    public static void Unpause() {
        isTimePaused = false;
    }

    public static void FastForward() {
        // Increase game speed.
        if (timerMultiplier < 32.0f)
            timerMultiplier *= 2.0f;

        // If the game is paused, unpause.
        if (isTimePaused)
            isTimePaused = false;
    }

    public static bool GetIsTimePaused() {
        return isTimePaused;
    }

    // TODO: Move below to somewhere else (a static populations_viewer?)
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
            deployedDemons += region_Controller[i].GetLocalEvilAgents();
        }

        return deployedDemons;
    }

    public ushort GetDeployedBanshees() {
        ushort deployedBanshees = 0;

        for (int i = 0; i < region_Controller.Length; i++) {
            deployedBanshees += region_Controller[i].GetLocalEvilSecondaryUnits();
        }

        return deployedBanshees;
    }

    public ushort GetDeployedAngels() {
        ushort deployedAngels = 0;

        for (int i = 0; i < region_Controller.Length; i++) {
            deployedAngels += region_Controller[i].GetLocalGoodAgents();
        }

        return deployedAngels;
    }

    public ushort GetDeployedInquisitors() {
        ushort deployedInquisitors = 0;

        for (int i = 0; i < region_Controller.Length; i++) {
            deployedInquisitors += region_Controller[i].GetLocalGoodSecondaryUnits();
        }

        return deployedInquisitors;
    }
}
