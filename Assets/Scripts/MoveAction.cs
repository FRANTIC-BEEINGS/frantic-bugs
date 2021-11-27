using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cards;

public class MoveAction : MonoBehaviour
{
    [SerializeField] private PathBuilder aPathBuilder;
    private void Start()
    {
        aPathBuilder.pathBuilt += Move;
    }
    
    public void Move(List<Card> Path)
    {
        Debug.Log("Иду куда-то " + Path.Count);
    }
}
