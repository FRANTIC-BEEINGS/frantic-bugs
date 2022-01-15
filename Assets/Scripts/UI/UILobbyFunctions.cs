using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class UILobbyFunctions : MonoBehaviourPunCallbacks, UIController
    {
        [SerializeField] private GameObject activePanel;
        private MessageLogUI _messageLog;
        private bool _roomIsPrivate;
        public const int MaxPlayerCount = 2;
        private readonly RoomOptions _publicRoomOptions = new RoomOptions() {MaxPlayers = MaxPlayerCount, IsVisible = true};
        private readonly RoomOptions _privateRoomOptions = new RoomOptions() {MaxPlayers = MaxPlayerCount, IsVisible = false};

        private void Start()
        {
            PhotonNetwork.JoinLobby();
        }

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
            GameSettings.Multiplayer = false;
            SceneManager.LoadScene("Game");
        }

        public void CreateRoom(Text roomName)
        {
            PhotonNetwork.CreateRoom(roomName.text, _roomIsPrivate ? _privateRoomOptions : _publicRoomOptions);
        }
        public void SetRoomPrivacy(Toggle toggle)
        {
            _roomIsPrivate = toggle.isOn;
        }

        public void FindRoomByName(Text roomName)
        {
            if(roomName.text.Length<1)
            {
                _messageLog.AddMessage("Please enter a room name");
                return;
            }

            bool joinSuccess = PhotonNetwork.JoinRoom(roomName.text);
            _messageLog.AddMessage(joinSuccess ? "Connecting..." : "Room " + roomName.text + " not found");
        }

        public void JoinRandomRoom()
        {
            bool joinSuccess = PhotonNetwork.JoinRandomRoom();
            _messageLog.AddMessage(joinSuccess ? "Connecting..." : "Room not found");
        }

        public void Logout()
        {
            PhotonNetwork.Disconnect();
            if (_messageLog != null)
                Destroy(_messageLog.gameObject.transform.parent.gameObject);
            SceneManager.LoadScene("MainMenu");
        }

        #endregion
        
        #region PunCallbacks

        public override void OnJoinedRoom()
        {
            SceneManager.LoadScene("GameRoom");
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            _messageLog.AddMessage("Could not join");
        }

        #endregion
    }
}
