using Cards;
using Photon.Pun;
using ResourceManagment;
using Unity.Netcode;
using UnityEngine;

namespace GameLogic
{
	public class PlayerController : MonoBehaviourPunCallbacks
	{
		public bool thisPlayerTurn;
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
			// if (Input.GetMouseButtonDown((int) MouseButtons.Right))
			// {
			// 	if (thisPlayerTurn)
			// 		StopMovement();
			// }
			//
			// if (Input.GetMouseButtonUp((int) MouseButtons.Left))
			// {
			// 	if (thisPlayerTurn)
			// 	{
			// 		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			// 		RaycastHit rayHit;
			// 		if (Physics.Raycast(ray, out rayHit, 100.0f))
			// 		{
			// 			if (rayHit.collider.tag == "Card")
			// 			{
			// 				lastClickedCard = rayHit.collider.transform.parent.gameObject.GetComponent<Card>();
			// 				_gameController.ClickedCard(lastClickedCard);
			// 			}
			// 		}
			// 	}
			// }
		}

		public bool ShowInteractionButtons()
		{
			return !_unitsMoveController.UnitIsMoving();
		}

		private void Start()
		{
			thisPlayerTurn = true;
			_resourceManager = new ResourceManager(GameSettings.TurnEnergy);
			GameObject pathBuilderGO = GameObject.FindWithTag("PathBuilder");
			Debug.Log("pathBuilderGO = " + pathBuilderGO);
			PathBuilder pathBuilder = pathBuilderGO.GetComponent<PathBuilder>();
			_unitsMoveController = new UnitsMoveController(pathBuilder, _resourceManager);

			_unitsMoveController.FinishedMovementAction += FinishedMovement;
			// _gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
		}

		public void EndTurn()
		{
			// if (thisPlayerTurn)
			// {
			// 	_gameController.EndTurnServerRpc();
			// 	StopMovement();
			// }
		}
		

		// [ServerRpc(RequireOwnership = false)]
		// public void StartTurnServerRpc()
		// {
		// 	thisPlayerTurn.Value = true;
		// }
		//
		// [ServerRpc(RequireOwnership = false)]
		// public void EndTurnServerRpc()
		// {
		// 	thisPlayerTurn.Value = false;
		// 	StopMovement();
		// }

		public void StopMovement()
		{
			_unitsMoveController.StopMovement();
		}

		void FinishedMovement()
		{
			// _gameController.ClickedCard(lastClickedCard);
		}

		// public void Initialize(int turnEnergy, PathBuilder pathBuilder)
		// {
		// 	_resourceManager = new ResourceManager(turnEnergy);
		// 	_unitsMoveController = new UnitsMoveController(pathBuilder, _resourceManager);
		// 	_unitsMoveController.FinishedMovementAction += FinishedMovement;
		// }

	}
}