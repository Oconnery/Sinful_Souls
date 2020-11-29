using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Faction_Selector_Scene_Controller : MonoBehaviour {


    public void LoadMainMenuScene(){
        SceneManager.LoadScene("Main_Menu");
    }

    public void LoadDevilEarthScene(){
        SceneManager.LoadScene("Devil_Earth");
    }

    public void LoadGodEarthScene(){
        SceneManager.LoadScene("God_Earth");
    }
}
