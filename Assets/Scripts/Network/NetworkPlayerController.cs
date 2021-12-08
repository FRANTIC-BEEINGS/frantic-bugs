
using System;
using System.Collections;
using System.Collections.Generic;
using Cards;
using ResourceManagment;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.PlayerLoop;

public class NetworkPlayerController : NetworkBehaviour
{
	public NetworkVariable<bool> readyToPlay;
	public NetworkVariable<bool> thisPlayerTurn;
	private GameController _gameController;
	private ResourceManager _resourceManager;
	private UnitsMoveController _unitsMoveController;
	public Card lastClickedCard;

	public ResourceManager GetResourceManager()
	{
		return _resourceManager;
	}
	
	public UnitsMoveController GetUnitsMoveController()
	{
		return _unitsMoveController;
	}
	
	void Update()
	{
		if (Input.GetMouseButtonDown((int)MouseButtons.Right))
		{
			if (thisPlayerTurn.Value)
				StopMovement();
		}
		
		if (Input.GetMouseButtonUp((int)MouseButtons.Left))
		{
			if (thisPlayerTurn.Value)
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit rayHit;
				if (Physics.Raycast(ray, out rayHit, 100.0f)) {
					if (rayHit.collider.tag == "Card") {
						lastClickedCard = rayHit.collider.transform.parent.gameObject.GetComponent<Card>();
						Debug.Log(lastClickedCard);
						_gameController.ClickedCard(lastClickedCard);
					}
				}
			}
		}
	}

	public bool ShowInteractionButtons()
	{
		return !_unitsMoveController.UnitIsMoving();
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
			StopMovement();
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
		StopMovement();
	}

	public void StopMovement()
	{
		_unitsMoveController.StopMovement();
	}

	void FinishedMovement()
	{
		_gameController.ClickedCard(lastClickedCard);
	}
	
	public void Initialize(int turnEnergy, PathBuilder pathBuilder)
	{
		_resourceManager = new ResourceManager(turnEnergy);
		_unitsMoveController = new UnitsMoveController(pathBuilder, _resourceManager);
		_unitsMoveController.FinishedMovementAction += FinishedMovement;
	}

}