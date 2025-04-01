using System;
using UnityEngine;
using UnityEngine.UI;

public class Hud_Controller : MonoBehaviour{
    // TODO: [SeralizeField] private instead of public

    [SerializeField] private GameObject statsPanel;

    [SerializeField] private GameObject researchTree;
    [SerializeField] private GameObject researchTreeBackground;

    [SerializeField] private GameObject eventsPanel;

    [SerializeField] private GameObject regionUI;

    [SerializeField] private Button playBtn;
    [SerializeField] private Button pauseBtn;
    [SerializeField] private Button fastForwardBtn;

    [SerializeField] private Button researchTreeBtn;

    [SerializeField] private Text primaryResourceText;
    [SerializeField] private Text secondaryResourceText;
    [SerializeField] private Text secondaryResourceEfficencyText;
    [SerializeField] private Text currentDateText;
    [SerializeField] private Text hellCountText;
    [SerializeField] private Text heavenCountText;

    [SerializeField] private Text baseUnitCountText;
    [SerializeField] private Text specialUnitCountText;

    [SerializeField] private Devil_Controller devilController;
    [SerializeField] private God_Controller godController;

    [SerializeField] private Player_Controller playerController;

    private void Start() {
        Clock.OnDayPassedNotifyHUD += SetHUD; 
        SetupHudText();
    }

    private void SetHUD() {
        SetDateText();
        SetHellCountText();
        SetHeavenCountText();
        SetPrimaryResourceText();
        SetSecondaryResourceText();
        SetSecondaryResourceEfficencyText();
    }

    private void SetupHudText() {
        primaryResourceText.text = "0";
        secondaryResourceText.text = "Sins: 0 Million";
        secondaryResourceEfficencyText.text = "100%";
        //newsFlashText.text = "NOTHING HAPPENED TODAY!";
        SetDateText();
        SetBaseUnitCountText();
        SetSecondaryUnitCountText();
    }

    private void SetDateText() {
        ushort day = Clock.Day;
        ushort month = Clock.Month;

        if (day < 10) {
            // If single digits it should be prefixed with 0
            currentDateText.text = "0" + day + "/";
        } else {
            currentDateText.text = day + "/";
        }

        if (month < 10) {
            // If single digits it should be prefixed with 0
            currentDateText.text += "0" + month + "/";
        } else {
            currentDateText.text += month + "/";
        }
        currentDateText.text += Clock.Year;
    }

    private void SetHellCountText() {
        hellCountText.text = ($"Hell: {devilController.RealmDeathCount}");
    }

    private void SetHeavenCountText() {
        heavenCountText.text = ($"Heaven: {godController.RealmDeathCount}");
    }
    public void SetPrimaryResourceText() {
        //Need to round like the sins
        if (playerController.PlayingAsDevil()) {
            primaryResourceText.text = Math.Floor(devilController.PrimaryResource).ToString();
        } else if (playerController.PlayingAsGod()) {
            throw new NotImplementedException("God faction not implemented."); // TODO
        } else { throw new System.Exception("The player isn't controlling the god or devil class."); }
    }
    private void SetSecondaryResourceText() {
        if (playerController.PlayingAsDevil()) {
            secondaryResourceText.text = "Sins: " + Math.Floor(devilController.SecondaryResource) + " Million";
        } else if (playerController.PlayingAsGod()) {
            throw new NotImplementedException("God faction not implemented."); // TODO
        } else { throw new System.Exception("The player isn't controlling the god or devil class."); }
    }

    private void SetSecondaryResourceEfficencyText() {
        if (playerController.PlayingAsDevil()) {
            secondaryResourceEfficencyText.text = (devilController.SecondaryResourceGenerationEfficency).ToString("F0");
        } else if (playerController.PlayingAsGod()) {
            throw new NotImplementedException("God faction not implemented."); // TODO
        }
    }

    public void SetBaseUnitCountText() {
        Faction faction = playerController.playerControlledFaction;
        baseUnitCountText.text = ($"{devilController.AvailableAgents} / {devilController.AgentPool}");
    }

    public void SetSecondaryUnitCountText() {
        Faction faction = playerController.playerControlledFaction;
        specialUnitCountText.text = ($"{devilController.AvailableSecondaryUnits} / {devilController.SecondaryUnitPool}");
    }

    public void OpenStatsScreen(){
        statsPanel.SetActive(true);
        Clock.Pause();
    }

    public void CloseStatsScreen(){
        statsPanel.SetActive(false);
        Clock.Unpause();
    }

    public void OpenResearchTree(){
        Clock.Pause();
        researchTreeBackground.SetActive(true);
        researchTree.SetActive(true);
        DisableClockHUDButtons();
        CloseRegionUI();
    }

    private void CloseRegionUI() {
        regionUI.SetActive(false);
    }

    public void CloseResearchTree(){
        researchTreeBackground.SetActive(false);
        researchTree.SetActive(false);
        EnableClockHUDButtons();
        EnableRegionUI();
        Clock.Unpause();
    }

    private void EnableRegionUI(){
        regionUI.SetActive(true);
    }

    public void SetEventsPanelActive(){
        Clock.Pause();
        eventsPanel.SetActive(true);
        DisableClockHUDButtons();
    }

    public void SetEventsPanelInactive(){
        eventsPanel.SetActive(false);
        EnableClockHUDButtons();
        Clock.Unpause();
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
