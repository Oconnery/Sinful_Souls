using UnityEngine;

public class Tooltip : MonoBehaviour{
    public string message;

    private void Start(){
        Debug.Log("Started");
    }

    void OnMouseEnter(){
        Debug.Log("Mouse is over tooltip area");
    }

    void OnMouseExit(){
        Debug.Log("Mouse is exiting tooltip area");
    }

    private void OnMouseOver(){
        Debug.Log("Mouse ONMOUSEOVER");
    }
}
