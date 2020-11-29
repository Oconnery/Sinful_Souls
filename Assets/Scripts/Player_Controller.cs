using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player_Controller : MonoBehaviour {

    public GameObject CountryPanel;
    private GameObject CountryHit;
    private Collider2D CountryHitCollider;

    public GameObject GetCountryHit(){
        return CountryHit;
    }

    private void Update() {
        if (Input.touchCount > 0){ 
            if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) {
                // Fix methods so that they also accept touch.position etc.
                Touch touch = Input.GetTouch(0);

                Debug.Log("Mouse pos" + Input.mousePosition);
                RaycastHit2D objectHit = InstantiateCastRay();

                if (objectHit){
                    Debug.Log("Clicked country: " + objectHit.collider.gameObject);
                    StoreHitInfoGlobally(objectHit);
                    SetCountryPanelUI(objectHit);
                }
            }
        }

        // Left click.
        else if (Input.GetMouseButtonDown(0)) {
            // Check that pointer is not over a game object.
            if (!EventSystem.current.IsPointerOverGameObject()) {
                Debug.Log("Mouse pos" + Input.mousePosition);
                RaycastHit2D objectHit = InstantiateCastRay();

                if (objectHit) {
                    Debug.Log("Clicked country: " + objectHit.collider.gameObject);
                    StoreHitInfoGlobally(objectHit);
                    SetCountryPanelUI(objectHit);
                }
            }
        }
    }

    /// <summary>
    /// Draw cast ray.
    /// </summary>
    /// <returns> Object detailing information about the object raycasted to.</returns>
    private RaycastHit2D InstantiateCastRay(){
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
    private void StoreHitInfoGlobally(RaycastHit2D raycastHit2D){
        // Store collider as class wide object.
        CountryHitCollider = raycastHit2D.collider;
        // Store country hit as class wide object.
        CountryHit = raycastHit2D.collider.gameObject;
    }

    /// <summary>
    /// Sets the country panel UI.
    /// </summary>
    private void SetCountryPanelUI(RaycastHit2D raycastHit2D){
        // Change the information displayed on the panel
        CountryPanel.GetComponent<Country_Info>().ChangeCountryPanelText(raycastHit2D.collider.gameObject);
        CountryPanel.SetActive(true);
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
        CountryPanel.transform.position = new Vector3(positionX, positionY, 0.0f);
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

    public Vector3 GetRandomCountryLocale(){
        Vector3 countryLocale = GetCountryHit().transform.position;
        countryLocale.z -= 1; //put it ahead of other game objs

        //random location on game object  --- or even better would be the collider (size of actuat country)
        Vector2 randMax = CountryHitCollider.bounds.size / 4; // /2 would work if there was no empty space. /4 randomly chosen 
        Vector2 randomLocation = new Vector2(Random.Range(-randMax.x, randMax.x), Random.Range(-randMax.y, randMax.y));

        countryLocale.x += randomLocation.x;
        countryLocale.y += randomLocation.y;        
        return countryLocale;

        //further improvements --- record points on maps and recursion until the randlocation is not within x amount
    }
}//end class