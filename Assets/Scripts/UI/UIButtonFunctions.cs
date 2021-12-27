using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UI;
using UnityEngine.SceneManagement;

public class UIButtonFunctions : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject activePanel;
    [SerializeField] private MessageLogUI messageLog;
    
    public void SwitchActivePanel(GameObject newActivePanel)
    {
        activePanel.SetActive(false);
        newActivePanel.SetActive(true);
        activePanel = newActivePanel;
    }

    public void Login(InputField nickname)
    {
        PhotonNetwork.NickName = nickname.text;
        messageLog.AddMessage("Logging in as " + nickname.text + "...");
        if(PhotonNetwork.ConnectUsingSettings())
            messageLog.AddMessage("Connecting...");
        else
            messageLog.AddMessage("Server not responding...");
    }

    public void Quit()
    {
        Application.Quit();
    }

    #region PunCallbacks

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("Lobby");
    }

    #endregion
}
