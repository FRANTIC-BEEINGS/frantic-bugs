using System.Collections.Generic;
using UnityEngine;
using Cards;
using ResourceManagment;
using Unity.Netcode;
using UnityEngine.UI;

public class GameController : NetworkBehaviour
{
	private const int UnitPositionZ = 0;
	private bool gameStarted = false;
	private bool gameOver = false;
	[SerializeField] private GameStartController _gameStartController;
	[SerializeField] private CountDown _gameTimer;
	[SerializeField] private List<CountDown> _turnTimers;

	private int currentTurnPlayer = -1;
	private List<ResourceManager> _resourceManagers;
	private List<NetworkPlayerController> _networkPlayerControllers;

	[SerializeField] public int GameDuration;
	[SerializeField] public int TurnDuration;
	[SerializeField] public int TurnEnergy;

	[SerializeField] GameObject camera;
	CameraController cameraController;

	[SerializeField] private GameObject mapPrefab;
	[SerializeField] private GameObject pathBuilderPrefab;
	[SerializeField] private GameObject unitPrefab;
	private MapGeneration map;
	private PathBuilder pathBuilder;
	private Unit unit;

	[SerializeField] private GUIController guiController;

	[SerializeField] private Button captureButton;
	[SerializeField] private Button getResourceButton;
	[SerializeField] private Button readyButton;

	[SerializeField] private int foodToWin;
	[SerializeField] private int moneyToWin;

	public PathBuilder GetPathBuilder()
	{
		return pathBuilder;
	}

	private void Awake()
	{
		_gameStartController.StartGame += StartGame;
		_gameTimer.TimerOver += GameOver;
		foreach (var _turnTimer in _turnTimers)
		{
			_turnTimer.TimerOver += StartNextTurn;
		}
	}

	// After all players ready
	private void StartGame()
	{
		if (NetworkManager.Singleton.IsServer && !gameStarted)
		{
			guiController.OnGameStarted();
			gameStarted = true;
			map = Instantiate(mapPrefab).GetComponent<MapGeneration>();

			map.MapGenerated += StartAfterMapGenerated;
		}
	}

	private void StartAfterMapGenerated()
	{
		cameraController = camera.GetComponent<CameraController>();
		cameraController.SetViewAtCoords(map.GetSpawnCoords());
		//cameraController.SetViewAtCoords(map.GetFirstSpawnCoords());
		//cameraController.SetViewAtCoords(map.GetSecondSpawnCoords());
		cameraController.SetViewBorders(map.GetMapUnityWidth(), map.GetMapUnityHeight());

		currentTurnPlayer = -1;
		_gameTimer.StartTimer(GameDuration);
		SetNetworkPlayers();
		InstantiateLocalObjects();
		foreach (var player in _networkPlayerControllers)
		{
			player.Initialize(TurnEnergy, pathBuilder);
			player.GetResourceManager().OnResourceChange = guiController.UpdateResourceDisplay;
			player.GetResourceManager().OnResourceChange += CheckWinCondition;
			guiController.SetGameGoals(foodToWin,moneyToWin);
		}
		SpawnMainUnits();
		StartNextTurn();
		// todo: show end turn button
	}

	private void SetNetworkPlayers()
	{
		_networkPlayerControllers = new List<NetworkPlayerController>();
		foreach (var player in NetworkManager.Singleton.ConnectedClients.Values)
		{
			var playerObject = player.PlayerObject;
			_networkPlayerControllers.Add(playerObject.GetComponent<NetworkPlayerController>());
		}
	}

	//client rpc
	private void InstantiateLocalObjects()
	{
		pathBuilder = Instantiate(pathBuilderPrefab).GetComponent<PathBuilder>();
		pathBuilder.Initialize(mapGeneration: map);
	}

	private void SpawnMainUnits()
	{
		if (NetworkManager.Singleton.ConnectedClients.Count > 0)
		{
			Card card = map.GetMap()[0][map.GetMapCardWidth()/2];

			Vector3 cardPosition = card.gameObject.transform.position;
			GameObject u = Instantiate(unitPrefab,
			  new Vector3(cardPosition.x, cardPosition.y, UnitPositionZ),
				Quaternion.identity);
			unit = u.GetComponent<Unit>();
			unit.OnDeath += Death;
			unit.OnLevelChange += ChangeLevelUI;
			UnitCardInteractionController.StepOnCard(unit, card);

		}

		if (NetworkManager.Singleton.ConnectedClients.Count == 2)
		{
			Card card = map.GetMap()[map.GetMapCardHeight()-1][map.GetMapCardWidth()/2];

			Vector3 cardPosition = card.gameObject.transform.position;
			GameObject u = Instantiate(unitPrefab,
				new Vector3(cardPosition.x, cardPosition.y, UnitPositionZ),
					Quaternion.identity);
			unit = u.GetComponent<Unit>();
			UnitCardInteractionController.StepOnCard(unit, card);
		}
	}

	private void StartNextTurn()
	{
		if (gameOver || !IsServer)
			return;
		if (currentTurnPlayer < 0)
		{
			ChangeCurrentTurnPlayer(UnityEngine.Random.Range(0, NetworkManager.Singleton.ConnectedClients.Count - 1));
		}
		else
		{
			ChangeCurrentTurnPlayer((currentTurnPlayer + 1) % NetworkManager.Singleton.ConnectedClients.Count);
		}

		_networkPlayerControllers[currentTurnPlayer].GetResourceManager().ReplenishEnergy();
		_turnTimers[currentTurnPlayer].StartTimer(TurnDuration);
	}

	private void ChangeCurrentTurnPlayer(int newValue)
	{
		if (currentTurnPlayer >= 0)
		{
			_networkPlayerControllers[currentTurnPlayer].EndTurnServerRpc();
		}
		currentTurnPlayer = newValue;
		_networkPlayerControllers[currentTurnPlayer].StartTurnServerRpc();
	}

	private void GameOver()
	{
		gameOver = true;
		Death();
	}

	[ServerRpc(RequireOwnership = false)]
	public void EndTurnServerRpc()
	{
		_turnTimers[currentTurnPlayer].StopTimer();
		StartNextTurn();
	}

	public void ClickedCard(Card card)
	{
		if (UnitCardInteractionController.CanGetResource(card, unit))
		{
			getResourceButton.gameObject.SetActive(true);
			if (UnitCardInteractionController.HaveEnoughResourceToGetResourceCard(card,
				_networkPlayerControllers[currentTurnPlayer].GetResourceManager(), unit))
			{
				getResourceButton.interactable = true;
			}
			else
			{
				getResourceButton.interactable = true;
			}

		}
		else
		{
			getResourceButton.gameObject.SetActive(false);
		}

		// if (UnitCardInteractionController.CanCaptureCard(card, unit))
		// {
		// 	captureButton.gameObject.SetActive(true);
		// 	if (UnitCardInteractionController.HaveEnoughResourceToCaptureCard(card,
		// 		_networkPlayerControllers[currentTurnPlayer].GetResourceManager(), unit))
		// 	{
		// 		captureButton.interactable = true;
		// 	}
		// 	else
		// 	{
		// 		captureButton.interactable = true;
		// 	}
		//
		// }
		// else
		// {
		// 	captureButton.gameObject.SetActive(false);
		// }
	}
	public void CaptureCard()
	{
		UnitCardInteractionController.CaptureCard(
			_networkPlayerControllers[currentTurnPlayer].lastClickedCard as ICapturable,
			(ulong)currentTurnPlayer, unit, _networkPlayerControllers[currentTurnPlayer].GetResourceManager());
		ClickedCard(_networkPlayerControllers[currentTurnPlayer].lastClickedCard);

	}

	public void GetResource()
	{
		UnitCardInteractionController.GetResource(
			(ResourceCard)_networkPlayerControllers[currentTurnPlayer].lastClickedCard, unit,
			_networkPlayerControllers[currentTurnPlayer].GetResourceManager()
			);
		ClickedCard(_networkPlayerControllers[currentTurnPlayer].lastClickedCard);
	}

	private void CheckWinCondition(Resource resource)
	{
		switch (resource.ResourceType)
		{
			case ResourceType.Food:
				if (resource.Amount >= foodToWin)
				{
					foodToWin = 0;
					if (moneyToWin == 0)
						guiController.OnWin();
				}
				break;
			case ResourceType.Money:
				if (resource.Amount >= moneyToWin)
				{
					moneyToWin = 0;
					if (foodToWin == 0)
						guiController.OnWin();
				}
				break;
		}
	}

	private void Death()
	{
		guiController.OnLoss();
	}

	private void ChangeLevelUI(int level)
	{
		guiController.UpdateLevelDisplay(level);
	}

}
