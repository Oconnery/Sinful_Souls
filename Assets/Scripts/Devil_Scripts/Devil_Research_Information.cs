using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Devil_Research_Information : MonoBehaviour {
    // This script should be attatched to a game object in the world regardless of scene

    // references should really be at the top .. folowed by private internal stuff
    private Devil_Controller devil_Controller;
    
    // 2D array
    private bool[,] hasResearchedTreePositions;

    private static int MAX_TIERS = 2;
    private static int MAX_RANKS = 3;

    void Start(){
        hasResearchedTreePositions = new bool[MAX_TIERS, MAX_RANKS];
    }

    // Is the given rank researched.
    private bool IsRankResearched(int tier, int rank){
        if (hasResearchedTreePositions[tier, rank]){
            return true;
        }
        return false;
    }
 
    #region Research Tiers
    
    #endregion
}
