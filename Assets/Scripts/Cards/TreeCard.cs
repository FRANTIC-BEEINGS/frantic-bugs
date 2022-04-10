using Photon.Pun;
using UnityEngine;

namespace Cards
{
    public class TreeCard : Card, ICapturable
    {
        [SerializeField] private int _visionRadius;
        private VisionController visionController;
        public void Capture(ulong captorId)
        {
            if (IsCaptured) return; //todo: remove tmp fix and handle recapturing
            IsCaptured = true;
            CaptorId = captorId;
            //todo: change captor status on each card in radius
        }
        
        //use this in generation for setting default parameters
        public void Initialize(int visionRadius, MapGeneration mapGeneration)
        {
            _visionRadius = visionRadius;
            visionController = GetComponent<VisionController>();
            visionController.Initialize(mapGeneration);
            visionController.OpenCardsInTreeVision(_visionRadius, this);
        }

        public override string ToString()
        {
            return "Tree | Vision range: " + _visionRadius;
        }
    }
}