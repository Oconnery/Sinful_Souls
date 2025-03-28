using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.UI.Image;

// Contains the player mouse/touch input. For Keyboard Controls see Keyboard_Controls class.
public class Player_Controller : MonoBehaviour{
    
    public Hud_Controller hudController;
    public GameObject regionPanel;

    private GameObject regionHit;
    private Collider2D regionHitCollider;

    private GameObject lastBorderRef;

    public GameObject GetCountryHit(){
        return regionHit;
    }

    private void Update(){
        // touch controls
        //if (Input.touchCount > 0){ 
        //    if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) {
        //        // Fix methods so that they also accept touch.position etc.
        //        Touch touch = Input.GetTouch(0);

        //        Debug.Log("Mouse pos" + Input.mousePosition);
        //        RaycastHit2D objectHit = DrawRayCast();

        //        if (objectHit){ // TODO: Check it's a country
        //            Debug.Log("Clicked country: " + objectHit.collider.gameObject);
        //            StoreHitInfoGlobally(objectHit);
        //            SetCountryPanelUI(objectHit);
        //        }
        //    }
        //}

        // left click
        if (Input.GetMouseButtonDown(0)){
            // Check that pointer is not over a game object.
            if (!EventSystem.current.IsPointerOverGameObject()) {
                Debug.Log("Mouse pos" + Input.mousePosition);
                RaycastHit2D objectHit = DrawRayCast();

                if (objectHit){ // TODO: Check it's a region/has a region component
                    Debug.Log("Clicked country: " + objectHit.collider.gameObject);
                    StoreHitInfo(objectHit);
                    SetCountryPanelUI(objectHit);

                    if (lastBorderRef != null)
                        lastBorderRef.SetActive(false);
                    lastBorderRef = regionHit.GetComponent<Region_Controller>().borderRef;
                    lastBorderRef.SetActive(true);
                } else {
                    DeselectRegion();
                }
            }
        } else {
            if (Input.GetMouseButtonDown(1)) {
                DeselectRegion();
            }
        }
    }

    private void DeselectRegion(){
        StoreNullHitInfo();
        regionPanel.GetComponent<Region_Panel_Script>().UpdateRegionPanel(null);
        if (lastBorderRef != null)
            lastBorderRef.SetActive(false);
        lastBorderRef = null;
    }

    private RaycastHit2D DrawRayCast(){
        // Description of ray from camera through to the screen at mouse position.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // Cast ray. Set hit2D.
        RaycastHit2D raycastHit2D = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
        return raycastHit2D;
    }

    private void StoreHitInfo(RaycastHit2D raycastHit2D){
        // Store collider as class wide object.
        regionHitCollider = raycastHit2D.collider;
        // Store country hit as class wide object.
        regionHit = raycastHit2D.collider.gameObject;
    }

    private void StoreNullHitInfo(){
        // Store collider as class wide object.
        regionHitCollider = null;
        // Store country hit as class wide object.
        regionHit = null;
    }

    private void SetCountryPanelUI(RaycastHit2D raycastHit2D){
        // Change the information displayed on the panel
        Region_Panel_Script regionPanelScript = regionPanel.GetComponent<Region_Panel_Script>();
        regionPanelScript.UpdateRegionPanel(regionHit);
    }

    public Vector3 GetRegionRandomLocale() {
        return GetRegionRandomLocale(regionHitCollider);
    }

    // TODO: Should not be on the player controller.
    public Vector3 GetRegionRandomLocale(Collider2D collider){
        Vector3 randomPosition;
        Vector3 colliderPos = collider.transform.position;
        do {
            randomPosition = new Vector3(
                    UnityEngine.Random.Range(collider.bounds.min.x, collider.bounds.max.x),
                    UnityEngine.Random.Range(collider.bounds.min.y, collider.bounds.max.y),
                    colliderPos.z);
        } while (!regionHitCollider.OverlapPoint(randomPosition));

        randomPosition.z -= 1;
        return randomPosition;
    }
}
