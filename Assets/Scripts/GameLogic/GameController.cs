using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        private void Start()
        {
            if (GameSettings.Multiplayer)
                MultiplayerStart();
            else
                SingleplayerStart();

            //set camera location and borders
            CameraController cameraController = mainCamera.GetComponent<CameraController>();
            cameraController.SetViewAtCoords(mapGeneration.GetSpawnCoords());
            cameraController.SetViewBorders(mapGeneration.GetMapUnityWidth(), mapGeneration.GetMapUnityHeight());
        }

        private void SingleplayerStart()
        {
            Setup();
        }

        private void MultiplayerStart()
        {
            Setup();
        }

        private void Setup()
        {
            mapGeneration.Initialize(1);
            pathBuilder.Initialize(mapGeneration: mapGeneration);
            _players = PhotonNetwork.PlayerList;
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