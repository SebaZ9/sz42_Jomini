using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDragController : MonoBehaviour
{
    public Vector3 resetCamera;
    private Vector3 origin;
    private Vector3 difference;
    private bool drag = false;
    private float minX = -4f;
    private float maxX = 18f;
    private float minY = -18f;
    private float maxY = 0f;

    void Start () {
        if(resetCamera == null) {
            resetCamera = Camera.main.transform.position;
        }
    }

    void LateUpdate () {
        if(Input.GetMouseButton(2)) {
            difference = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - Camera.main.transform.position;
            if (drag == false){
                drag = true;
                origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        } 
        else {
            drag = false;
        }

        if(drag == true){
            Vector3 bordered = origin - difference;
            // Clamp camera to hard-coded game edges.
            bordered.x = Mathf.Clamp(transform.position.x, minX, maxX);
            bordered.y = Mathf.Clamp(transform.position.y, minY, maxY);
            Camera.main.transform.position = origin - difference;
        }

        //RESET CAMERA TO STARTING POSITION WITH RIGHT CLICK
        if (Input.GetMouseButton(1)) {
            Camera.main.transform.position = resetCamera;
        }
    }
}
