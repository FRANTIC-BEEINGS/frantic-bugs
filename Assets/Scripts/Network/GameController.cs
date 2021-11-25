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
	[SerializeField] private GameStartController _gameStartController;
	[SerializeField] private CountDown _gameTimer;

	[SerializeField] public int GameDuration;
	private MapGeneration _mapGeneration;
	
	private void Awake()
	{
		_gameStartController.StartGame += StartGame;
		_mapGeneration = GetComponent<MapGeneration>();
	}

	private void StartGame()
	{
		if (NetworkManager.Singleton.IsServer && !gameStarted)
		{
			gameStarted = true;
			_gameTimer.StartTimer(GameDuration);
			_mapGeneration.GenerateMap();
		}
	}
}