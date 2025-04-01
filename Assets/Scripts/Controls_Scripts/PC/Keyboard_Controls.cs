using UnityEngine;
using UnityEngine.UI;

public class Keyboard_Controls : MonoBehaviour{
    [SerializeField] private Button playBtn;
    [SerializeField] private Button pauseBtn;

    void Update(){
        if (Input.GetKeyDown(KeyCode.Space)){
            SpaceBarPressed();
        }   
    }

    private void SpaceBarPressed(){
        if (Clock.IsTimePaused){
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
