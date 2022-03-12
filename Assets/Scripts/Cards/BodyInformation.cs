using System;
using System.Collections;
using System.Collections.Generic;
using Cards;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class BodyInformation : MonoBehaviour {

    [SerializeField] private AnimationCurve RotationCurve;
    [SerializeField] private AnimationCurve JumpCurve;

    private GUIFunctions _guiFunctions;
    
    public AnimationCurve GetRotationCurve()
    {
        return RotationCurve;
    }

    public AnimationCurve GetJumpCurve()
    {
        return JumpCurve;
    }

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
        _guiFunctions = GameObject.FindWithTag("UIController").GetComponent<GUIFunctions>();
    }

    private void OnMouseUp()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonUp(0))
        {
            _guiFunctions.UpdateCardInfo(_card);
        }
    }

    private void OnMouseDrag()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        
        _guiFunctions.HideCardInfo();
    }
}
