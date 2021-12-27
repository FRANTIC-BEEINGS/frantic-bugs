using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class UIMainMenuFunctions : MonoBehaviourPunCallbacks, UIController
    {
        [SerializeField] private GameObject activePanel;
        private MessageLogUI _messageLog;

        public void AddMessageLog(MessageLogUI messageLog)
        {
            _messageLog = messageLog;
        }

        #region ButtonOnClickFunctions

        public void SwitchActivePanel(GameObject newActivePanel)
        {
            activePanel.SetActive(false);
            newActivePanel.SetActive(true);
            activePanel = newActivePanel;
        }

        public void Login(InputField nickname)
        {
            if (nickname.text.Length < 1)
            {
                _messageLog.AddMessage("Please enter a name");
                return;
            }
            
            PhotonNetwork.NickName = nickname.text;
            _messageLog.AddMessage("Logging in as " + nickname.text + "...");
            if(PhotonNetwork.ConnectUsingSettings())
                _messageLog.AddMessage("Connecting...");
            else
                _messageLog.AddMessage("Server not responding...");
        }

        public void Quit()
        {
            Application.Quit();
        }

        #endregion

        #region PunCallbacks

        public override void OnConnectedToMaster()
        {
            SceneManager.LoadScene("Lobby");
        }

        #endregion
    }
}
