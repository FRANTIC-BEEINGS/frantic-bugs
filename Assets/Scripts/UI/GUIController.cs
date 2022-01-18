using System;
using System.Collections;
using System.Collections.Generic;
using Cards;
using ResourceManagment;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour, UIController
{
    [SerializeField] private CardInfoUI cardInfoUI;

    [SerializeField] private GameObject resources;
    [SerializeField] private Text energyCount;
    [SerializeField] private Text foodCount;
    [SerializeField] private Text moneyCount;
    [SerializeField] private Text currentLevel;

    [SerializeField] private GameObject endScreen;
    // [SerializeField] private Text endText;
    // [SerializeField] private Button restartButton;
    
    private MessageLogUI _messageLog;
    private Canvas _logRenderer;

    private void Start()
    {
        _logRenderer.enabled = false;
    }
    
    public void OnGameStarted()
    {
        
    }

    public void UpdateCardInfo(Card card)
    {
        cardInfoUI.gameObject.SetActive(true);
        cardInfoUI.DisplayCardInfo(card);
    }

    public void SetGameGoals(int foodToWin, int moneyToWin)
    {
        
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
    }

    public void UpdateLevelDisplay(int level)
    {
        currentLevel.text = level.ToString();
    }

    public void OnWin()
    {
        // endText.gameObject.SetActive(true);
        // endText.text = "You won!";
        // restartButton.gameObject.SetActive(true);
    }

    public void OnLoss()
    {
        //endText.gameObject.SetActive(true);
        //endText.text = "You lost...";
        //restartButton.gameObject.SetActive(true);
    }

    public void AddMessageLog(MessageLogUI messageLog)
    {
        _messageLog = messageLog;
        _logRenderer = messageLog.gameObject.GetComponentInParent<Canvas>();
    }

    private void Update()
    {
        //ui input updates
        
        //hide/show text log
        if (Input.GetButtonDown("Log"))
        {
            _logRenderer.enabled = !_logRenderer.enabled;
        }
    }
}
