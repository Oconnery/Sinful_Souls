using UnityEngine;
using System;
using System.Collections.Generic;

public class Clock : MonoBehaviour {
    private const float DAY_LENGTH = 3.0f;
    public static float TimerMultiplier { get; private set; } // at 2.0f, this represents a double increase in speed. i.e. a 10 second day would now be a 5 second day, since the timer moves faster.
    public static byte Day { get; private set; }
    public static byte Month { get; private set; }
    public static byte Year { get; private set; }
    public static bool IsTimePaused { get; private set; }

    // Don't bother with leap year for now.
    private readonly IReadOnlyList<byte> daysInMonths = Array.AsReadOnly(new byte[12] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 });

    private static float timer;
  
    // Multiple delegates/actions to enforce ordering.
    public static event Action OnDayPassedNotifyRegions;
    public static event Action OnDayPassedNotifyFactions;
    public static event Action OnDayPassedNotifyAI;
    public static event Action OnDayPassedNotifyMinorEvents;
    public static event Action OnDayPassedNotifyHUD;

    public static event Action OnPause;
    public static event Action OnUnpause;

    // do I need one for pause?

    private void Awake() {
        Day = (byte)DateTime.Now.Day;
        Month = (byte)DateTime.Now.Month;
        Year = (byte)DateTime.Now.Year;

        TimerMultiplier = 1.0f;
        timer = 0.0f;
        IsTimePaused = false;
    }

    private void Update() {
        if (!IsTimePaused) {
            timer += Time.deltaTime * TimerMultiplier;
            if (timer >= DAY_LENGTH) {
                timer = 0.0f;
                AddDayToDate();

                OnDayPassedNotifyRegions?.Invoke(); // The same as doing a null check 
                OnDayPassedNotifyFactions?.Invoke();
                OnDayPassedNotifyAI?.Invoke();
                OnDayPassedNotifyMinorEvents?.Invoke();
                OnDayPassedNotifyHUD?.Invoke();
            }
        }
    }

    private void AddDayToDate(){
        try {
            if (Day == daysInMonths[(Month - 1)]) {
                // New month.
                SetDateToNewMonth();
            } else {
                Day++;
            }
        } catch (IndexOutOfRangeException E) {
            Debug.LogError($"The public starting variables for day, month year need to be set in the World_Controller script on the gameobject {gameObject}");
        }
    }

    private void SetDateToNewMonth(){
        Day = 1;
        if (Month == 12) {
            SetDateToNewYear();
        } else {
            Month += 1;
        }
    }

    private void SetDateToNewYear(){
        // Happy new year.
        Month = 1;
        Year++;
    }

    /// <summary>
    /// Pauses the game.
    /// </summary>
    public static void Pause() {
        Debug.Log("Pause");
        IsTimePaused = true;
        OnPause?.Invoke();
    }

    /// <summary>
    /// Unpauses the game and resets the multiplier for daily clock timer speed.
    /// </summary>
    public static void UnpauseResetSpeed() {
        Debug.Log("Unpause");
        IsTimePaused = false;
        TimerMultiplier = 1.0f;
        OnUnpause?.Invoke();
    }

    /// <summary>
    /// Unpauses the game and resumes at the clock speed it was set to before the pause.
    /// </summary>
    public static void Unpause() {
        Debug.Log("Unpause");
        IsTimePaused = false;
        OnUnpause?.Invoke();
    }

    public static void FastForward() {
        // If the game is paused, unpause.
        if (IsTimePaused) {
            IsTimePaused = false;
            OnUnpause?.Invoke();
        }

        // Increase game speed.
        if (TimerMultiplier < 32.0f)
            TimerMultiplier *= 2.0f;
    }
}
