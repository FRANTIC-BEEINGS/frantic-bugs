
using System;
using System.Collections;
using System.Collections.Generic;
using ResourceManagment;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class NetworkPlayerController : NetworkBehaviour
{
	public NetworkVariable<bool> readyToPlay;
	public NetworkVariable<bool> thisPlayerTurn;
	private GameController _gameController;
	private ResourceManager _resourceManager;
	private UnitsMoveController _unitsMoveController;

	public ResourceManager GetResourceManager()
	{
		return _resourceManager;
	}
	
	public UnitsMoveController GetUnitsMoveController()
	{
		return _unitsMoveController;
	}

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
			_unitsMoveController.StopMovement();
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
		_unitsMoveController.StopMovement();
	}

	public void Initialize(int turnEnergy, PathBuilder pathBuilder)
	{
		_resourceManager = new ResourceManager(turnEnergy);
		_unitsMoveController = new UnitsMoveController(pathBuilder, _resourceManager);
	}

}