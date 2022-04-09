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

        public void AddMessageLog(MessageLogUI messageLog)
        {
            _messageLog = messageLog;
        }

        private void Awake()
        {
            PhotonNetwork.Reconnect();
            var snd = SoundController.Instance;
            snd.PlayMusic(snd.LobbyMusic);
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
            var snd = SoundController.Instance;
            snd.PlaySound(snd.ButtonSnd);
            GameSettings.Multiplayer = false;
            PhotonNetwork.CreateRoom(null, _privateRoomOptions);
        }

        public void CreateRoom(Text roomName)
        {
            var snd = SoundController.Instance;
            snd.PlaySound(snd.ButtonSnd);
            PhotonNetwork.CreateRoom(roomName.text, _roomIsPrivate ? _privateRoomOptions : _publicRoomOptions);
        }
        public void SetRoomPrivacy(Toggle toggle)
        {
            var snd = SoundController.Instance;
            snd.PlaySound(snd.ButtonSnd);
            _roomIsPrivate = toggle.isOn;
        }

        public void FindRoomByName(Text roomName)
        {
            var snd = SoundController.Instance;
            snd.PlaySound(snd.ButtonSnd);
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
            if (GameSettings.Multiplayer)
                SceneManager.LoadScene("GameRoom");
            else
                SceneManager.LoadScene("Game");
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            _messageLog.AddMessage("Could not join");
        }

        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinLobby();
        }

        #endregion
    }
}
