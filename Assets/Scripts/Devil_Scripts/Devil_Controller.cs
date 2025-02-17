using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Devil_Controller : MonoBehaviour {
    #region Declaration of global variables & objects.

    public bool isPlayerControlled;

    // Default value is 100 (percent). will be changed by research trees.
    public double _sinEfficency;

    // Resources.
    private long _skulls;
    private double _sins;

    private double _skullsMultiplier;
    private double _sinsMultiplier;

    private long _totalEvilPopulationToday;
    private long _totalEvilDiedToday;

    // The starting available units allows the game designer to change gameplay in the inspector without changing code, 
    // whilst keeping the avaialable units varaibles private. 
    public int _startingAvailableDemons;
    public int _startingAvailableBanshees;

    private int _availableDemons;
    private int _availableBanshees;

    public int _maxDeployableDemons;
    public int _maxDeployableBanshees;

    public AudioSource buyDemonAudio;

    public World_Controller world_Controller;
    //private Resource _skulls;
    //private Resource _sins;

    #endregion

    void Start() {
        _skulls = 0;
        _sins = 0.0;

        _sinEfficency = 100.0;
        _totalEvilDiedToday = 0;

        // Units available to the devil at game start.
        _availableDemons = _startingAvailableDemons;
        _availableBanshees = _startingAvailableBanshees;

        // The current maximum deployable units.
        _maxDeployableDemons = _startingAvailableDemons;
        _maxDeployableBanshees = _startingAvailableBanshees;

        _sinsMultiplier = 0.00000000005;
        _skullsMultiplier = 0.1948;

    world_Controller = this.gameObject.GetComponent<World_Controller>();
    }

    #region Get Statements
    public long GetSkulls() {
        return _skulls;
    }

    public double GetSins() {
        return _sins;
    }

    public int GetAvailableDemons() {
        return _availableDemons;
    }

    public int GetAvailableBanshees() {
        return _availableBanshees;
    }
    #endregion

    #region Increments | Decrements
    public void IncrementGlobalDemons() {
        _availableDemons++;
    }

    public void DecrementGlobalDemons() {
        _availableDemons--;
    }

    public void IncrementGlobalBanshees() {
        _availableBanshees++;
    }

    public void DecrementGlobalBanshees() {
        _availableBanshees--;
    }

    #endregion

    public void SpendSins(double sinsToSpend) {
        //Method to prevent other classes from increasing sins. 
        if(sinsToSpend > _sins) {
            throw new System.ArgumentException($"The number of sins ({_sins}) is less than the amount passed from the Devil_Research_Information class object ({sinsToSpend}).");
        }
        if(sinsToSpend < 0) {
            throw new System.ArgumentException($"The parameter passed to SpendSins ({sinsToSpend}) was below zero.");
        }
        _sins -= sinsToSpend;
    }

    /// <summary>
    /// Buy a demon by spending the skulls resource.
    /// </summary>
    public void BuyDemon() {
        if(_skulls >= 100) {
            _skulls -= 100;
            _maxDeployableDemons++;
            _availableDemons++;

            buyDemonAudio.Play();
        }
    }

    /// <summary>
    /// Calls methods to be executed daily.
    /// </summary>
    public void DailyShout() {
        // Update the daily statistics for today.
        UpdateTotalEvilPopulation();
        UpdateTotalEvilDiedToday();

        // Update the count of people in hell.
        world_Controller.hellDeathCount += _totalEvilDiedToday;

        // Update the resources based on the new statistics.
        UpdateSins();
        UpdateSkulls();

        // Reset the daily statistics.
        ResetEvilDiedTodayInEachRegion();
        ResetTotalEvilDiedToday();
    }

    /// <summary>
    /// Updates the total evil population in the world.
    /// </summary>
    private void UpdateTotalEvilPopulation() {
        _totalEvilPopulationToday = 0;

        for(int i = 0; i < (world_Controller.region_Controller.Length); i++) {
            _totalEvilPopulationToday += world_Controller.region_Controller[i].evilPop;
        }
    }

    /// <summary>
    /// Calculates and sets the total number of evil people died across the whole globe.
    /// </summary>
    public void UpdateTotalEvilDiedToday() {
        for(int i = 0; i < (world_Controller.region_Controller.Length); i++) {
            _totalEvilDiedToday += world_Controller.region_Controller[i].GetEvilDied();
        }
        Debug.Log("Todays evil deaths are: " + _totalEvilDiedToday);
    }

    /// <summary>
    /// Updates the number of sins made available to the devil faction based off the total number of evil pepople and global sin efficency (need to take into account local sin efficency?)
    /// </summary>
    private void UpdateSins() {
        _sins += (_sinEfficency * (double)_totalEvilPopulationToday * _sinsMultiplier);
    }

    /// <summary>
    /// Updates the skulls available to the devil faction based off the number of evil people that died today.
    /// </summary>
    public void UpdateSkulls() {
        _skulls += (long) ((double) _totalEvilDiedToday / 100.0 * _skullsMultiplier);
    }

    /// <summary>
    /// Resets the total number of evil people who died in a day to zero.
    /// </summary>
    private void ResetEvilDiedTodayInEachRegion() {
        for(int i = 0; i < (world_Controller.region_Controller.Length); i++) {
            // Reset death counters.
            world_Controller.region_Controller[i].ResetDeathCounterEvil();
        }
    }

    /// <summary>
    /// Sets the global daily death count to zero.
    /// </summary>
    public void ResetTotalEvilDiedToday() {
        _totalEvilDiedToday = 0;
    }

} //end class