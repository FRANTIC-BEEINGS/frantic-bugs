using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera camera;
    
    // speed of camera
    [SerializeField] private float zoomSpeed = 1;
    [SerializeField] private float zoomCursorSpeed = 300;
    [SerializeField] private float movementSpeed = 5f;
    
    // borders on the edge of the screen on which cursor moves camera (in pixels)
    [SerializeField] private float bordersWidth = 10f;

    // min and max camera.orthographicSize
    [SerializeField] private float minOrtho = 1.0f;
    [SerializeField] private float maxOrtho = 20.0f;

    // switchers
    [SerializeField] public bool zoomToCursorEnabled;
    [SerializeField] public bool zoomEnabled;
    [SerializeField] public bool buttonsCameraMoveEnabled;
    [SerializeField] public bool cursorCameraMoveEnabled;


     
    void Awake()
    {
        camera = GetComponent<Camera>();
    }

    void Update()
    {
        if (zoomEnabled) Zoom();
        if (zoomToCursorEnabled) ZoomToCursor();
        if (buttonsCameraMoveEnabled) ButtonsCameraMove();
        if (cursorCameraMoveEnabled) CursorCameraMove();
    }

    void Zoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0.0f)
        {
            camera.orthographicSize = Mathf.Clamp(camera.orthographicSize - scroll * zoomSpeed,
                minOrtho, maxOrtho);
        }
    }

    void ZoomToCursor()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        
        if (scroll != 0.0f && camera.orthographicSize > minOrtho && camera.orthographicSize < maxOrtho)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                camera.ScreenToWorldPoint(Input.mousePosition), Time.deltaTime * scroll * zoomCursorSpeed);
        }
    }

    void ButtonsCameraMove()
    {
        float horizontal = Input.GetAxis("Camera Horizontal");
        float vertical = Input.GetAxis("Camera Vertical");

        if (horizontal != 0.0f || vertical != 0.0f)
            MoveCamera(horizontal, vertical);
    }

    void CursorCameraMove()
    {
        float horizontal = 0.0f;
        float vertical = 0.0f;
        
        if (Input.mousePosition.x >= Screen.width - bordersWidth)
        {
            horizontal = 1f;
        }
        else if (Input.mousePosition.x <= 0 + bordersWidth)
        {
            horizontal = -1f;
        }
        
        if ( Input.mousePosition.y >= Screen.height - bordersWidth )
        {
            vertical = 1f;
        }
        else if ( Input.mousePosition.y <= 0 + bordersWidth )
        {
            vertical = -1f;
        }
        
        if (horizontal != 0.0f || vertical != 0.0f)
            MoveCamera(horizontal, vertical);
    }

    void MoveCamera(float horizontalSpeed, float verticalSpeed)
    {
        transform.Translate(new Vector2(horizontalSpeed, verticalSpeed) 
                            * (movementSpeed * Time.deltaTime));
    }

}