using System;
using UnityEngine;

public class AI_Controller : MonoBehaviour {

    [SerializeField]
    private Clock worldController; //TODO: Change.
    [SerializeField]
    private Player_Controller playerController;
    [SerializeField]
    private Faction aiControlledFaction;

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
            int randomNumber = (int)(UnityEngine.Random.Range(0.0f, 25.0f));
            worldController.region_Controller[randomNumber].IncrementLocalGoodAgents(); ////// TODO: Use delegates for this in region controller? 
        }                                                                               ////// This can pass the AIFaction to the region controller,
                                                                                        ////// which will then increment the AIFaction.

        aiControlledFaction.AvailableAgents = 0;
    }

    private void DeploySecondaryUnits(){
        uint availableInquisitors = aiControlledFaction.AvailableSecondaryUnits;

        for (uint i = availableInquisitors; i > 0; i--) {
            int randomNumber = (int)(UnityEngine.Random.Range(0.0f, 25.0f));
            worldController.region_Controller[randomNumber].IncrementLocalGoodSecondaryUnits();////////
        }

        aiControlledFaction.AvailableSecondaryUnits = 0;
    }
}
