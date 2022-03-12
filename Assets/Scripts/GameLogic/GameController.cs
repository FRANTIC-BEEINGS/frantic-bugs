﻿using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Realtime;

namespace GameLogic
{
    public class GameController : MonoBehaviourPunCallbacks, IInRoomCallbacks
    {
        //gameplay
        [SerializeField] private MapGeneration mapGeneration;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private PathBuilder pathBuilder;
        
        //UI
        [SerializeField] private GUIController guiController;
        
        //network
        private Photon.Realtime.Player[] _players;
        
        //player
        [SerializeField] private GameObject playerPrefab;
        //unit
        [SerializeField] private GameObject fighterPrefab;
        
        // Network
        private List<int> playerIds;
        private int IndexOfCurrentPlayerTurn = 0;
        
        // Actions
        public Action<int> NextTurnPlayerId;
        
        //timers
        private double gameStartTime = 0;
        private double lastTurnStartTime = 0;
        private double timeToNextTurn = 0;
        private double timeToEndGame = 0;

        public int GetCurrentPlayerTurnPhotonId()
        {
            if (IndexOfCurrentPlayerTurn < 0 || IndexOfCurrentPlayerTurn >= playerIds.Count)
                return -1;
            return playerIds[IndexOfCurrentPlayerTurn];
        }

        private void Start()
        {
            
            if (GameSettings.Multiplayer)
                MultiplayerStart();
            else
                SingleplayerStart();

            // //set camera location and borders
            
            //
            // Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        }

        private void SingleplayerStart()
        {
            // TestSpawner();
            // Setup();
        }

        private void MultiplayerStart()
        {
            MultiplayerSetup();
        }

        private void Setup()
        {
            mapGeneration.Initialize(1);
            pathBuilder.Initialize(mapGeneration: mapGeneration);
            // spawn unit
            GameObject unitGO = Instantiate(fighterPrefab, mapGeneration.GetFirstSpawnCoords(), Quaternion.identity);
            Unit unit = unitGO.GetComponent<Unit>();
            // unit.OnDeath += Death;
            // unit.OnLevelChange += ChangeLevelUI;
            UnitCardInteractionController.StepOnCard(unit, mapGeneration.GetFirstSpawnCard());
            _players = PhotonNetwork.PlayerList;
        }

        private void MultiplayerSetup()
        {
            // сохраняем id игроков, чтобы потом понимать, какой игрок, за какую сторону играет
            setPlayerIds();
            
            // генерация карты идет асинхронно
            // action нужен, чтобы юниты спавнились после того, как места спавна определены
            mapGeneration.MapGenerated += SpawnUnitsAndSetCamera;
            //Начать генерацию карты для нужного количества игроков
            mapGeneration.Initialize(PhotonNetwork.PlayerList.Length);
            
            //Set start time to room properties
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.AddCallbackTarget(this);
                var timeTmp = PhotonNetwork.Time;
                Hashtable ht = new Hashtable {{"GameStartTime", timeTmp}, {"LastTurnStartTime", timeTmp}};
                PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
            }
        }

        private void SpawnUnitsAndSetCamera()
        {
            // спавн игрока (не юнита) с его контроллером, чтобы у каждого игрока был свой
            GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
            player.GetComponent<PlayerController>().SetGameControllerAndSubscribe(this);

            // спавн главного юнита первого игрока
            if (PhotonNetwork.LocalPlayer.ActorNumber == playerIds[0])
            {
                GameObject unitGO = PhotonNetwork.Instantiate(fighterPrefab.name, mapGeneration.GetFirstSpawnCoords(), Quaternion.identity);
                Unit unit = unitGO.GetComponent<Unit>();
                UnitCardInteractionController.StepOnCard(unit, mapGeneration.GetFirstSpawnCard());
                
                //set camera
                CameraController cameraController = mainCamera.GetComponent<CameraController>();
                cameraController.SetViewAtCoords(mapGeneration.GetFirstSpawnCoords());
                cameraController.SetViewBorders(mapGeneration.GetMapUnityWidth(), mapGeneration.GetMapUnityHeight());
                
            }
            // спавн главного юнита второго игрока, если он есть
            else if (playerIds.Count > 1 && PhotonNetwork.LocalPlayer.ActorNumber == playerIds[1])
            {
                GameObject unitGO = PhotonNetwork.Instantiate(fighterPrefab.name, mapGeneration.GetSecondSpawnCoords(), Quaternion.identity);
                Unit unit = unitGO.GetComponent<Unit>();
                UnitCardInteractionController.StepOnCard(unit, mapGeneration.GetSecondSpawnCard());
                
                //set camera
                CameraController cameraController = mainCamera.GetComponent<CameraController>();
                cameraController.SetViewAtCoords(mapGeneration.GetSecondSpawnCoords());
                cameraController.SetViewBorders(mapGeneration.GetMapUnityWidth(), mapGeneration.GetMapUnityHeight());
            }
            // set path builder map
            pathBuilder.Initialize(mapGeneration: mapGeneration);
            NextTurn();
        }

        private void setPlayerIds()
        {
            playerIds = new List<int>();
            foreach (var player in PhotonNetwork.PlayerList)
            {
                playerIds.Add(player.ActorNumber);
            }
        }

        private void NextTurn()
        {
            //todo UI?
            if (PhotonNetwork.IsMasterClient)
            {
                int newIndexOfCurrentPlayerTurn = (IndexOfCurrentPlayerTurn + 1) % playerIds.Count;
                Hashtable ht = new Hashtable {{"LastTurnStartTime", PhotonNetwork.Time}, 
                    {"IndexOfCurrentPlayerTurn", newIndexOfCurrentPlayerTurn}};
                PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
            }
        }
        
        private void Update()
        {
            //todo: game input updates
            timeToNextTurn = (int) (GameSettings.TurnDuration - (PhotonNetwork.Time - lastTurnStartTime));
            timeToEndGame = (int) (GameSettings.GameDuration - (PhotonNetwork.Time - gameStartTime));
            if (timeToNextTurn <= 0 && lastTurnStartTime > 0 && gameStartTime > 0)
            {
                NextTurn();
            }
            Debug.Log("Update: timeToNextTurn = " + timeToNextTurn + 
                      ", turn of " + IndexOfCurrentPlayerTurn);
        }

        #region PunCallbacks

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            //todo: set game win
            if (!GameSettings.Multiplayer)
            {
                PhotonNetwork.LeaveRoom();
                SceneManager.LoadScene("Lobby");
            }
            else
            {
                SceneManager.LoadScene("GameRoom");
            }
        }
        
        public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            object propsTime;
            if (propertiesThatChanged.TryGetValue("GameStartTime", out propsTime))
            {
                gameStartTime = (double) propsTime;
            }
            
            if (propertiesThatChanged.TryGetValue("LastTurnStartTime", out propsTime))
            {
                lastTurnStartTime = (double) propsTime;
            }
            
            if (propertiesThatChanged.TryGetValue("IndexOfCurrentPlayerTurn", out propsTime))
            {
                IndexOfCurrentPlayerTurn = (int) propsTime;
                NextTurnPlayerId?.Invoke(playerIds[IndexOfCurrentPlayerTurn]);
            }
        }

        #endregion

    }
}