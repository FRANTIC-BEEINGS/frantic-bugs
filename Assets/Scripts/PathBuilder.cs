using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum MouseButtons {
    Left = 0,
    Right = 1,
    Nothing = 2,
    Both = 3
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
        //Debug.Log(MouseState);

        if (Input.GetMouseButton(0)) { // pressed left button
            if (MouseState == MouseButtons.Nothing) {
                MouseState = MouseButtons.Left;
            }
            else if (MouseState == MouseButtons.Right) {
                MouseState = MouseButtons.Both;
            }
        }
        else { // unpressed left button
            if (MouseState == MouseButtons.Left) {
                MouseState = MouseButtons.Nothing;
            }
            else if (MouseState == MouseButtons.Both) {
                MouseState = MouseButtons.Right;
            }
        }
        if (Input.GetMouseButton(1)) { // pressed right button
            if (MouseState == MouseButtons.Nothing) {
                MouseState = MouseButtons.Both;
            }
            else if (MouseState == MouseButtons.Left) {
                MouseState = MouseButtons.Both;
            }
        }
        else { // unpressed right button
            if (MouseState == MouseButtons.Right) {
                MouseState = MouseButtons.Nothing;
            }
            else if (MouseState == MouseButtons.Both) {
                MouseState = MouseButtons.Left;
            }
        }
    }
}
