using UnityEngine;

public class Research_Tree : MonoBehaviour{
     
    void OnEnable(){
        // pause the game
        Debug.Log("Research tree enabled");

        Clock.Pause();
    }

    void OnDisable(){
        // pause the game
        Debug.Log("Research tree disabled");

        Clock.UnpauseResetSpeed();
    }
}
