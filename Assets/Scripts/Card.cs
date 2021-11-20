using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using UnityEngine;

public class Card : MonoBehaviour
{
    private bool _isVisible = false;
    [SerializeField] private Sprite face;
    [SerializeField] private Sprite back;
    private Unit currentUnit;

    public bool StepOn(Unit unit)
    {
        if(!CanStepOn(unit)) return false;
        if (currentUnit != null)
        {
            //fight with enemy unit
            //TODO:update current unit
        }
        unit.transform.parent = this.transform; //change parent of the unit in the hierarchy (check later)
        return true;
    }

    private bool CanStepOn(Unit unit)
    {
        return currentUnit == null || unit.GetAllegiance() == currentUnit.GetAllegiance();
    }
    
    //get/set card visibility (also calls method for flipping card on visibility change)
    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            UpdateCardView(value);
            _isVisible = value;
        }
    }

    //play flipping animation and change visibility value
    private void UpdateCardView(bool visibility)
    {
        if (visibility == _isVisible) return;
        //TODO: play animation
        _isVisible = visibility;
        GetComponent<SpriteRenderer>().sprite = _isVisible ? face : back;
    }

    //assign card back image
    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = back;
    }
}
