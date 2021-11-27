
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class NetworkPlayerController : NetworkBehaviour
{
	public NetworkVariable<bool> readyToPlay;
	public NetworkVariable<bool> thisPlayerTurn;
	[SerializeField] private GameController _gameController;

	private void Start()
	{
		_gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
	}

	public void Ready()
	{
		ReadyServerRpc();
	}

	public void EndTurn()
	{
		if (thisPlayerTurn.Value)
		{
			_gameController.EndTurnServerRpc();
		}
	}

	[ServerRpc]
	private void ReadyServerRpc()
	{
		readyToPlay.Value = true;
	}
	
	[ServerRpc(RequireOwnership = false)]
	public void StartTurnServerRpc()
	{
		thisPlayerTurn.Value = true;
	}
	
	[ServerRpc(RequireOwnership = false)]
	public void EndTurnServerRpc()
	{
		thisPlayerTurn.Value = false;
	}
	
}