using System;
using System.Collections.Generic;
using Cards;
using ExitGames.Client.Photon;
using Photon.Pun;
using UI;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using ResourceManagment;

namespace GameLogic
{
    public class GameController : MonoBehaviourPunCallbacks, IInRoomCallbacks
    {
        //gameplay
        [SerializeField] private MapGeneration mapGeneration;
        [SerializeField] private GameObject tavernPrefab;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private PathBuilder pathBuilder;
        private TavernGeneration TG;

        //UI
        [SerializeField] private GUIFunctions guiFunctions;
        [SerializeField] private UITimerController gameTimer;
        [SerializeField] private UITimerController turnTimer;
        
        //network
        private Photon.Realtime.Player[] _players;
        
        //prefabs
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject fighterPrefab;
        [SerializeField] private Material localPlayerMaterial;
        
        // Network
        private List<int> playerIds;
        private int IndexOfCurrentPlayerTurn = 0;
        
        private int _foodNeededForLevelUp = 8;
        private int _moneyNeededForLevelUp = 100;
        private int _foodNeededToWin = 30;
        private int _moneyNeededToWin = 500;

        private Unit _unit;
        // Actions
        public Action<int> NextTurnPlayerId;
        
        //timers
        private double gameStartTime = 0;
        private double lastTurnStartTime = 0;
        private double timeToNextTurn = 0;
        private double timeToEndGame = 0;
        
        private PlayerController _playerController;
        public Card lastClickedCard;
        private bool isLooser = false;

        public GUIFunctions GetGuiFunctions() => guiFunctions;

        public PlayerController GetPlayerController()
        {
            return _playerController;
        }
        public bool CanGetResource()
        {
            return UnitCardInteractionController.CanGetResource(lastClickedCard as ResourceCard,
                lastClickedCard.GetCurrentUnit());
        }

        private bool HasEnoughEnergyToInteract(ResourceCard card)
        {
            return UnitCardInteractionController.HaveEnoughResourceToGetResourceCard(card,
                _playerController.GetResourceManager(), _unit);
        }
        public void GetResource()
        {
            UnitCardInteractionController.GetResource(lastClickedCard as ResourceCard,
                lastClickedCard.GetCurrentUnit(), _playerController.GetResourceManager());
        }
        private void Death()
        {
            guiFunctions.OnLoss();
        }

        private void CardInfoCollectResourceButton()
        {
            GetResource();
            guiFunctions.UpdateCardInfo(lastClickedCard);
        }

        public void UpdateCardInfo()
        {
            guiFunctions.UpdateCardInfo(lastClickedCard);
        }

        private void ChangeLevelUI(int level)
        {
            guiFunctions.UpdateLevelDisplay(level);
        }

        private void CheckSingleplayerWin(Resource resource)
        {
            ResourceManager resourceManager = _playerController.GetResourceManager();
            switch (resource.ResourceType)
            {
                case ResourceType.Food when resourceManager.GetResource(ResourceType.Food) >= _foodNeededToWin:
                    _foodNeededToWin = -1;
                    break;
                case ResourceType.Money when resourceManager.GetResource(ResourceType.Money) >= _moneyNeededToWin:
                    _moneyNeededToWin = -1;
                    break;
            }

            if (_foodNeededToWin == -1 && _moneyNeededToWin == -1)
            {
                guiFunctions.OnWin();
            }
        }

        private void CanManualLevelUp(Resource resource)
        {
            if(!GameSettings.Multiplayer)
                return;
            if (_foodNeededForLevelUp <= _playerController.GetResourceManager().GetResource(ResourceType.Food) &&
                _moneyNeededForLevelUp <= _playerController.GetResourceManager().GetResource(ResourceType.Money))
            {
                guiFunctions.ShowManualLevelUpUI(true);
            }
        }

        public void ManualLevelUp()
        {
            _unit.IncreaseLevel();
            _playerController.GetResourceManager().ConsumeResource(ResourceType.Food,_foodNeededForLevelUp);
            _playerController.GetResourceManager().ConsumeResource(ResourceType.Money,_moneyNeededForLevelUp);
            guiFunctions.ShowManualLevelUpUI(false);
        }

        public int GetCurrentPlayerTurnPhotonId()
        {
            if (IndexOfCurrentPlayerTurn < 0 || IndexOfCurrentPlayerTurn >= playerIds.Count)
                return -1;
            return playerIds[IndexOfCurrentPlayerTurn];
        }

        private void Awake()
        {
            var snd = SoundController.Instance;
            snd.PlayMusic(snd.LevelMusic);
        }

        private void Start()
        {
            
            if (GameSettings.Multiplayer)
                MultiplayerStart();
            else
                MultiplayerStart();

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
            
            if (!GameSettings.Multiplayer)
            {
                guiFunctions.DisplayGoals();
                guiFunctions.SetGameGoals(_foodNeededToWin,_moneyNeededToWin);
            }

            guiFunctions.GetResourceButtonAction = CardInfoCollectResourceButton;
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
            
            guiFunctions.AddLevelUpButtonAction(ManualLevelUp);
        }

        private void SpawnUnitsAndSetCamera()
        {
            // beautiful taverna
            TG = Instantiate(tavernPrefab).GetComponent<TavernGeneration>();
            TG.Initialize(mapGeneration.GetMapUnityWidth(), mapGeneration.GetMapUnityHeight());
            
            // спавн игрока (не юнита) с его контроллером, чтобы у каждого игрока был свой
            GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
            _playerController = player.GetComponent<PlayerController>();
            _playerController.SetGameControllerAndSubscribe(this);
            //subscribe resource update actions
            _playerController.GetResourceManager().OnResourceChange += guiFunctions.UpdateResourceDisplay;
            _playerController.GetResourceManager().OnResourceChange += CanManualLevelUp;
            if(!GameSettings.Multiplayer)
                _playerController.GetResourceManager().OnResourceChange += CheckSingleplayerWin;

            CardInfoUI.CheckEnergy = HasEnoughEnergyToInteract;

            // спавн главного юнита первого игрока
            if (PhotonNetwork.LocalPlayer.ActorNumber == playerIds[0])
            {
                GameObject unitGO = PhotonNetwork.Instantiate(fighterPrefab.name, mapGeneration.GetFirstSpawnCoords(), 
                    Quaternion.identity);
                // раскрашиваем своего юнита в другой цвет (локально)
                Transform unitModel = unitGO.transform.Find("DefaultUnit");
                for (int i = 0; i < unitModel.childCount; i++)
                {
                    unitModel.GetChild(i).gameObject.GetComponent<MeshRenderer>().materials = 
                        new []{localPlayerMaterial};
                }
                Unit unit = unitGO.GetComponent<Unit>();
                _unit = unit;
                unit.OnDeath += Death;
                unit.OnLevelChange += ChangeLevelUI;
                UnitCardInteractionController.StepOnCard(unit, mapGeneration.GetFirstSpawnCard());

                //set camera
                CameraController cameraController = mainCamera.GetComponent<CameraController>();
                cameraController.SetViewAtCoords(mapGeneration.GetFirstSpawnCoords());
                cameraController.SetViewBorders(mapGeneration.GetMapUnityWidth(), mapGeneration.GetMapUnityHeight());
                
                //set open cards near spawned unit
                VisionController visionController = unitGO.GetComponent<VisionController>();
                visionController.Initialize(mapGeneration);
                HashSet<Card> firstCards = visionController.GetCardsInVision(2, mapGeneration.GetFirstSpawnCard());
                foreach (var c in firstCards)
                {
                    c.IsVisible = true;
                }
                
            }
            // спавн главного юнита второго игрока, если он есть
            else if (playerIds.Count > 1 && PhotonNetwork.LocalPlayer.ActorNumber == playerIds[1])
            {
                GameObject unitGO = PhotonNetwork.Instantiate(fighterPrefab.name, mapGeneration.GetSecondSpawnCoords(), 
                    Quaternion.identity);
                // раскрашиваем своего юнита в другой цвет (локально)
                Transform unitModel = unitGO.transform.Find("DefaultUnit");
                for (int i = 0; i < unitModel.childCount; i++)
                {
                    unitModel.GetChild(i).gameObject.GetComponent<MeshRenderer>().materials = 
                        new []{localPlayerMaterial};
                }
                Unit unit = unitGO.GetComponent<Unit>();
                _unit = unit;
                unit.OnDeath += Death;
                unit.OnLevelChange += ChangeLevelUI;
                UnitCardInteractionController.StepOnCard(unit, mapGeneration.GetSecondSpawnCard());

                //set camera
                CameraController cameraController = mainCamera.GetComponent<CameraController>();
                cameraController.SetViewAtCoords(mapGeneration.GetSecondSpawnCoords());
                cameraController.SetViewBorders(mapGeneration.GetMapUnityWidth(), mapGeneration.GetMapUnityHeight());
                
                //set open cards near spawned unit
                VisionController visionController = unitGO.GetComponent<VisionController>();
                visionController.Initialize(mapGeneration);
                HashSet<Card> firstCards = visionController.GetCardsInVision(2, mapGeneration.GetSecondSpawnCard());
                foreach (var c in firstCards)
                {
                    c.IsVisible = true;
                }
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

        public bool CanEndTurn()
        {
            return _playerController.thisPlayerTurn;
        }
        public void NextTurn()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                int newIndexOfCurrentPlayerTurn = (IndexOfCurrentPlayerTurn + 1) % playerIds.Count;
                Hashtable ht = new Hashtable {{"LastTurnStartTime", PhotonNetwork.Time}, 
                    {"IndexOfCurrentPlayerTurn", newIndexOfCurrentPlayerTurn}};
                PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
            }
        }

        public void Loss(int playerId)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == playerId)
                guiFunctions.OnLoss();
            else 
                guiFunctions.OnWin();
        }
        
        public void Win(int playerId)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == playerId)
                guiFunctions.OnWin();
            else
                guiFunctions.OnLoss();

        }
        
        private void Update()
        {
            //todo: game input updates
            timeToNextTurn = (int) (GameSettings.TurnDuration - (PhotonNetwork.Time - lastTurnStartTime));
            turnTimer.UpdateTime((int)timeToNextTurn);
            timeToEndGame = (int) (GameSettings.GameDuration - (PhotonNetwork.Time - gameStartTime));
            gameTimer.UpdateTime((int)timeToEndGame);
            if (timeToNextTurn < 0 && lastTurnStartTime > 0 && gameStartTime > 0)
            {
                NextTurn();
            }

            if (timeToEndGame < 0 && lastTurnStartTime > 0 && gameStartTime > 0 && !isLooser)
            {
                var units = GameObject.FindGameObjectsWithTag("Unit");
                var myLevel = units[0].GetComponent<Unit>().Level;
                var enemyLevel = units[1].GetComponent<Unit>().Level;
                if (units[1].GetComponent<PhotonView>().IsMine)
                {
                    (myLevel, enemyLevel) = (enemyLevel, myLevel);
                }
                isLooser = true;
                if (myLevel > enemyLevel)
                    guiFunctions.OnWin();
                else
                    guiFunctions.OnLoss();
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