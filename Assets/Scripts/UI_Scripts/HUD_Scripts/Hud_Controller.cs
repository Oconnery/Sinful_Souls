using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hud_Controller : MonoBehaviour {

    public GameObject statsPanel;
    public GameObject researchTree;
    public GameObject countryPanel;
    public GameObject eventsPanel;

    public void openStatsScreen(){
        statsPanel.SetActive(true);
    }

    public void closeStatsScreen(){
        statsPanel.SetActive(false);
    }

    public void openResearchTree() {
        researchTree.SetActive(true);
    }

    public void SetCountryPanelInactive(){
        countryPanel.SetActive(false);
    }

    public void SetResearchTreePanelInactive(){
        researchTree.SetActive(false);
    }

    public void SetEventsPanelInactive(){
        eventsPanel.SetActive(false);
    }
}
