using System;
using System.Collections;
using System.Collections.Generic;
using Cards;
using ResourceManagment;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject turnControlPanel;
    [SerializeField] private GameObject buttonStart;
    [SerializeField] private GameObject curtain;
    [SerializeField] private CardInfoUI cardInfoUI;

    [SerializeField] private GameObject resources;
    [SerializeField] private Text energyCount;
    [SerializeField] private Text foodCount;
    [SerializeField] private Text moneyCount;

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

    public void UpdateCardInfo(Card card)
    {
        cardInfoUI.gameObject.SetActive(true);
        cardInfoUI.DisplayCardInfo(card);
    }

    public void UpdateResourceDisplay(Resource resource)
    {
        switch (resource.ResourceType)
        {
            case ResourceType.Energy:
                energyCount.text = resource.Amount.ToString();
                break;
            case ResourceType.Food:
                foodCount.text = resource.Amount.ToString();
                break;
            case ResourceType.Money:
                moneyCount.text = resource.Amount.ToString();
                break;
            default:
                break;
        }
        Debug.Log("update rd");
    }
}
