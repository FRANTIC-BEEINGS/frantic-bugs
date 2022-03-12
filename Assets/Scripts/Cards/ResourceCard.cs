using System;
using Photon.Pun;
using ResourceManagment;
using UnityEngine;

namespace Cards
{
    public class ResourceCard : Card, ICapturable
    {
        [SerializeField] private ResourceType _resource;
        [SerializeField] private int _quantity;  //initial amount of resource
        [SerializeField] private int _replenishmentQuantity; //how much of the resource is added per replenishment
        [SerializeField] private int _replenishmentSpeed;    //how many turns must pass before a replenishment occurs
        private int _turnsToNextReplenishment;
        public Action<ResourceCard> Replenish;
        
        private bool _resourceCollected;

        public bool ResourceCollected
        {
            get => _resourceCollected;
            set
            {
                if (!PhotonNetwork.IsMasterClient)
                {
                    photonView.RPC("SetResourceСollected", RpcTarget.MasterClient, value);
                }
                _resourceCollected = value;
            }
        }

        public void Initialize(Sprite face, ResourceType resource, int quantity, int replenishmentQuantity = 0, int replenishmentSpeed = 0)
        {
            base.FaceSprite = face;
            _resource = resource;
            _quantity = quantity;
            _replenishmentQuantity = replenishmentQuantity;
            _replenishmentSpeed = replenishmentSpeed;
            _turnsToNextReplenishment = replenishmentSpeed;
            ResourceCollected = false;
        }

        //use this in generation for setting default parameters
        public void Initialize(ResourceType resource, int quantity, int replenishmentQuantity = 0, int replenishmentSpeed = 0)
        {
            _resource = resource;
            _quantity = quantity;
            _replenishmentQuantity = replenishmentQuantity;
            _replenishmentSpeed = replenishmentSpeed;
            _turnsToNextReplenishment = replenishmentSpeed;
            ResourceCollected = false;
        }

        public ResourceType GetResource()
        {
            ResourceCollected = true;
            return _resource;
        }

        public int GetResourceCount()
        {
            return _quantity;
        }
        
        public void ConsumeResource()
        {
            ResourceCollected = true;
        }

        //must be called every player turn (do not call when it is enemy's turn)
        public bool ReplenishTurnTick()
        {
            _turnsToNextReplenishment--;
            if (_turnsToNextReplenishment > 0)
                return true;
            Replenish?.Invoke(this);
            _turnsToNextReplenishment = _replenishmentSpeed;
            return false;
        }

        public void Capture(ulong captorId)
        {
            if (IsCaptured) return; //todo: remove tmp fix and handle recapturing
            IsCaptured = true;
            CaptorId = captorId;
            //todo: add replenish
        }

        //todo resource visual update

        public override string ToString()
        {
            return _resource.ToString() + " | " + _quantity;
        }

        public string GetCollectButtonText()
        {
            string result = "";

            switch (_resource)
            {
                case ResourceType.Energy:
                    result = "Rest";
                    break;
                case ResourceType.Food:
                    result = "Gather " + _resource.ToString();
                    break;
                case ResourceType.Money:
                    result = "Collect " + _resource.ToString();
                    break;
            }
            return result;
        }
        
        public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo messageInfo)
        {
            base.OnPhotonSerializeView(stream, messageInfo);
            if (stream.IsWriting)
            {
                stream.SendNext(_resourceCollected);
            }
            else if (stream.IsReading)
            {
                _resourceCollected = (bool) stream.ReceiveNext();
            }
        }

        #region RPCs

        [PunRPC]
        protected void SetResourceСollected(bool value)
        {
            _resourceCollected = value;
        }
        
        #endregion
    }
}
