using UnityEngine;
using UnityEngine.UI;

public class Hud_Controller : MonoBehaviour{
    public GameObject statsPanel;

    public GameObject researchTree;
    public GameObject researchTreeBackground;

    private Region_Panel_Script currentRegionOpen;
    public GameObject regionPanel;

    public GameObject eventsPanel;

    public Button playBtn;
    public Button pauseBtn;
    public Button fastForwardBtn;

    public Button researchTreeBtn;

    public World_Controller worldController;

    public void SetCurrentRegionOpen(Region_Panel_Script region){
        currentRegionOpen = region;
    }

    public void OpenStatsScreen(){
        statsPanel.SetActive(true);
        worldController.Pause();
    }

    public void CloseStatsScreen(){
        statsPanel.SetActive(false);
        worldController.Unpause();
    }

    public void SetCountryPanelActive(){
        regionPanel.SetActive(true);
    }

    public void SetCountryPanelInactive(){
        regionPanel.SetActive(false);
    }

    public void OpenResearchTree(){
        worldController.Pause();
        researchTreeBackground.SetActive(true);
        researchTree.SetActive(true);
        DisableClockHUDButtons();
    }

    public void CloseResearchTree(){
        researchTreeBackground.SetActive(false);
        researchTree.SetActive(false);
        worldController.Unpause();
        EnableClockHUDButtons();
    }

    public void SetEventsPanelActive(){
        worldController.Pause();
        eventsPanel.SetActive(true);
        DisableClockHUDButtons();
    }

    public void SetEventsPanelInactive(){
        eventsPanel.SetActive(false);
        EnableClockHUDButtons();
    }

    private void EnableClockHUDButtons(){
        playBtn.enabled = true;
        pauseBtn.enabled = true;
        fastForwardBtn.enabled = true;
    }

    private void DisableClockHUDButtons(){
        playBtn.enabled = false;
        pauseBtn.enabled = false;
        fastForwardBtn.enabled = false;
    }
}
