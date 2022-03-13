using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonOnClickAdd : MonoBehaviour
{
    [SerializeField] private GameLogic.GameController _gameController;
    public ButtonType buttonType;
    private bool onClickAdded = false;

    void Update()
    {
        if (!onClickAdded)
        {
            onClickAdded = true;
            SetButtonOnClickMethod();
        }
    }

    private void SetButtonOnClickMethod()
    {
        switch (buttonType)
        {
            case ButtonType.PlayerReady:
                SetPlayerReady();
                break;
            case ButtonType.EndTurn:
                SetEndTurn();
                break;
            case ButtonType.CaptureCard:
                CaptureCard();
                break;
            case ButtonType.GetResource:
                GetResource();
                break;
            case ButtonType.Restart:
                Restart();
                break;
        }
        
    }
    
    private void SetPlayerReady()
    {
        //tmp add host game feature on ready button (for single player)
        // NetworkManager.Singleton.StartHost();
        //
        // if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsClient)
        // {
        //     Button btn = gameObject.GetComponent<Button>();
        //     btn.onClick.AddListener(NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject()
        //         .GetComponent<NetworkPlayerController>().Ready);
        //     onClickAdded = true;
        // }
    }
    
    private void SetEndTurn()
    {
        Button btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(EndTurn);
        onClickAdded = true;
    }
    private void EndTurn()
    {
        if (!_gameController.GetPlayerController().thisPlayerTurn)
            return;
        if (PhotonNetwork.IsMasterClient)
        {
            _gameController.NextTurn();
            return;
        }

        PhotonView photonView = PhotonView.Get(_gameController.GetPlayerController());
        photonView.RPC("EndTurnRpc", RpcTarget.MasterClient);
    }
    
    
    private void CaptureCard()
    {
        // if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsClient)
        // {
        //     Button btn = gameObject.GetComponent<Button>();
        //     btn.onClick.AddListener(_gameController.CaptureCard);
        //     onClickAdded = true;
        // }
    }
    
    private void GetResource()
    {
        // if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsClient)
        // {
        //     Button btn = gameObject.GetComponent<Button>();
        //     btn.onClick.AddListener(_gameController.GetResource);
        //     onClickAdded = true;
        // }
    }

    private void Restart()
    {
        // Destroy(GameObject.Find("NetworkManager"));
        // SceneManager.LoadScene(0);
    }
}
