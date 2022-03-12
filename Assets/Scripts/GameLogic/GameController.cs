using System;
using System.Collections.Generic;
using Photon.Pun;
using UI;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace GameLogic
{
    public class GameController : MonoBehaviourPunCallbacks
    {
        //gameplay
        [SerializeField] private MapGeneration mapGeneration;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private PathBuilder pathBuilder;
        
        //UI
        [SerializeField] private GUIFunctions guiFunctions;
        
        //network
        private Photon.Realtime.Player[] _players;
        
        //player
        [SerializeField] private GameObject playerPrefab;
        //unit
        [SerializeField] private GameObject fighterPrefab;

        public void GetResource()
        {
            //TODO: ASAP
            //UnitCardInteractionController.GetResource();
        }
        private void Death()
        {
            guiFunctions.OnLoss();
        }

        private void ChangeLevelUI(int level)
        {
            guiFunctions.UpdateLevelDisplay(level);
        }
        
        // Network
        private List<int> playerIds;

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
        }

        private void SpawnUnitsAndSetCamera()
        {
            // спавн игрока (не юнита) с его контроллером, чтобы у каждого игрока был свой
            GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);

            // спавн главного юнита первого игрока
            if (PhotonNetwork.LocalPlayer.ActorNumber == playerIds[0])
            {
                GameObject unitGO = PhotonNetwork.Instantiate(fighterPrefab.name, mapGeneration.GetFirstSpawnCoords(), Quaternion.identity);
                Unit unit = unitGO.GetComponent<Unit>();
                unit.OnDeath += Death;
                unit.OnLevelChange += ChangeLevelUI;
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
                unit.OnDeath += Death;
                unit.OnLevelChange += ChangeLevelUI;
                UnitCardInteractionController.StepOnCard(unit, mapGeneration.GetSecondSpawnCard());
                
                //set camera
                CameraController cameraController = mainCamera.GetComponent<CameraController>();
                cameraController.SetViewAtCoords(mapGeneration.GetSecondSpawnCoords());
                cameraController.SetViewBorders(mapGeneration.GetMapUnityWidth(), mapGeneration.GetMapUnityHeight());
            }
            // set path builder map
            pathBuilder.Initialize(mapGeneration: mapGeneration);
        }

        private void setPlayerIds()
        {
            playerIds = new List<int>();
            foreach (var player in PhotonNetwork.PlayerList)
            {
                playerIds.Add(player.ActorNumber);
            }
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

        #endregion

        private void Update()
        {
            //todo: game input updates
        }
    }
}