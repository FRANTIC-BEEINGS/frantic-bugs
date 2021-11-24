using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum MouseButtons {
    Left = 0,
    Right = 1,
    Nothing,
    Both
}

public class PathBuilder : MonoBehaviour
{
    [SerializeField] private MouseButtons MouseState;

    void Start() {
        MouseState = MouseButtons.Nothing;
    }

    void Update() {
        UpdateMouseState();
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
