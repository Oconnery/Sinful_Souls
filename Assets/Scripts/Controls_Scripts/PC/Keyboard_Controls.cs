using UnityEngine;
using UnityEngine.UI;

public class Keyboard_Controls : MonoBehaviour{
    private World_Controller worldController;
    public Button playBtn;
    public Button pauseBtn;

    private void Start(){
        worldController = GetComponent<World_Controller>();
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.Space)){
            SpaceBarPressed();
        }   
    }

    private void SpaceBarPressed(){
        if (worldController.GetIsTimePaused()){
            InvokeClickIfActive(playBtn);
        } else {
            InvokeClickIfActive(pauseBtn);
        }
    }

    private void InvokeClickIfActive(Button button) {
        if (button.IsActive()) { 
            button.onClick.Invoke();
        }
    }
}
