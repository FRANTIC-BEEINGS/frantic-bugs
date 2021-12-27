using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class UILobbyFunctions : MonoBehaviourPunCallbacks, UIController
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

        public void StartSoloGame()
        {
            //todo
        }

        public void CreateRoom(InputField roomName)
        {
            if(roomName.text.Length<1)
                _messageLog.AddMessage("Please enter a room name");
            //todo
            
        }

        public void FindRoomByName(InputField roomName)
        {
            if(roomName.text.Length<1)
                _messageLog.AddMessage("Please enter a room name");
            //todo
        }

        public void Logout()
        {
            PhotonNetwork.Disconnect();
            if (_messageLog != null)
                Destroy(_messageLog.gameObject.transform.parent.gameObject);
            SceneManager.LoadScene("MainMenu");
        }

        #endregion
    }
}
