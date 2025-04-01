using System;
using UnityEngine;

public class AI_Controller : MonoBehaviour {

    [SerializeField] private Region_Master_Controller regionMasterController; 
    [SerializeField] private Player_Controller playerController;
    [SerializeField] private Faction aiControlledFaction;

    void Start(){
        Clock.OnDayPassedNotifyAI += DailyActions;

        if (aiControlledFaction == playerController.playerControlledFaction){
            throw new NotImplementedException("Player and AI are playing as the same faction. Not currently allowed.");
        }
    }

    public void DailyActions(){
        DeployAgents();
        DeploySecondaryUnits();
    }
    
    private void DeployAgents(){
        uint availableAgents = aiControlledFaction.AvailableAgents;

        for (uint i = availableAgents; i > 0; i--){
            regionMasterController.GetRandomRegionController().IncrementLocalGoodAgents(); 
        }                                                                               

        aiControlledFaction.AvailableAgents = 0;
    }

    private void DeploySecondaryUnits(){
        uint availableInquisitors = aiControlledFaction.AvailableSecondaryUnits;

        for (uint i = availableInquisitors; i > 0; i--) {
            regionMasterController.GetRandomRegionController().IncrementLocalGoodSecondaryUnits();
        }

        aiControlledFaction.AvailableSecondaryUnits = 0;
    }
}
