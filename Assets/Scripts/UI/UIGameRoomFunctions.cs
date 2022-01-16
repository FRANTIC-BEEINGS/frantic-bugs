﻿using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class UIGameRoomFunctions : MonoBehaviourPunCallbacks, UIController
    {
        private MessageLogUI _messageLog;
        [SerializeField] private Text playerList;
        public void AddMessageLog(MessageLogUI messageLog)
        {
            _messageLog = messageLog;
        }

        private void UpdatePlayerList()
        {
            playerList.text = "";
            foreach (var player in PhotonNetwork.PlayerList)
            {
                playerList.text += player.NickName + '\n';
            }
        }
        
        #region ButtonClickFunctions

        public void LeaveRoom()
        {
            _messageLog.AddMessage("Leaving room " + PhotonNetwork.CurrentRoom?.Name);
            PhotonNetwork.LeaveRoom();
        }

        public void SetPlayerReady(bool isReady)
        {
            
        }

        public void StartGame()
        {
            GameSettings.Multiplayer = true;
        }

        #endregion

        #region PunCallbacks
        
        public override void OnLeftRoom()
        {
            UpdatePlayerList();
            SceneManager.LoadScene("Lobby");
        }

        public override void OnJoinedRoom()
        {
            UpdatePlayerList();
        }

        public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
        {
            
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            UpdatePlayerList();
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            UpdatePlayerList();
        }

        #endregion
    }
}