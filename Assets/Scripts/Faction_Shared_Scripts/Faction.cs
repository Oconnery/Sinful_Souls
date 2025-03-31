using System;
using UnityEngine;

// TODO: Could create a separate class "army" and create a reference here. it would contain all the units.
public class Faction : MonoBehaviour { // TODO: Should use singleton pattern too? Then I can reference isPlayedControlled easily? Does singleton work when you are using inheritance?
  
    public double PrimaryResource { get; private set; }// (Skulls and Souls)
    public double SecondaryResource { get; private set; }// (Sins and Prayers)

    private const double PRIMARY_RESOURCE_MULTIPLIER = 0.1948;
    public double SecondaryResourceGenerationEfficency { get; set; }
    private const double SECONDARY_RESOURCE_MULTIPLIER = 0.00000000005;
   
    public uint startingAgents;
    public uint startingSecondaryUnits;

    public uint AvailableAgents { get; set; } // Agents convert people
    public uint AvailableSecondaryUnits { get; set; } // Secondary units reduce effectiveness of enemy agents. It would be cooler if these secondary units were demons/angels, and if they slayed agents in the local area, or slayed people.
    public uint AgentPool { get; private set; }
    public uint SecondaryUnitPool { get; private set; }

    protected uint agentCost;

    public AudioSource buyAgentAudio;

    public event Func<ulong> OnDailyShoutReturnPop;
    public event Func<ulong> OnDailyShoutReturnDiedToday;
    public event Action OnDailyShoutResetDiedToday; 

    public ulong RealmDeathCount { get; protected set; }

    protected ulong factionPopToday;
    protected ulong factionDiedToday;

    void Start() {
        factionDiedToday = 0;
        factionPopToday = 0;
        RealmDeathCount = 0;

        PrimaryResource = 0;
        SecondaryResource = 0.0;
        agentCost = 100;
        SecondaryResourceGenerationEfficency = 1.0f;

        AvailableAgents = startingAgents;
        AvailableSecondaryUnits = startingSecondaryUnits;
        AgentPool = startingAgents;
        SecondaryUnitPool = startingSecondaryUnits;

        Clock.OnDayPassedNotifyFactions += DailyShout;
    }

    public void DailyShout() {
        // Update the daily statistics for today.
        UpdateDailyStatistics();
        
        // Update the resources based on the new statistics.
        UpdateSecondaryResource();
        UpdatePrimaryResource();

        // Reset the daily statistics to 0.
        OnDailyShoutResetDiedToday?.Invoke();
        ResetLocalPopulationStatistics();
    }

    private void UpdateDailyStatistics() {
        Debug.Log($"Faction {this.name} - Before Update: factionPopToday = {factionPopToday}");
        // Update the daily statistics for today.
        Debug.Log($"OnDailyShoutReturnPop has {OnDailyShoutReturnPop?.GetInvocationList().Length ?? 0} subscribers.");
        Debug.Log($"OnDailyShoutReturnDiedToday has {OnDailyShoutReturnDiedToday?.GetInvocationList().Length ?? 0} subscribers.");

        if (OnDailyShoutReturnPop != null) {
            foreach (var del in OnDailyShoutReturnPop.GetInvocationList()){
                factionPopToday += ((Func<ulong>)del).Invoke();
            }
        }
        if (OnDailyShoutReturnDiedToday != null) {
            foreach (var del in OnDailyShoutReturnDiedToday.GetInvocationList()) {
                factionDiedToday += ((Func<ulong>)del).Invoke();
            }
        }
        RealmDeathCount += factionDiedToday;
    }
    
    public void ResetLocalPopulationStatistics() {
        factionPopToday = 0;
        factionDiedToday = 0;
    }

    public void BuyAgent() {
        if (PrimaryResource >= agentCost) {
            PrimaryResource -= agentCost;
            AgentPool++;
            AvailableAgents++;
            buyAgentAudio.Play();
        }
    }

    public void SpendSecondaryResource(double resourceToSpend) {
        if (resourceToSpend > SecondaryResource) {
            throw new System.ArgumentException($"The number of sins ({SecondaryResource}) is less than the amount passed from the Devil_Research_Information class object ({resourceToSpend}).");
        } else if (resourceToSpend < 0) {
            throw new System.ArgumentException($"The parameter passed to SpendSins ({resourceToSpend}) was below zero.");
        } else {
            SecondaryResource -= resourceToSpend;
        }
    }

    public void UpdatePrimaryResource() {
        PrimaryResource += (factionDiedToday / 100.0 * PRIMARY_RESOURCE_MULTIPLIER);
    }

    public void UpdateSecondaryResource() {
        SecondaryResource += factionPopToday * SecondaryResourceGenerationEfficency * SECONDARY_RESOURCE_MULTIPLIER;
    }
}
