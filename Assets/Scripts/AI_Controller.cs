using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Controller : MonoBehaviour {

    private World_Controller world_Controller;
    private God_Controller god_Controller;
    private Devil_Controller devil_Controller;

    void Start(){
        devil_Controller = this.gameObject.GetComponent<Devil_Controller>();
        god_Controller = this.gameObject.GetComponent<God_Controller>();
        world_Controller = this.gameObject.GetComponent<World_Controller>();
    }

    public void AIDailyActions(){
        if (!devil_Controller.isPlayerControlled){
            PlaceDemons();
        }
        else if (!god_Controller.isPlayerControlled){
            // Place angels 
            PlaceAngels();
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
        for (int i = god_Controller._availableAngels; i > 0; i--){
            int randomNumber = (int)(Random.Range(0.0f, 26.0f));
            world_Controller.region_Controller[randomNumber].IncrementLocalAngels();
        }

        god_Controller._availableAngels = 0;
    }

    private void PlaceInquisitors(){

    }

    private void PlaceDemons(){

    }

    private void PlaceBanshees(){

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
