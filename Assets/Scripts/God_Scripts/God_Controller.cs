using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class God_Controller : Faction {
    #region Declaration of global variables & objects.

    public bool isPlayerControlled;

    // Default value is 100 (percent). will be changed by progresing in the research tree.
    public double prayerEfficency;

    // Resources
    private ulong souls;
    private double prayers;

    private double prayersMultiplier;

    private ulong totalGoodPopulationToday;
    private ulong totalGoodDiedToday;

    public uint startingAvailableAngels;
    public uint startingAvailableInquisitors;

    private uint availableAngels;
    private uint availableInquisitors;

    public uint maxDeployableAngels;
    public uint maxDeployableInquisitors;

    public World_Controller worldController;

    #endregion

    void Start () {
        souls = 0;
        prayers = 0.0;

        prayerEfficency = 1.0;
        totalGoodDiedToday = 0;

        // Units available to God at game start.
        availableAngels = startingAvailableAngels;
        availableInquisitors = startingAvailableInquisitors;

        // The current maximum deployable unit.
        maxDeployableAngels = availableAngels;
        maxDeployableInquisitors = availableInquisitors;

        prayersMultiplier = 0;

        worldController = this.gameObject.GetComponent<World_Controller>();

        World_Controller.OnDayPassedNotifySecond += DailyShout;
    }

    #region Get Statements
    public ulong GetSouls(){
        return souls;
    }

    public double GetPrayers(){
       return prayers;
    }

    public uint GetAvailableAngels() {
        return availableAngels;
    }

    public uint GetAvailableInquisitors(){
        return availableInquisitors;
    }
    #endregion

    #region Setters
    public void SetAvailableAngels(uint availableAngels) {
        this.availableAngels = availableAngels;
    }

    public void SetAvailableInquisitors(uint availableInquisitors) {
        this.availableInquisitors = availableInquisitors;
    }
    #endregion

    #region Increments and decrements.
    public void IncrementGlobalAngels(){
        availableAngels++;
    }

    public void DecrementGlobalAngels(){
        availableAngels--;
    }

    public void IncrementGlobalInquisitors(){
        availableInquisitors++;
    }

    public void DecrementGlobalBanshees(){
        availableInquisitors--;
    }
    #endregion


    public void SpendPrayers(ulong prayersToSpend){
        //Method to prevent other classes from increasing prayers. 
        if (prayersToSpend > prayers) {
            throw new System.ArgumentException($"The number of prayers ({prayers}) is less than the amount passed from the Devil_Research_Information class object({prayersToSpend}.)");
        } else {
            prayers -= prayersToSpend;
        }
    }

    public void BuyAngel(){
        // Spend 100 prayers to purchase an extra angel.
        if(prayers>= 100){
            prayers -= 100;
            maxDeployableAngels++;
            availableAngels++;
        }
    }

    public void DailyShout(){
        // Update the daily statistics for today.
        UpdateTotalGoodPopulation();
        UpdateTotalGoodDiedToday();

        // Update the count of people in heaven.
        // TODO: Code like this is old (from 2020) and should be updated.
        worldController.heavenDeathCount += totalGoodDiedToday;

        // Update the resources based on the new statistics.
        UpdatePrayers();
        UpdateSouls();

        // Reset the daily statistics.
        ResetGoodDiedTodayInEachRegion();
        ResetGoodDailyDeathCountGlobal();
    }

    private void UpdateTotalGoodPopulation(){
        totalGoodDiedToday = 0;

        for (int i =0; i < worldController.region_Controller.Length; i++){
            totalGoodPopulationToday += worldController.region_Controller[i].GetGoodPop();
        }
    }

    private void UpdateTotalGoodDiedToday(){
        for (int i=0; i < worldController.region_Controller.Length; i++){
            totalGoodDiedToday += worldController.region_Controller[i].GetGoodDied();
        }
    }

    private void UpdatePrayers(){
        prayers += (ulong)(prayerEfficency * totalGoodPopulationToday * prayersMultiplier);
    }

    public void UpdateSouls(){
        souls += (totalGoodDiedToday / 100);
    }

    private void ResetGoodDiedTodayInEachRegion(){
        for (int i =0; i < worldController.region_Controller.Length; i++){
            worldController.region_Controller[i].ResetDeathCounterGood();
        }
    }

    public void ResetGoodDailyDeathCountGlobal(){
        totalGoodDiedToday = 0;
    }
}
