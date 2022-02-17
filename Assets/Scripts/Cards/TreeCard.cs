using Photon.Pun;
using UnityEngine;

namespace Cards
{
    public class TreeCard : Card, ICapturable
    {
        [SerializeField] private int _visionRadius;
        public void Capture(ulong captorId)
        {
            if (IsCaptured) return; //todo: remove tmp fix and handle recapturing
            IsCaptured = true;
            CaptorId = captorId;
            //todo: change captor status on each card in radius
        }
        
        //use this in generation for setting default parameters
        public void Initialize(int visionRadius)
        {
            _visionRadius = visionRadius;
        }

        public override string ToString()
        {
            return "Tree | Vision range: " + _visionRadius;
        }
        
        [PunRPC]
        protected virtual void SetIsCaptured(bool value)
        {
            base.SetIsCaptured(value);
        }
        
        [PunRPC]
        protected virtual void SetCurrentUnitId(int value)
        {
            base.SetCurrentUnitId(value);
        }
    }
}