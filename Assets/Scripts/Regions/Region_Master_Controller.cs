using JetBrains.Annotations;
using UnityEngine;

public class Region_Master_Controller : MonoBehaviour{
    // TODO: shouldn't need this hard coded value. Use a list instead?
    private const int NUMBER_OF_REGIONS = 26;
    [SerializeField] private Region_Controller[] regionController = new Region_Controller[NUMBER_OF_REGIONS];

    public Region_Controller GetRandomRegionController(){
        int randomCountryInt = (int)(Random.Range(0.0f, 25.0f));
        return GetRegionController(randomCountryInt);
    }

    public Region_Controller GetRegionController(int i){
        return regionController[i];
    }
}
