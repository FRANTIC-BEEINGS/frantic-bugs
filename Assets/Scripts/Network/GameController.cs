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
	[SerializeField] private GameStartController _gameStartController;
	[SerializeField] private CountDown _gameTimer;

	[SerializeField] public int GameDuration;
	
	private void Awake()
	{
		_gameStartController.StartGame += StartGame;
	}

	private void StartGame()
	{
		if (IsServer)
		{
			_gameTimer.StartTimer(GameDuration);
		}
	}
}