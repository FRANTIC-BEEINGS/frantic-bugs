using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class MessageLogUI : MonoBehaviour
    {
        [SerializeField] private GameObject messageContainer;
        [SerializeField] private GameObject messageItem;
        //private List<string> _systemMessages = new List<string>();
        private ScrollRect _scrollRect;

        private void Start()
        {
            _scrollRect = gameObject.GetComponentInChildren<ScrollRect>();
        }

        public void AddMessage(string message)
        {
            //_systemMessages.Add(message);
            GameObject newMessage = Instantiate(messageItem, messageContainer.transform);
            newMessage.GetComponent<Text>().text = message;
            ScrollToBottom();
        }

        private void ScrollToBottom()
        {
            Canvas.ForceUpdateCanvases();
            _scrollRect.verticalNormalizedPosition = 0;
        }

        #region SceneChangeHandling

        void OnEnable() {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
 
        void OnDisable() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            GameObject.FindWithTag("UIController").GetComponent<UIController>().AddMessageLog(this);
            if (SceneManager.GetActiveScene().name == "Lobby")
                AddMessage("Connected");
            if (SceneManager.GetActiveScene().name == "Game")
                AddMessage("Joined room " + PhotonNetwork.CurrentRoom.Name);
        }

        #endregion
    }
}
