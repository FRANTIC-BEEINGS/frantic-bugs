using System;
using System.Collections;
using System.Collections.Generic;
using Cards;
using GameLogic;
using Photon.Pun;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class BodyInformation : MonoBehaviour {

    [SerializeField] private AnimationCurve RotationCurve;
    [SerializeField] private AnimationCurve JumpCurve;

    private GUIFunctions _guiFunctions;
    private GameLogic.GameController _gameController;
    
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
        _gameController = GameObject.FindWithTag("GameController").GetComponent<GameLogic.GameController>();
    }

    private void OnMouseUp()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        
        _gameController.lastClickedCard = _card;
    }
    
    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            _guiFunctions.UpdateCardInfo(_card);
        }
    }
}
