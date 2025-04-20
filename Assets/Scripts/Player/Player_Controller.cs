using UnityEngine;
using UnityEngine.EventSystems;

// Contains the player mouse/touch input. For Keyboard Controls see Keyboard_Controls class.
public class Player_Controller : MonoBehaviour {
    // TODO: MVC pattern for player controller? 
    public static Faction playerControlledFaction; // TODO: This should not be here. It should ideally be on it's own thing, and preferably static.
    
    public Hud_Controller hudController;
    public Region_Panel_Script regionPanelScript; // TODO: PlayerController and RegionPanelScript should not both reference eacother as it is too tightly coupled.

    private GameObject lastRegionHit;
    private Collider2D lastRegionHitCollider;

    static Player_Controller(){
        playerControlledFaction = new Devil_Controller();
    }

    public GameObject GetRegionHit(){
        return lastRegionHit;
    }

    public static bool PlayingAsDevil() {
        return playerControlledFaction.GetType() == typeof(Devil_Controller);
    }

    public static bool PlayingAsGod() {
        return playerControlledFaction.GetType() == typeof(God_Controller);
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
                RaycastHit2D objectHit = DrawRayCast();


                // Raycast hit something that is a region.
                if (objectHit && objectHit.collider.gameObject.GetComponent<Region_Controller>() != null){
                    if (lastRegionHit != null){
                        // deactivate the borders of the last region.
                        DeactivateBorder();
                    }

                    SetRegionHitInfo(objectHit);
                    SetCountryPanelUI(objectHit);
                    ActivateBorder();
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

    private void ActivateBorder() {
        lastRegionHit.GetComponent<Region_Controller>().ActivateBorder();
    }

    private void DeactivateBorder() {
        lastRegionHit.GetComponent<Region_Controller>().DeactivateBorder();
    }

    private void DeselectRegion(){
        DeactivateBorder();
        StoreNullHitInfo();
        regionPanelScript.UpdateRegionPanel(null);
    }

    private RaycastHit2D DrawRayCast(){
        // Description of ray from camera through to the screen at mouse position.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // Cast ray. Set hit2D.
        RaycastHit2D raycastHit2D = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
        return raycastHit2D;
    }

    private void SetRegionHitInfo(RaycastHit2D raycastHit2D){
        // Store collider as class wide object.
        lastRegionHitCollider = raycastHit2D.collider;
        // Store country hit as class wide object.
        lastRegionHit = raycastHit2D.collider.gameObject;
    }

    private void StoreNullHitInfo(){
        // Store collider as class wide object.
        lastRegionHitCollider = null;
        // Store country hit as class wide object.
        lastRegionHit = null;
    }

    private void SetCountryPanelUI(RaycastHit2D raycastHit2D){
        regionPanelScript.UpdateRegionPanel(lastRegionHit);
    }

    public Vector3 GetRegionRandomLocale() {
        return GetRegionRandomLocale(lastRegionHitCollider);
    }

    // TODO: Should not be on the player controller.
    public Vector3 GetRegionRandomLocale(Collider2D collider){
        Vector3 randomPosition;
        Vector3 colliderPos = collider.transform.position;
        do {
            randomPosition = new Vector3(
                    Random.Range(collider.bounds.min.x, collider.bounds.max.x),
                    Random.Range(collider.bounds.min.y, collider.bounds.max.y),
                    colliderPos.z);
        } while (!lastRegionHitCollider.OverlapPoint(randomPosition));

        randomPosition.z -= 1;
        return randomPosition;
    }
}
