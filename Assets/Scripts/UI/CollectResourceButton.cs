using UnityEngine;

namespace UI
{
    public class CollectResourceButton : MonoBehaviour
    {
        [SerializeField] private GameLogic.GameController _gameController;

        public void GetResource()
        {
            _gameController.GetResource();
        }
    }
}