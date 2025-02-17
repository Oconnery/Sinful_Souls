using UnityEngine;
using UnityEngine.EventSystems;

public class PauseButton : MonoBehaviour, IPointerClickHandler{

    public World_Controller worldController;

    public void OnPointerClick(PointerEventData eventData){
        worldController.Pause();
    }
}
