using UnityEngine;
using System;

public class Clock : MonoBehaviour {
    //public static Clock instance { get; private set; } TODO: SINGLETON?
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
        // SINGLETON CODE //
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
}
