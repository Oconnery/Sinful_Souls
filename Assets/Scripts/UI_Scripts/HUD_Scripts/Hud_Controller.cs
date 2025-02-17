using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hud_Controller : MonoBehaviour {

    public GameObject statsPanel;
    public GameObject researchTree;
    public GameObject researchTreeBackground;
    public GameObject countryPanel;
    public GameObject eventsPanel;

    public void openStatsScreen(){
        statsPanel.SetActive(true);
    }

    public void closeStatsScreen(){
        statsPanel.SetActive(false);
    }


    public void SetCountryPanelInactive(){
        countryPanel.SetActive(false);
    }

    public void openResearchTree(){
        researchTreeBackground.SetActive(true);
        researchTree.SetActive(true);
    }

    public void closeResearchTree(){
        researchTreeBackground.SetActive(false);
        researchTree.SetActive(false);
    }

    public void SetEventsPanelInactive(){
        eventsPanel.SetActive(false);
    }
}
