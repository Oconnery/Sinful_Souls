using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class God_Research_Tree_Information : MonoBehaviour {
    // 2D array
    private bool[,] hasResearchedTreePositions;

    private static int MAX_TIERS = 2;
    private static int MAX_RANKS = 3;

    // Use this for initialization
    void Start(){
        //two tiers, each with
        hasResearchedTreePositions = new bool[MAX_TIERS, MAX_RANKS];
    }
	
    // Methods to 
}
