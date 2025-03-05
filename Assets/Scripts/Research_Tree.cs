using UnityEngine;

public class Research_Tree : MonoBehaviour{

    public World_Controller worldController;
     
    void OnEnable(){
        // pause the game
        Debug.Log("Research tree enabled");

        worldController.Pause();
    }

    void OnDisable(){
        // pause the game
        Debug.Log("Research tree disabled");

        worldController.UnpauseResetSpeed();
    }
}
