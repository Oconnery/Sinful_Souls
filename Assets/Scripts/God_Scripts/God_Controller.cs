﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class God_Controller : MonoBehaviour {
    #region Declaration of global variables & objects.

    public bool isPlayerControlled;

    // Default value is 100 (percent). will be changed by progresing in the research tree.
    public double _prayerEfficency;

    // Resources
    private long _prayers;
    private long _souls;

    private double _prayersMultiplier;

    private long _totalGoodPopulationToday;
    private long _totalGoodDeathCountToday;

    public int _availableAngels;
    public int _availableInquisitors;

    public int _maxDeployableAngels;
    public int _maxDeployableInquisitors;

    public int _startingAvailableAngels;
    public int _startingAvailableInquisitors;

    public World_Controller world_Controller;

    #endregion

    void Start () {
        _souls = 0;
        _prayers = 0;

        _prayerEfficency = 100.0;
        _totalGoodDeathCountToday = 0;

        // Units available to God at game start.
        _availableAngels = _startingAvailableAngels;
        _availableInquisitors = _startingAvailableInquisitors;

        // The current maximum deployable unit.
        _maxDeployableAngels = _availableAngels;
        _maxDeployableInquisitors = _availableInquisitors;

        _prayersMultiplier = 0;

        world_Controller = this.gameObject.GetComponent<World_Controller>();
	}

    #region Get Statements
    public long GetSouls(){
        return _souls;
    }

    public long GetPrayers(){
       return _prayers;
    }

    public int GetAvailableAngels(){
        return _availableAngels;
    }

    public int GetAvailableInquisitors(){
        return _availableInquisitors;
    }
    #endregion

    #region Increments and decrements.
    public void IncrementGlobalAngels(){
        _availableAngels++;
    }

    public void DecrementGlobalAngels(){
        _availableAngels--;
    }

    public void IncrementGlobalInquisitors(){
        _availableInquisitors++;
    }

    public void DecrementGlobalBanshees(){
        _availableInquisitors--;
    }
    #endregion

    /// <summary>
    /// Reduces the prayers resource.
    /// </summary>
    /// <param name="prayersToSpend">The number of prayers that should be spent.</param>
    public void SpendPrayers(long prayersToSpend){
        //Method to prevent other classes from increasing prayers. 
        if (prayersToSpend> _prayers){
            throw new System.ArgumentException($"The number of prayers ({_prayers}) is less than the amount passed from the Devil_Research_Information class object({prayersToSpend}.)");
        }
        if (prayersToSpend<0){
            throw new System.ArgumentException($"The parameter passed to SpendPrayers ({prayersToSpend}) was below zero.");
        }
        _prayers -= prayersToSpend;
    }

    /// <summary>
    /// Buy an angel by spending the prayers resource.
    /// </summary>
    public void BuyAngel(){
        // Spend 100 prayers to purchase an extra angel.
        if(_prayers>= 100){
            _prayers -= 100;
            _maxDeployableAngels++;
            _availableAngels++;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void DailyShout(){
        // Update the daily statistics for today.
        UpdateTotalGoodPopulation();
        UpdateGoodDeathCountGlobal();

        // Update the count of people in hell.
        world_Controller.hellDeathCount += _totalGoodDeathCountToday;

        // Update the resources based on the new statistics.
        UpdatePrayers();
        UpdateSouls();

        // Reset the daily statistics.
        ResetGoodDiedTodayInEachRegion();
        ResetGoodDailyDeathCountGlobal();
    }

    /// <summary>
    /// Updates the total evil population in the world.
    /// </summary>
    private void UpdateTotalGoodPopulation(){
        _totalGoodDeathCountToday = 0;

        for (int i =0; i < (world_Controller.region_Controller.Length); i++){
            _totalGoodPopulationToday += world_Controller.region_Controller[i].evilPop;
        }
    }

    /// <summary>
    /// Calculates and sets the total number of good people died across the whole globe.
    /// </summary>
    private void UpdateGoodDeathCountGlobal(){
        for (int i=0; i < (world_Controller.region_Controller.Length); i++){
            _totalGoodDeathCountToday += world_Controller.region_Controller[i].GetGoodDied();
        }
    }

    /// <summary>
    /// Updates the number of prayers made available to the god faction based off the total number of good people and global sin efficency (need to take into account local sin efficency?)  
    /// </summary>
    private void UpdatePrayers(){
        _prayers += (long)(_prayerEfficency * _totalGoodPopulationToday * _prayersMultiplier);
    }

    /// <summary>
    /// Updates the souls available to the God faction based off the number of evil people that died today.
    /// </summary>
    public void UpdateSouls(){
        _souls += (_totalGoodDeathCountToday / 100);
    }

    /// <summary>
    /// Resets the total number of good people who died in a day to zero.
    /// </summary>
    private void ResetGoodDiedTodayInEachRegion(){
        for (int i =0; i < (world_Controller.region_Controller.Length); i++){
            world_Controller.region_Controller[i].ResetDeathCounterGood();
        }
    }

    /// <summary>
    /// Sets the good global death count to zero
    /// </summary>
    public void ResetGoodDailyDeathCountGlobal(){
        _totalGoodDeathCountToday = 0;
    }

}