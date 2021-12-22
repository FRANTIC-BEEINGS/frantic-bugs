using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera camera;
    private double angle;
    private bool canMoveCamera;
    private float distanceToCards;
    // projection - point at which camera ray intersects plane with cards
    private float projectionX, projectionY, projectionZ;

    // speed of camera
    [SerializeField] private float zoomSpeed;
    [SerializeField] private float movementSpeed;

    // borders on the edge of the screen on which cursor moves camera (in pixels)
    [SerializeField] private float bordersWidth;

    //
    [SerializeField] private float minZPoint;
    [SerializeField] private float maxZPoint;

    // extreme points that camera can see
    [SerializeField] private float minXPoint;
    [SerializeField] private float maxXPoint;
    [SerializeField] private float minYPoint;
    [SerializeField] private float maxYPoint;

    // switchers
    [SerializeField] public bool zoomEnabled;
    [SerializeField] public bool zoomToCursorEnabled;
    [SerializeField] public bool buttonsCameraMoveEnabled;
    [SerializeField] public bool cursorCameraMoveEnabled;



    void Awake()
    {
        canMoveCamera = true;
        camera = GetComponent<Camera>();
        angle = (this.transform.rotation.x / 360) * Math.PI;
    }

    void Update()
    {
        if (canMoveCamera)
        {
            UpdateCoordinates();
            if (zoomEnabled) Zoom();
            if (buttonsCameraMoveEnabled) ButtonsCameraMove();
            if (cursorCameraMoveEnabled) CursorCameraMove();
        }
    }

    void UpdateCoordinates()
    {
        // distanceToCards * cos(angle) = z
        distanceToCards = (float)((-this.transform.position.z) / Math.Cos(angle));
        projectionX = this.transform.position.x;
        projectionY = (float)(this.transform.position.y - distanceToCards * Math.Sin(angle));
        projectionZ = 0;
    }

    void Zoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            Vector3 direction;
            if (zoomToCursorEnabled)
                direction = Camera.main.ScreenPointToRay(Input.mousePosition).direction * scroll * zoomSpeed * Time.deltaTime;
            else
                direction = transform.forward * scroll * zoomSpeed * Time.deltaTime;

            if ((direction[2] > 0 && this.transform.position.z < maxZPoint) ||
                (direction[2] < 0 && this.transform.position.z > minZPoint))
            {
                transform.position += direction;
            }
        }
    }

    void ButtonsCameraMove()
    {
        float horizontal = Input.GetAxis("Camera Horizontal");
        float vertical = Input.GetAxis("Camera Vertical");

        if (horizontal != 0.0f || vertical != 0.0f)
        {
            Vector3 direction = new Vector3(horizontal, vertical, 0);
            MoveCamera(direction);
        }
    }

    void CursorCameraMove()
    {
        if (Input.mousePosition.x >= Screen.width  - bordersWidth || Input.mousePosition.x <= 0 + bordersWidth ||
            Input.mousePosition.y >= Screen.height - bordersWidth || Input.mousePosition.y <= 0 + bordersWidth)
        {
            Vector3 direction = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2, 0);
            MoveCamera(direction);
        }
    }

    void MoveCamera(Vector3 direction)
    {
        direction = Vector3.Normalize(direction);
        direction *= Time.deltaTime;
        direction *= movementSpeed;

        if  ((direction[0] < 0 && projectionX <= minXPoint) ||
             (direction[0] > 0 && projectionX >= maxXPoint))
        {
            direction[0] = 0;
        }
        if  ((direction[1] < 0 && projectionY <= minYPoint) ||
             (direction[1] > 0 && projectionY >= maxYPoint))
        {
            direction[1] = 0;
        }
        if  ((direction[2] < 0 && this.transform.position.z <= minZPoint) ||
             (direction[2] > 0 && this.transform.position.z >= maxZPoint))
        {
            direction[2] = 0;
        }

        transform.position += direction;
    }
}
