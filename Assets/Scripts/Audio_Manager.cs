using UnityEngine;

// TODO: Should contain references to a bunch of audios and other scripts should be able to play them. 
// SINGLETON //
public class Audio_Manager : MonoBehaviour {
    public static Audio_Manager instance { get; private set; }

    private void Awake() {
        // If there is an instance, and it's not me, delete myself.
        if (instance != null && instance != this) {
            Destroy(this);
        } else {
            instance = this;
        }
    }
}
