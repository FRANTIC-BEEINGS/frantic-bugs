using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject turnControlPanel;
    [SerializeField] private GameObject buttonStart;
    [SerializeField] private GameObject curtain;

    private void Start()
    {
        //turnControlPanel.SetActive(false);
        curtain.SetActive(true);
        buttonStart.SetActive(true);
    }

    public void OnGameStarted()
    {
        //turnControlPanel.SetActive(true);
        curtain.SetActive(false);
        buttonStart.SetActive(false);
    }
}
