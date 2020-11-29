using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PC_Pan : MonoBehaviour {
    private float dist;
    private Vector3 MouseStart, MouseMove;

    void Start() {
        dist = transform.position.z;  // Distance camera is above map
    }

    void Update() {
        if (Input.GetMouseButtonDown(2)) {
            MouseStart = new Vector3(Input.mousePosition.x, Input.mousePosition.y, dist);
        }
        else if (Input.GetMouseButton(2)) {
            MouseMove = new Vector3((Input.mousePosition.x - MouseStart.x), Input.mousePosition.y - MouseStart.y, dist);
            MouseStart = new Vector3(Input.mousePosition.x, Input.mousePosition.y, dist);

            transform.position = new Vector3(transform.position.x + MouseMove.x * Time.deltaTime, transform.position.y + MouseMove.y * Time.deltaTime, dist);
            
        }
    }
}
