using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject turnControlPanel;
    [SerializeField] private GameObject buttonStart;

    private void Start()
    {
        turnControlPanel.SetActive(false);
        buttonStart.SetActive(true);
    }

    public void OnGameStarted()
    {
        turnControlPanel.SetActive(true);
        buttonStart.SetActive(false);
    }
}
