using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum MouseButtons {
    Left = 0,
    Right = 1,
    Nothing,
    Both
}

enum BuildingState {
    Forbidden, // wait for Allowed
    Allowed,   // wait for input
    Started,   // wait for finished
    Finished   // go to Forbidden
}

public class PathBuilder : MonoBehaviour
{
    [SerializeField] private MouseButtons MouseState;
    [SerializeField] private BuildingState PathState;
    [SerializeField] private bool OnCard;
    public List<GameObject> Path;

    void Start() {
        MouseState = MouseButtons.Nothing;
        PathState = BuildingState.Forbidden;
        OnCard = false;
    }

    void Update() {
        UpdateOnCard();
        UpdateMouseState();
        UpdateBuildingState();
    }

    void UpdateBuildingState() {
      /*
        if (PathState == BuildingState.Forbidden) {
            // Каким-то образом узнать, можно ли строить путь.
            if (true) {
                PathState = BuildingState.Allowed;
            }
        }
        else if (PathState == BuildingState.Allowed) {
            if (MouseState == MouseButtons.Left) {
                //
            }
        }
      */
    }

    void UpdateOnCard() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;
        OnCard = false;
        if (Physics.Raycast(ray, out rayHit, 100.0f)) {
            if (rayHit.collider.tag == "Card") {
                OnCard = true;
            }
        }
    }

    void UpdateMouseState() {
        if (Input.GetMouseButton((int)MouseButtons.Left)) {
            if (Input.GetMouseButton((int)MouseButtons.Right)) {
                MouseState = MouseButtons.Both;
            }
            else {
                MouseState = MouseButtons.Left;
            }
        }
        else {
            if (Input.GetMouseButton((int)MouseButtons.Right)) {
                MouseState = MouseButtons.Right;
            }
            else {
                MouseState = MouseButtons.Nothing;
            }
        }
    }
}
