using System.Collections;
using System.Collections.Generic;
using UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ButtonOnClickAdd : MonoBehaviour
{
    public ButtonType buttonType;
    private bool onClickAdded = false;

    void Update()
    {
        if (!onClickAdded)
        {
            SetButtonOnClickMethod();
        }
    }

    private void SetButtonOnClickMethod()
    {
        if (buttonType == ButtonType.PlayerReady)
            SetPlayerReady();
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
}
