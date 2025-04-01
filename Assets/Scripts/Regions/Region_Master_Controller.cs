using JetBrains.Annotations;
using UnityEngine;

public class Region_Master_Controller : MonoBehaviour{
    private const int NUMBER_OF_COUNTRIES = 26;
    [SerializeField] private Region_Controller[] regionController = new Region_Controller[NUMBER_OF_COUNTRIES];

    public Region_Controller GetRandomRegionController(){
        int randomCountryInt = (int)(Random.Range(0.0f, 25.0f));
        return GetRegionController(randomCountryInt);
    }

    public Region_Controller GetRegionController(int i){
        return regionController[i];
    }
}
