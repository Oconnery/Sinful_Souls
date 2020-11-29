using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu_Scene_Controller : MonoBehaviour {
    // Loads different scenes from the main menu scene.
	
    public void LoadFactionSelectScene(){
        SceneManager.LoadScene("Choose_Faction");
    }

    public void LoadContinueGameScene(){
        
    }

    public void ExitGame(){
        Application.Quit();
    }
}
