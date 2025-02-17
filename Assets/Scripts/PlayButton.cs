using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayButton : MonoBehaviour, IPointerClickHandler{

    public World_Controller worldController;

    public void OnPointerClick(PointerEventData eventData){
        worldController.UnPause();
    }
}
