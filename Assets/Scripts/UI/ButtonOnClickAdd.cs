using System.Collections;
using System.Collections.Generic;
using UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ButtonOnClickAdd : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    public ButtonType buttonType;
    private bool onClickAdded = false;

    void Update()
    {
        if (!onClickAdded)
        {
            // if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsHost)
            //     onClickAdded = true;
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
        }
        
    }
    
    private void SetPlayerReady()
    {
        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsClient)
        {
            Button btn = gameObject.GetComponent<Button>();
            btn.onClick.AddListener(NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject()
                .GetComponent<NetworkPlayerController>().Ready);
            onClickAdded = true;
        }
    }
    
    private void SetEndTurn()
    {
        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsClient)
        {
            Button btn = gameObject.GetComponent<Button>();
            btn.onClick.AddListener(NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject()
                .GetComponent<NetworkPlayerController>().EndTurn);
            onClickAdded = true;
        }
    }
}
