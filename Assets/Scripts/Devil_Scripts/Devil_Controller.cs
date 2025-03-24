using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Devil controller and God controller should both have a shared parent class, or interface at least.
public class Devil_Controller : MonoBehaviour {
    #region Declaration of global variables & objects.

    public bool isPlayerControlled;

    // Default value is 100 percent i.e. 1.0f. will be changed by research trees.
    public double sinEfficency;

    // Resources.
    private long skulls;
    private double sins;

    private double skullsMultiplier;
    private double sinsMultiplier;

    private ulong totalEvilPopulationToday;
    private ulong totalEvilDiedToday;

    // The starting available units allows the game designer to change gameplay in the inspector without changing code, 
    // whilst keeping the avaialable units varaibles private. 
    public int startingAvailableDemons;
    public int startingAvailableBanshees;

    private int availableDemons;
    private int availableBanshees;

    public int maxDeployableDemons;
    public int maxDeployableBanshees;

    public AudioSource buyDemonAudio;

    public World_Controller world_Controller;
    //private Resource _skulls;
    //private Resource _sins;

    #endregion

    void Start() {
        skulls = 0;
        sins = 0.0;

        sinEfficency = 1.0f;
        totalEvilDiedToday = 0;

        // Units available to the devil at game start.
        availableDemons = startingAvailableDemons;
        availableBanshees = startingAvailableBanshees;

        // The current maximum deployable units.
        maxDeployableDemons = startingAvailableDemons;
        maxDeployableBanshees = startingAvailableBanshees;

        sinsMultiplier = 0.00000000005;
        skullsMultiplier = 0.1948;

    world_Controller = this.gameObject.GetComponent<World_Controller>();
    }

    #region Get Statements
    public long GetSkulls() {
        return skulls;
    }

    public double GetSins() {
        return sins;
    }

    public int GetAvailableDemons() {
        return availableDemons;
    }

    public int GetAvailableBanshees() {
        return availableBanshees;
    }
    #endregion

    #region Increments | Decrements
    public void IncrementGlobalDemons() {
        availableDemons++;
    }

    public void DecrementGlobalDemons() {
        availableDemons--;
    }

    public void IncrementGlobalBanshees() {
        availableBanshees++;
    }

    public void DecrementGlobalBanshees() {
        availableBanshees--;
    }

    #endregion

    public void SpendSins(double sinsToSpend) {
        //Method to prevent other classes from increasing sins. 
        if(sinsToSpend > sins) {
            throw new System.ArgumentException($"The number of sins ({sins}) is less than the amount passed from the Devil_Research_Information class object ({sinsToSpend}).");
        }
        if(sinsToSpend < 0) {
            throw new System.ArgumentException($"The parameter passed to SpendSins ({sinsToSpend}) was below zero.");
        }
        sins -= sinsToSpend;
    }

    /// <summary>
    /// Buy a demon by spending the skulls resource.
    /// </summary>
    public void BuyDemon() {
        if(skulls >= 100) {
            skulls -= 100;
            maxDeployableDemons++;
            availableDemons++;

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
        world_Controller.hellDeathCount += totalEvilDiedToday;

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
        totalEvilPopulationToday = 0;

        for(int i = 0; i < (world_Controller.region_Controller.Length); i++) {
            totalEvilPopulationToday += world_Controller.region_Controller[i].GetEvilPop();
        }
    }

    /// <summary>
    /// Calculates and sets the total number of evil people died across the whole globe.
    /// </summary>
    public void UpdateTotalEvilDiedToday() {
        for(int i = 0; i < (world_Controller.region_Controller.Length); i++) {
            totalEvilDiedToday += world_Controller.region_Controller[i].GetEvilDied();
        }
        Debug.Log("Todays evil deaths are: " + totalEvilDiedToday);
    }

    /// <summary>
    /// Updates the number of sins made available to the devil faction based off the total number of evil pepople and global sin efficency (need to take into account local sin efficency?)
    /// </summary>
    private void UpdateSins() {
        sins += (sinEfficency * (double)totalEvilPopulationToday * sinsMultiplier);
    }

    /// <summary>
    /// Updates the skulls available to the devil faction based off the number of evil people that died today.
    /// </summary>
    public void UpdateSkulls() {
        skulls += (long) ((double) totalEvilDiedToday / 100.0 * skullsMultiplier);
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
        totalEvilDiedToday = 0;
    }

} //end class