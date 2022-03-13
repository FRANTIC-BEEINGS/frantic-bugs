using System;
using Cards;
using GameLogic;
using Photon.Pun;
using ResourceManagment;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class GUIFunctions : MonoBehaviour, UIController
    {
        [SerializeField] private CardInfoUI cardInfoUI;

        [SerializeField] private Text energyCount;
        [SerializeField] private Text foodCount;
        [SerializeField] private Text moneyCount;
        [SerializeField] private Text currentLevel;
        [SerializeField] private Text turnTimerText;
        [SerializeField] private Button timerButton;
        
        [SerializeField] private Text foodToWin;
        [SerializeField] private Text moneyToWin;

        [SerializeField] private Button manualLevelUpButton;
        
        [SerializeField] private Button leaveButton;
        [SerializeField] private GameObject endScreen;
        [SerializeField] private Text endText;

        [SerializeField] private GameObject goals;
    
        private MessageLogUI _messageLog;
        private Canvas _logRenderer;
        
        public Action GetResourceButtonAction;

        public void GetResourceButton()
        {
            GetResourceButtonAction();
        }

        private void Start()
        {
            if (_logRenderer != null) _logRenderer.enabled = false;
        }
        
        public void SetTimerTurnValue(bool yourTurn)
        {
            turnTimerText.text = yourTurn ? "Your Turn " : "Enemy Turn: ";
            timerButton.interactable = yourTurn;
        }
    
        public void OnGameStarted()
        {
        
        }

        public void Leave()
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("Lobby");
        }

        public void DisplayGoals()
        {
            goals.SetActive(true);
        }

        public void AddLevelUpButtonAction(UnityAction action)
        {
            manualLevelUpButton.onClick.AddListener(action);
        }

        public void ShowManualLevelUpUI(bool isVisible)
        {
            manualLevelUpButton.gameObject.SetActive(isVisible);
        }

        public void UpdateCardInfo(Card card)
        {
            if(!card.IsVisible)
                return;
            if ((card.GetCurrentUnit() == null || 
                 card.GetCurrentUnit().GetComponent<PhotonView>().IsMine) && 
                (card is EmptyCard || card is SpawnerCard))
            {
                cardInfoUI.gameObject.SetActive(false);
                return;
            }
            cardInfoUI.gameObject.SetActive(true);
            cardInfoUI.DisplayCardInfo(card);
        }

        public void HideCardInfo()
        {
            cardInfoUI.gameObject.SetActive(false);
        }

        public void SetGameGoals(int food, int money)
        {
            foodToWin.text = food.ToString();
            moneyToWin.text = money.ToString();
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
            Debug.Log("level " + level);
            currentLevel.text = level.ToString();
        }

        public void OnWin()
        {
            endScreen.gameObject.SetActive(true);
            endText.text = "You won";
            leaveButton.gameObject.SetActive(true);
        }

        public void OnLoss()
        {
            endScreen.gameObject.SetActive(true);
            endText.text = "You lost";
            leaveButton.gameObject.SetActive(true);
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

            if (!endScreen.gameObject.activeSelf && Input.GetButtonDown("Pause"))
            {
                leaveButton.gameObject.SetActive(leaveButton.gameObject.activeSelf);
            }
        }
    }
}
