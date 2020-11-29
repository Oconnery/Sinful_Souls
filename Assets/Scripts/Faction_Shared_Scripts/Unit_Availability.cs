using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit_Availability : MonoBehaviour {

    // The starting available units allows the game designer to change gameplay in the inspector without changing code, 
    // whilst keeping the avaialable units varaibles private. 
    public int _startingAvailableMainUnits;
    public int _startingAvailableSpecialUnits;

    private int _availableMainUnits;
    private int _availableSpecialUnits;

    public int _maxDeployableMainUnits;
    public int _maxDeployableSpecialUnits;

    void Start () {
        // Units available to the devil at game start.
        _availableMainUnits = _startingAvailableMainUnits;
        _availableSpecialUnits = _startingAvailableSpecialUnits;

        // The current maximum deployable units.
        _maxDeployableMainUnits = _startingAvailableMainUnits;
        _maxDeployableSpecialUnits = _startingAvailableSpecialUnits;
    }
}
