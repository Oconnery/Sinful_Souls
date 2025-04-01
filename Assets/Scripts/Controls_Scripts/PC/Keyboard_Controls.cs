using UnityEngine;
using UnityEngine.UI;

public class Keyboard_Controls : MonoBehaviour{
    //[SerializeField] private Button playBtn;
    //[SerializeField] private Button pauseBtn;
    [SerializeField] private AudioSource playAudio;
    [SerializeField] private AudioSource pauseAudio;

    void Update(){
        if (Input.GetKeyDown(KeyCode.Space)){
            SpaceBarPressed();
        }   
    }

    private void SpaceBarPressed(){
        if (Clock.IsTimePaused) {
            //InvokeClickIfActive(playBtn); This is buggy. Issues with invoking click. Doesn't work as expected.
            Clock.UnpauseResetSpeed();
            playAudio.Play();
        } else {
            //InvokeClickIfActive(pauseBtn);
            Clock.Pause();
            pauseAudio.Play();
        }
    }

    private void InvokeClickIfActive(Button button) {
        if (button.IsActive()) { 
            button.onClick.Invoke();
        }
    }
}
