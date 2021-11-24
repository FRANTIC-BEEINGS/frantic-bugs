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

        public void Initialize(Sprite face, ResourceType resource, int quantity, int replenishmentQuantity,
            int replenishmentSpeed)
        {
            base.FaceSprite = face;
            _resource = resource;
            _quantity = quantity;
            _replenishmentQuantity = replenishmentQuantity;
            _replenishmentSpeed = replenishmentSpeed;
            _turnsToNextReplenishment = replenishmentSpeed;
        }

        public ResourceType GetResource()
        {
            return _resource;
        }

        public int GetResourceCount()
        {
            return _quantity;
        }
    
        public int GetReplenishResourceCount()
        {
            return _replenishmentQuantity;
        }

        public bool ReplenishTick()
        {
            _turnsToNextReplenishment--;
            if (_turnsToNextReplenishment > 0)
                return true;

            _turnsToNextReplenishment = _replenishmentSpeed;
            return false;
        }

        public void Capture(ulong captorId)
        {
            if (IsCaptured) return; //todo: remove tmp fix and handle recapturing
            IsCaptured = true;
            CaptorId = captorId;
        }
    }
}