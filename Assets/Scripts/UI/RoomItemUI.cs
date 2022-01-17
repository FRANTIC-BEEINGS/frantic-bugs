using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class RoomItemUI : MonoBehaviour
    {
        private readonly string _countOutOf = "/" + UILobbyFunctions.MaxPlayerCount;
        [SerializeField] private Text roomNameTextField;
        [SerializeField] private Text playerCountTextField;
        private UILobbyFunctions _uiController;

        private void Start()
        {
            _uiController = GameObject.FindWithTag("UIController").GetComponent<UILobbyFunctions>();
        }

        public void SetRoomName(string roomName)
        {
            roomNameTextField.text = roomName;
        }

        public void SetPlayerCount(int playerCount)
        {
            playerCountTextField.text = playerCount + _countOutOf;
        }

        public void OnItemClick()
        {
            _uiController.FindRoomByName(roomNameTextField);
        }
    }
}
