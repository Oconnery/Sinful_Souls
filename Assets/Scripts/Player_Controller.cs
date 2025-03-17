using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Contains the player mouse/touch input. For Keyboard Controls see Keyboard_Controls class.
public class Player_Controller : MonoBehaviour {
    
    public Hud_Controller hudController;
    public GameObject regionPanel;

    private GameObject regionHit;
    private Collider2D regionHitCollider;

    public GameObject GetCountryHit(){
        return regionHit;
    }

    private void Update() {
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
        if (Input.GetMouseButtonDown(0)) {
            // Check that pointer is not over a game object.
            if (!EventSystem.current.IsPointerOverGameObject()) {
                Debug.Log("Mouse pos" + Input.mousePosition);
                RaycastHit2D objectHit = DrawRayCast();

                if (objectHit) { // TODO: Check it's a country
                    Debug.Log("Clicked country: " + objectHit.collider.gameObject);
                    StoreHitInfo(objectHit);
                    SetCountryPanelUI(objectHit);
                    // TODO: Enable border in the region.
                    regionHit.GetComponent<Region_Controller>().borderRef.SetActive(true);
                }
            }
        }
    }

    private RaycastHit2D DrawRayCast(){
        // Description of ray from camera through to the screen at mouse position.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // Cast ray. Set hit2D.
        RaycastHit2D raycastHit2D = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
        return raycastHit2D;
    }

    /// <summary>
    /// Stores a raycast hit object gloablly, including it's associated collider.
    /// </summary>
    /// <param name="raycastHit2D"> The information of an object that was hit by a raycast.</param>
    private void StoreHitInfo(RaycastHit2D raycastHit2D){
        // Store collider as class wide object.
        regionHitCollider = raycastHit2D.collider;
        // Store country hit as class wide object.
        regionHit = raycastHit2D.collider.gameObject;
    }

    private void SetCountryPanelUI(RaycastHit2D raycastHit2D){
        // Change the information displayed on the panel
        Region_Panel_Script regionPanelScript = regionPanel.GetComponent<Region_Panel_Script>();
        regionPanelScript.SetCurrentCountry(regionHit.GetComponent<Region_Controller>());
        regionPanelScript.ChangeCountryPanelText(raycastHit2D.collider.gameObject);
        hudController.SetCountryPanelActive();
        ClipCountryPanel();
    }

    /// <summary>
    /// Changes the position of the country panel to the position of the mouse.
    /// </summary>
    private void ClipCountryPanel(){
        // Set position of X and Y.
        float positionX = CalculateCountryPanelPosition(Input.mousePosition.x, Screen.width, 300);
        float positionY = CalculateCountryPanelPosition(Input.mousePosition.y, Screen.height, 215);

        // Apply position to country panel.
        regionPanel.transform.position = new Vector3(positionX, positionY, 0.0f);
    }

    /// <summary>
    /// Calculates the x or y position of the mouse, minus the margin.
    /// </summary>
    /// <param name="mousePositionOneD"> The 1 dimensional position of the mouse pointer.</param>
    /// <param name="screenSideSize"> The size of one of the sides of the sceen (horizontal or vertical).</param>
    /// <param name="sideMargin"> The minimum amount that the panel has to be from the left and right or top and bottom sides of the screen. </param>
    /// <returns></returns>
    private float CalculateCountryPanelPosition(float mousePositionOneD, int screenSideSize, int sideMargin){
        float amountToShiftPanel = mousePositionOneD;

        // Check if mouse is past the margin at the far/top portion of the screen.
        if (mousePositionOneD > (screenSideSize - sideMargin)){
            amountToShiftPanel = (screenSideSize - sideMargin);
        }
        // Check if mouse is before the margin at the near/bottom portion of the screen
        else if (mousePositionOneD < sideMargin){
            amountToShiftPanel = sideMargin;
        }
        return amountToShiftPanel;
    }

    public Vector3 GetCountryRandomLocale(){
        Vector3 countryLocale = GetCountryHit().transform.position;
        countryLocale.z -= 1; //put it ahead of other game objs

        //random location on game object  --- or even better would be the collider (size of actuat country)
        Vector2 randMax = regionHitCollider.bounds.size / 4; // /2 would work if there was no empty space. /4 randomly chosen 
        Vector2 randomLocation = new Vector2(Random.Range(-randMax.x, randMax.x), Random.Range(-randMax.y, randMax.y));

        countryLocale.x += randomLocation.x;
        countryLocale.y += randomLocation.y;        
        return countryLocale;

        //further improvements --- record points on maps and recursion until the randlocation is not within x amount
    }
}
