using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameStartController : NetworkBehaviour
{
    [SerializeField] private int requiredPlayersNumber;
    private int currentPlayersNumber = 0;
    
    int readyPlayersNumber = 0;
    private bool matchStarted = false;
    
    // Вызывается при изменении количества присоединенных игроков 
    // Передается число игроков
    public Action<int> ChangeNumberOfPlayers;
    // Вызывается с true, если нужное количество игроков присоединилось
    // C false, если кто-то вышел
    public Action<bool> CanStartPreparation;
    // Все готовы, начинаем игру
    public Action StartGame;
    
    //todo сделать присоединение игроков через Action
    private void Update()
    {
        // матч начался - отрубаем скрипт, чтобы не мешал
        if (matchStarted)
        {
            this.enabled = false;
        }
        
        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
        {
            var PlayersDict = NetworkManager.Singleton.ConnectedClients;
            
            if (currentPlayersNumber != PlayersDict.Count)
            {
                currentPlayersNumber = PlayersDict.Count;
                ChangeNumberOfPlayers?.Invoke(currentPlayersNumber);
                
                if (currentPlayersNumber == readyPlayersNumber)
                {
                    CanStartPreparation(true);
                }
                else if (currentPlayersNumber < readyPlayersNumber)
                {
                    CanStartPreparation(false);
                }
            }
            
            readyPlayersNumber = 0;
            foreach (var player in PlayersDict)
            {
                if (player.Value.PlayerObject.GetComponent<NetworkPlayerController>().readyToPlay.Value)
                {
                    readyPlayersNumber++;
                }
            }

            if (readyPlayersNumber == PlayersDict.Count)
            {
                StartGame?.Invoke();
                matchStarted = true;
            }
        }
    }
}
