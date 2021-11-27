using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyInformation : MonoBehaviour {
    public int id;
    public GameObject Highlight;
    public GameObject Selection;

    public void SetSelection(bool v) {
        Selection.SetActive(v);
    }

    public void SetHighlight(bool v) {
        Highlight.SetActive(v);
    }
}
