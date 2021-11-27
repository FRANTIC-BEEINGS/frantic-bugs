using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cards;
using Unity.Netcode;
using UnityEngine.Networking.Match;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class GameController : NetworkBehaviour
{
	private bool gameStarted = false;
	private bool gameOver = false;
	[SerializeField] private GameStartController _gameStartController;
	[SerializeField] private CountDown _gameTimer;
	[SerializeField] private List<CountDown> _turnTimers;

	private int currentTurnPlayer = -1;

	[SerializeField] public int GameDuration;
	[SerializeField] public int TurnDuration;
	
	[SerializeField] private GameObject mapPrefab;
	[SerializeField] private GameObject pathBuilderPrefab;
	[SerializeField] private GameObject unitPrefab;
	private MapGeneration map;
	private PathBuilder path;
	private Unit unit;

	private void Awake()
	{
		_gameStartController.StartGame += StartGame;
		_gameTimer.TimerOver += GameOver;
		foreach (var _turnTimer in _turnTimers)
		{
			_turnTimer.TimerOver += StartNextTurnTimer;
		}
	}

	private void StartGame()
	{
		if (NetworkManager.Singleton.IsServer && !gameStarted)
		{
			gameStarted = true;
			currentTurnPlayer = -1;
			_gameTimer.StartTimer(GameDuration);
			map = Instantiate(mapPrefab).GetComponent<MapGeneration>();
			map.MapGenerated += StartAfterMapGenerated;
		}
	}

	private void StartAfterMapGenerated()
	{
		path = Instantiate(pathBuilderPrefab).GetComponent<PathBuilder>();
		path.Initialize(mapGeneration: map);
		SpawnMainUnits();
		StartNextTurnTimer();
	}

	private void SpawnMainUnits()
	{
		if (NetworkManager.Singleton.ConnectedClients.Count > 0)
		{
			Card card = map.Map[0][map.MapCardWidth/2];
			GameObject u = Instantiate(unitPrefab,card.gameObject.transform.position, Quaternion.identity);
			unit = u.GetComponent<Unit>();
			card.StepOn(unit);
		}

		if (NetworkManager.Singleton.ConnectedClients.Count == 2)
		{
			Card card = map.Map[map.MapCardHeight-1][map.MapCardWidth/2];
			GameObject u = Instantiate(unitPrefab,card.gameObject.transform.position, Quaternion.identity);
			unit = u.GetComponent<Unit>();
			card.StepOn(unit);
		}
	} 

	private void StartNextTurnTimer()
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

		_turnTimers[currentTurnPlayer].StartTimer(TurnDuration);
	}

	private void ChangeCurrentTurnPlayer(int newValue)
	{
		if (currentTurnPlayer >= 0)
		{
			NetworkManager.Singleton.ConnectedClients[(ulong) currentTurnPlayer].PlayerObject
				.GetComponent<NetworkPlayerController>().EndTurnServerRpc();
		}
		currentTurnPlayer = newValue;
		NetworkManager.Singleton.ConnectedClients[(ulong) currentTurnPlayer].PlayerObject
			.GetComponent<NetworkPlayerController>().StartTurnServerRpc();
	}

	private void GameOver()
	{
		gameOver = true;
	}

	[ServerRpc(RequireOwnership = false)]	
	public void EndTurnServerRpc()
	{
		_turnTimers[currentTurnPlayer].StopTimer();
		StartNextTurnTimer();
	}
}
