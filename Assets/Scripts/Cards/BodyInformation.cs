using System;
using System.Collections;
using System.Collections.Generic;
using Cards;
using UnityEngine;
using UnityEngine.EventSystems;

public class BodyInformation : MonoBehaviour {
    public int id;
    public GameObject Highlight;
    public GameObject Selection;
    private Card _card;

    public void SetSelection(bool v) {
        Selection.SetActive(v);
    }

    public void SetHighlight(bool v) {
        Highlight.SetActive(v);
    }

    private void Start()
    {
        _card = gameObject.GetComponentInParent<Card>();
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            UIController uiController = GameObject.FindWithTag("UIController").GetComponent<UIController>();
            uiController.UpdateCardInfo(_card);
        }

        _card.IsVisible = !_card.IsVisible;
    }
}
