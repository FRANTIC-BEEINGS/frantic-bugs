using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
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
        [SerializeField] private GUIController guiController;
        
        //network
        private Photon.Realtime.Player[] _players;
        
        //player
        [SerializeField] private GameObject playerPrefab;
        //unit
        [SerializeField] private GameObject fighterPrefab;
        
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
            TestSpawner();
            // Setup();
        }

        private void MultiplayerStart()
        {
            TestSpawner();
            // Setup();
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
            setPlayerIds();
            mapGeneration.MapGenerated += SpawnSecond;
            mapGeneration.Initialize(PhotonNetwork.PlayerList.Length);
        }

        private void TestSpawner()
        {
            MultiplayerSetup();
        }

        private void SpawnSecond()
        {
            GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);

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
            else if (PhotonNetwork.LocalPlayer.ActorNumber == playerIds[1])
            {
                Debug.Log("spawn second");
                GameObject unitGO = PhotonNetwork.Instantiate(fighterPrefab.name, mapGeneration.GetSecondSpawnCoords(), Quaternion.identity);
                Unit unit = unitGO.GetComponent<Unit>();
                UnitCardInteractionController.StepOnCard(unit, mapGeneration.GetSecondSpawnCard());
                
                //set camera
                CameraController cameraController = mainCamera.GetComponent<CameraController>();
                cameraController.SetViewAtCoords(mapGeneration.GetSecondSpawnCoords());
                cameraController.SetViewBorders(mapGeneration.GetMapUnityWidth(), mapGeneration.GetMapUnityHeight());
            }
            // set path builder map
            // pathBuilder.Map = mapGeneration;
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