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
    [SerializeField] private GameObject turnControlPanel;
    [SerializeField] private GameObject buttonStart;
    [SerializeField] private GameObject curtain;
    [SerializeField] private CardInfoUI cardInfoUI;
    [SerializeField] private GameInfoUI gameInfoUI;

    [SerializeField] private GameObject resources;
    [SerializeField] private Text energyCount;
    [SerializeField] private Text foodCount;
    [SerializeField] private Text moneyCount;

    [SerializeField] private Text endText;
    [SerializeField] private Button restartButton;
    
    private MessageLogUI _messageLog;

    private void Start()
    {
        var soundController = SoundController.Instance;
        soundController.PlayMusic(soundController.LobbyMusic);
        //turnControlPanel.SetActive(false);
        curtain.SetActive(true);
        buttonStart.SetActive(true);
    }

    public GameInfoUI GetGameInfoUI()
    {
        return gameInfoUI;
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

    public void OnWin()
    {
        curtain.SetActive(true);
        endText.gameObject.SetActive(true);
        endText.text = "You won!";
        restartButton.gameObject.SetActive(true);
    }

    public void OnLoss()
    {
        curtain.SetActive(true);
        endText.gameObject.SetActive(true);
        endText.text = "You lost...";
        restartButton.gameObject.SetActive(true);
    }

    public void AddMessageLog(MessageLogUI messageLog)
    {
        _messageLog = messageLog;
    }
}
