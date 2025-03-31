using UnityEngine;
using UnityEngine.UI;

public class Keyboard_Controls : MonoBehaviour{
    public Button playBtn;
    public Button pauseBtn;

    void Update(){
        if (Input.GetKeyDown(KeyCode.Space)){
            SpaceBarPressed();
        }   
    }

    private void SpaceBarPressed(){
        if (Clock.GetIsTimePaused()){
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
