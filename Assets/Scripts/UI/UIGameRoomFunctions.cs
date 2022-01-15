using Photon.Pun;
using UnityEngine.SceneManagement;

namespace UI
{
    public class UIGameRoomFunctions : MonoBehaviourPunCallbacks, UIController
    {
        private MessageLogUI _messageLog;
        public void AddMessageLog(MessageLogUI messageLog)
        {
            _messageLog = messageLog;
        }

        public void LeaveRoom()
        {
            _messageLog?.AddMessage("Leaving room " + PhotonNetwork.CurrentRoom?.Name);
            PhotonNetwork.LeaveRoom();
            if (_messageLog != null)
                Destroy(_messageLog.gameObject.transform.parent.gameObject);
            SceneManager.LoadScene("Lobby");
        }
    }
}