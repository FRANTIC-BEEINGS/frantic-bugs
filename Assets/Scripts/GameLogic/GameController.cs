using System;
using UnityEngine;

namespace GameLogic
{
    public class GameController : MonoBehaviour
    {
        private void Start()
        {
            if (GameSettings.Multiplayer)
            {
                MultiplayerStart();
            }
            else
            {
                SinglePlayerStart();
            }

        }

        private void SinglePlayerStart()
        {

        }

        private void MultiplayerStart()
        {

        }
    }
}