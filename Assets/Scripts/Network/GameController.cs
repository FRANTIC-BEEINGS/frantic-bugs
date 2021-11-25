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
	[SerializeField] private GameObject MapPrefab;
	private GameObject Map;

	private void Awake()
	{
		_gameStartController.StartGame += StartGame;
	}

	private void StartGame()
	{
		if (NetworkManager.Singleton.IsServer && !gameStarted)
		{
			gameStarted = true;
			_gameTimer.StartTimer(GameDuration);
			Map = Instantiate(MapPrefab);
		}
	}
}
