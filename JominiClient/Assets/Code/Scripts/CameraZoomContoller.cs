using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoomContoller : MonoBehaviour
{
    private Camera cam;
    private float targetZoom;
    private float zoomfactor = 6f;
    [SerializeField]
    private float zoomLerpSpeed = 10;
    private float yVelocity = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        targetZoom = cam.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        float scrollData;
        scrollData = Input.GetAxis("Mouse ScrollWheel");
        targetZoom -= scrollData * zoomfactor;
        targetZoom = Mathf.Clamp(targetZoom, 3.0f, 15.0f);
        //cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomLerpSpeed);
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, targetZoom, ref yVelocity, Time.deltaTime * zoomLerpSpeed);
    }
}
