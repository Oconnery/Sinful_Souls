using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PC_Zoom : MonoBehaviour {
    private float minimumZoom;
    private float maximumZoom;

    // The distance moved for each of the smallest movement of the mouse wheel.
    private float zoomDistancePerScroll;
    // The speed at which zooming occurs.
    private float zoomSpeedMultiplier;

    private float targetCameraSize;

    private Camera mainCamera;

    void Start() {
        mainCamera = GetComponent<Camera>();
        minimumZoom = 2.0f;
        maximumZoom = mainCamera.orthographicSize;
        zoomDistancePerScroll = 2.5f;
        zoomSpeedMultiplier = 1.0f;
        targetCameraSize = mainCamera.orthographicSize;

        // Zoom from minimum to maximum on startup.
        mainCamera.orthographicSize = minimumZoom;
    }

    void Update() {
        if (Input.mouseScrollDelta.y != 0.0f) {
         // Change target size 
            targetCameraSize -= Input.mouseScrollDelta.y * zoomDistancePerScroll;
        }
        
        ChangeCameraSize(mainCamera, ref targetCameraSize, zoomSpeedMultiplier);
    }

    /// <summary>
    /// Changes the camera size.
    /// </summary>
    /// <param name="camera"> The camera to change.</param>
    /// <param name="targetCameraSize"> The target camera size.</param>
    /// <param name="zoomSpeedMultiplier"> The rate at which zooming should happen.</param>
    private void ChangeCameraSize(Camera camera, ref float targetCameraSize, float zoomSpeedMultiplier) {
        if (targetCameraSize > maximumZoom) {
            targetCameraSize = maximumZoom;
        }
        else if (targetCameraSize < minimumZoom){
            targetCameraSize = minimumZoom;
        }

        // Change size of camera.
        if (targetCameraSize != camera.orthographicSize) {
            camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, targetCameraSize, Time.deltaTime * zoomSpeedMultiplier);
        }
    }

}
    

