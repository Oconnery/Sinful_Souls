using System.Collections.Generic;
using UnityEngine;

public class Unit_UI : MonoBehaviour{
    // At the moment the player can only see their own units, but in the future it's possible enemy units could be displayed on the screen. So I should remove "player" link
    private Dictionary<string, GameObject> playerAgentsByRegion = new Dictionary<string, GameObject>(); // Might be better to make a dictionary of <regionName, ListOfUnitGOsIntheRegion> instead of having <regionName+unitNum, one GO in region>
    private Dictionary<string, GameObject> playerSecondaryUnitsByRegion = new Dictionary<string, GameObject>();

    [SerializeField] private GameObject evilAgentDotPrefab;
    [SerializeField] private GameObject evilSecondaryUnitPrefab;
    [SerializeField] private GameObject goodAgentDotPrefab;
    [SerializeField] private GameObject goodSecondaryUnitPrefab;

    public void AddAgent(Faction faction, string currentRegionName, string localFactionAgents, Vector3 location) {
        if (faction is Devil_Controller){
            AddEvilAgent(currentRegionName, localFactionAgents, location);
        } else if (faction is God_Controller){
            AddGoodAgent(currentRegionName, localFactionAgents, location);
        }
    }

    private void AddEvilAgent(string currentRegionName, string localEvilAgents, Vector3 location) {
        GameObject unit = Instantiate(evilAgentDotPrefab, location, Quaternion.identity, transform);
        // TODO: Make the first 3 parts a map of the possible regions - create at game start and then get by "region" key after and add "localEvilAgents" onto the end? // This way, the game start will take time to load but game will be more performant.
        unit.name = $"evilAgent_{currentRegionName}{localEvilAgents}";
        playerAgentsByRegion.Add($"{currentRegionName}{localEvilAgents}", unit);
    }

    private void AddGoodAgent(string currentRegionName, string localGoodAgents, Vector3 location) {
        GameObject unit = Instantiate(goodAgentDotPrefab, location, Quaternion.identity, transform);
        unit.name = $"goodAgent_{currentRegionName}{localGoodAgents}";
        playerAgentsByRegion.Add($"{currentRegionName}{localGoodAgents}", unit);
    }

    public void RemoveAllAgents(Faction faction, string currentRegionName, ushort localFactionAgents) {
        if (faction is Devil_Controller) {
            RemoveAllEvilAgents(currentRegionName, localFactionAgents);
        } else if (faction is God_Controller) {
            RemoveAllGoodAgents(currentRegionName, localFactionAgents);
        }
    }

    public void RemoveAllEvilAgents(string currentRegionName, ushort localEvilAgents) {
        for (ushort i = 1; i <= localEvilAgents; i++) {
            RemoveEvilAgent(currentRegionName, i);
        }
    }

    public void RemoveAllGoodAgents(string currentRegionName, ushort localGoodAgents) {
        for (ushort i = 1; i <= localGoodAgents; i++) {
            RemoveGoodAgent(currentRegionName, i);
        }
    }

    public void RemoveAgent(Faction faction, string currentRegionName, ushort localEvilAgents) {
        if (faction is Devil_Controller) {
            RemoveEvilAgent(currentRegionName, localEvilAgents);
        } else if (faction is God_Controller) {
            RemoveGoodAgent(currentRegionName, localEvilAgents);
        }
    }

    private void RemoveEvilAgent(string currentRegionName, ushort localEvilAgent){
        string evilAgentGOName = $"{currentRegionName}{localEvilAgent}";
        GameObject evilAgentPrefabInstance = playerAgentsByRegion[evilAgentGOName]; // TODO: ISSUE: ICELAND 4 WAS NOT PRESENT IN THE DICTIONARY. 
        playerAgentsByRegion.Remove(evilAgentGOName);
        Destroy(evilAgentPrefabInstance);
    }

    private void RemoveGoodAgent(string currentRegionName, ushort localGoodAgent) {
        string goodAgentGOName = $"{currentRegionName}{localGoodAgent}";
        GameObject goodAgentPrefabInstance = playerAgentsByRegion[goodAgentGOName];
        playerAgentsByRegion.Remove(goodAgentGOName);
        Destroy(goodAgentPrefabInstance);
    }

    public void AddSecondaryUnit(Faction faction, string currentRegionName, ushort localFactionSecondaryUnits, Vector3 location){
        if (faction is Devil_Controller) {
            AddEvilSecondaryUnit(currentRegionName, localFactionSecondaryUnits, location);
        } else if (faction is God_Controller) {
            AddGoodSecondaryUnit(currentRegionName, localFactionSecondaryUnits, location);
        }
    }

    private void AddEvilSecondaryUnit(string currentRegionName, ushort localEvilSecondaryUnits, Vector3 location) {
        GameObject evilSecondaryUnit = Instantiate(evilSecondaryUnitPrefab, location, Quaternion.identity, transform);
        evilSecondaryUnit.name = $"evilSecondaryUnit_{currentRegionName}{localEvilSecondaryUnits}";
        playerSecondaryUnitsByRegion.Add($"{currentRegionName}{localEvilSecondaryUnits}", evilSecondaryUnit);
    }

    private void AddGoodSecondaryUnit(string currentRegionName, ushort localGoodSecondaryUnits, Vector3 location) {
        GameObject goodSecondaryUnit = Instantiate(goodSecondaryUnitPrefab, location, Quaternion.identity, transform);
        goodSecondaryUnit.name = $"goodSecondaryUnit_{currentRegionName}{localGoodSecondaryUnits}";
        playerSecondaryUnitsByRegion.Add($"{currentRegionName}{localGoodSecondaryUnits}", goodSecondaryUnit);
    }

    public void RemoveSecondaryUnit(Faction faction, string currentRegionName, ushort localFactionSecondaryUnits) {
        if (faction is Devil_Controller) {
            RemoveEvilSecondaryUnit(currentRegionName, localFactionSecondaryUnits);
        } else if (faction is God_Controller) {
            RemoveGoodSecondaryUnit(currentRegionName, localFactionSecondaryUnits);
        }
    }

    private void RemoveEvilSecondaryUnit(string currentRegionName, ushort localEvilSecondaryUnits) {
        string evilSecondaryUnitGOName = $"{currentRegionName}{localEvilSecondaryUnits}";
        GameObject evilAgentPrefabInstance = playerAgentsByRegion[evilSecondaryUnitGOName];
        playerSecondaryUnitsByRegion.Remove(evilSecondaryUnitGOName);
        Destroy(evilAgentPrefabInstance);
    }

    private void RemoveGoodSecondaryUnit(string currentRegionName, ushort localGoodSecondaryUnits) {
        string goodSecondaryUnitGOName = $"{currentRegionName}{localGoodSecondaryUnits}";
        GameObject evilAgentPrefabInstance = playerAgentsByRegion[goodSecondaryUnitGOName];
        playerSecondaryUnitsByRegion.Remove(goodSecondaryUnitGOName);
        Destroy(evilAgentPrefabInstance);
    }
}
