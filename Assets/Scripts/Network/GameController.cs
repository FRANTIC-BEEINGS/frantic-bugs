using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
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
	
	[SerializeField] private GameObject MapPrefab;
	private GameObject Map;

	private void Awake()
	{
		_gameStartController.StartGame += StartGame;
		_gameTimer.TimerOver += GameOver;
		foreach (var _turnTimer in _turnTimers)
		{
			_turnTimer.TimerOver += StartTurnTimer;
		}
	}

	private void StartGame()
	{
		if (NetworkManager.Singleton.IsServer && !gameStarted)
		{
			gameStarted = true;
			currentTurnPlayer = -1;
			_gameTimer.StartTimer(GameDuration);
			Map = Instantiate(MapPrefab);
			StartTurnTimer();
		}
	}

	private void StartTurnTimer()
	{
		if (gameOver)
			return;
		if (currentTurnPlayer < 0)
		{
			currentTurnPlayer = UnityEngine.Random.Range(0, NetworkManager.Singleton.ConnectedClients.Count - 1);
		}
		else
		{
			currentTurnPlayer = (currentTurnPlayer + 1) % NetworkManager.Singleton.ConnectedClients.Count;
		}
		_turnTimers[currentTurnPlayer].StartTimer(TurnDuration);
	}

	private void GameOver()
	{
		gameOver = true;
	}
}
