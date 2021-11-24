
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

	public void Ready()
	{
		ReadyServerRpc();
	}

	[ServerRpc]
	private void ReadyServerRpc()
	{
		readyToPlay.Value = true;
	}
	
	
}