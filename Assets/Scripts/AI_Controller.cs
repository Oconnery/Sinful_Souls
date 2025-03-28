using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO; Instead of having "placeAngels" it should just be "placeUnitOne" and there should only be one con
public class AI_Controller : MonoBehaviour {
    private World_Controller worldController;

    // TODO: Instead of the below I should just have "Faction" and then set that to the godController or
    // devilController depending upon which faction the AI is playing as.
    private God_Controller godController;
    private Devil_Controller devilController;

    void Start(){
        devilController = this.gameObject.GetComponent<Devil_Controller>();
        godController = this.gameObject.GetComponent<God_Controller>();
        worldController = this.gameObject.GetComponent<World_Controller>();

        World_Controller.OnDayPassedNotifyThird += DailyActions;
    }

    public void DailyActions(){
        if (!devilController.isPlayerControlled){
            PlaceDemons();
        }
        else if (!godController.isPlayerControlled){
            // Place angels 
            PlaceAngels();
            PlaceInquisitors();
        } else {
            throw new System.Exception("The player is not controlling any faction.");
        }
    }
    
    private void PlaceAngels(){
        // Randomly place all of the angels into different countries.
        // Reset angels (Commented Out for now.).
        //for (int i = world_Controller.region_Controller.Length; i > 0; i--){
        //    world_Controller.region_Controller[(i - 1)].SetLocalAngels(0);
        //}
        //god_Controller._availableAngels = god_Controller._maxDeployableAngels;

        // Place angels in random countries.
        uint availableAngels = godController.GetAvailableAngels();

        for (uint i = availableAngels; i > 0; i--){
            int randomNumber = (int)(UnityEngine.Random.Range(0.0f, 26.0f));
            worldController.region_Controller[randomNumber].IncrementLocalAngels();
        }

        godController.SetAvailableAngels(0);
    }

    private void PlaceInquisitors(){
        uint availableInquisitors = godController.GetAvailableInquisitors();

        for (uint i = availableInquisitors; i > 0; i--) {
            int randomNumber = (int)(UnityEngine.Random.Range(0.0f, 26.0f));
            worldController.region_Controller[randomNumber].IncrementLocalInquisitors();
        }

        godController.SetAvailableInquisitors(0);
    }

    private void PlaceDemons(){
        throw new NotImplementedException();
    }

    private void PlaceBanshees(){
        throw new NotImplementedException();
    }

    
    //void AiDailyActions(){
    //// This stuff below should be on the God_Controller
    //    float totalGoodPop = 0.0f;
    //    long totalGoodDied = 0;

    //    for (int i = 0; i < 25; i++){
    //        totalGoodDied += regionNames[i].GetGoodDied();
    //        regionNames[i].ResetDeathCounterGood();
    //    }

    //    //set prayers
    //    prayers += (long)(prayerEfficency * totalGoodPop * 0.00000000005);

    //    // set souls 
    //    souls += (totalGoodDied / 100);

    //    //set dead counter to increase by the total number of deaths today
    //    goodDeathCount += totalGoodDied;
    //}
}
