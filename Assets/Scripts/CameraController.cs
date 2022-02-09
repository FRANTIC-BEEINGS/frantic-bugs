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

    private Vector3 target;
    private Vector3 projection;
    private Vector3 projectionToTarget;

    // speed of camera
    [SerializeField] private float zoomSpeed;
    [SerializeField] private float movementSpeed;

    // borders on the edge of the screen on which cursor moves camera (in pixels)
    [SerializeField] private float bordersWidth;

    //
    [SerializeField] private float minDistance;
    [SerializeField] private float maxDistance;

    // extreme points that camera can see
    private float minXPoint;
    private float maxXPoint;
    private float minYPoint;
    private float maxYPoint;

    // switchers
    [SerializeField] public bool zoomEnabled;
    [SerializeField] public bool zoomToCursorEnabled;
    [SerializeField] public bool buttonsCameraMoveEnabled;
    [SerializeField] public bool cursorCameraMoveEnabled;

    //[SerializeField] GameObject projectionObject;
    //[SerializeField] GameObject targetObject;

    // coords.z must be 0
    public void SetViewAtCoords(Vector3 coords)
    {
        target = coords;
        /*
        UpdateCoordinates();
        Vector3 projection = new Vector3(projectionX, projectionY, projectionZ);
        transform.position = transform.position + coords - projection;
        */
    }

    public void SetViewBorders(float mapUnityWidth, float mapUnityHeight) {
        minXPoint = -mapUnityWidth/2;
        maxXPoint = -minXPoint;
        minYPoint = -mapUnityHeight/2;
        maxYPoint = -minYPoint;
    }

    void Awake()
    {
        SetViewBorders(100, 100);
        projectionToTarget = new Vector3(0, 0, 0);
        projection = new Vector3(0, 0, 0);
        target = new Vector3(0, 0, 0);
        canMoveCamera = true;
        camera = GetComponent<Camera>();
        angle = this.transform.rotation.x * 2;//(this.transform.rotation.x / 360.0) * Math.PI;
    }

    void Update()
    {
        if (canMoveCamera)
        {
            UpdateProjection();
            UpdateTargetPosition();
            if (zoomEnabled) Zoom();
            if (buttonsCameraMoveEnabled) ButtonsCameraMove();
            if (cursorCameraMoveEnabled) CursorCameraMove();
        }
        MoveProjectionToTarget();
    }

    void MoveProjectionToTarget()
    {
        projectionToTarget = target - projection;
        projectionToTarget *= Time.deltaTime;
        projectionToTarget *= movementSpeed;
        transform.position = transform.position + projectionToTarget;
    }

    void UpdateTargetPosition()
    {
        target[0] = Math.Max(minXPoint, target[0]);
        target[0] = Math.Min(maxXPoint, target[0]);
        target[1] = Math.Max(minYPoint, target[1]);
        target[1] = Math.Min(maxYPoint, target[1]);
        target[2] = 0;
    }

    void UpdateProjection()
    {
        // distanceToCards * cos(angle) = z
        distanceToCards = (float)((-this.transform.position.z) / Math.Cos(angle));
        projection[0] = this.transform.position.x;
        projection[1] = (float)(this.transform.position.y - distanceToCards * Math.Sin(angle));
        projection[2] = 0;
        //projectionObject.transform.position = projection;
        //targetObject.transform.position = target;
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

            if ((direction[2] > 0 && this.transform.position.z < -minDistance) ||
                (direction[2] < 0 && this.transform.position.z > -maxDistance))
            {
                target[0] += direction[0];
                target[1] += direction[1] + direction[2] * (float)Math.Tan(angle);
                projection[0] += direction[0];
                projection[1] += direction[1] + direction[2] * (float)Math.Tan(angle);
                this.transform.position = this.transform.position + direction;
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
            MoveTarget(direction);
        }
    }

    void CursorCameraMove()
    {
        if (Input.mousePosition.x >= Screen.width  - bordersWidth || Input.mousePosition.x <= 0 + bordersWidth ||
            Input.mousePosition.y >= Screen.height - bordersWidth || Input.mousePosition.y <= 0 + bordersWidth)
        {
            Vector3 direction = Input.mousePosition - new Vector3(Screen.width / 2, Screen.height / 2, 0);
            MoveTarget(direction);
        }
    }

    void MoveTarget(Vector3 direction)
    {
        direction = Vector3.Normalize(direction);
        direction *= Time.deltaTime;
        direction *= movementSpeed;

        if  ((direction[0] < 0 && projection[0] <= minXPoint) ||
             (direction[0] > 0 && projection[0] >= maxXPoint))
        {
            direction[0] = 0;
        }
        if  ((direction[1] < 0 && projection[1] <= minYPoint) ||
             (direction[1] > 0 && projection[1] >= maxYPoint))
        {
            direction[1] = 0;
        }
        if  ((direction[2] < 0 && this.transform.position.z <= minDistance) ||
             (direction[2] > 0 && this.transform.position.z >= maxDistance))
        {
            direction[2] = 0;
        }

        target += direction;
    }
}
