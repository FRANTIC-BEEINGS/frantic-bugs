using System;
using System.Collections.Generic;
using Cards;
using UnityEngine;

namespace ResourceManagment
{
    public class ResourceManager
    {
        private List<ResourceCard> _replenishableResources;
        private Dictionary<ResourceType, int> resources;
        private int _maxEnergy;
        private int _extraEnergy;
        
        public Action<ResourceCard> ReplenishResource;
        
        public ResourceManager()
        {
            resources = new Dictionary<ResourceType, int>();
        }
        
        public ResourceManager(int maxEnergy)
        {
            resources = new Dictionary<ResourceType, int>();
            _maxEnergy = maxEnergy;
        }

        public int GetResource(ResourceType resourceType)
        {
            return resources.ContainsKey(resourceType) ? resources[resourceType] : 0;
        }

        public void ReplenishEnergy()
        {
            if (resources.ContainsKey(ResourceType.Energy))
            {
                resources[ResourceType.Energy] = _maxEnergy;
            }
            else
            {
                resources.Add(ResourceType.Energy,_maxEnergy);
            }
        }
        
        //returns false when attempted to detract more of the resource than was available
        public bool AddResource(ResourceType resource, int quantity)
        {
            if (resources.ContainsKey(resource))
            {
                resources[resource] += quantity;
            }
            else
            {
                resources.Add(resource,quantity);
            }

            if (resource == ResourceType.Energy)
                _extraEnergy = resources[resource] > _maxEnergy ? resources[resource] - _maxEnergy : 0;

            if (resources[resource] < 0)
            {
                resources[resource] = 0;
                return false;
            }
            
            return true;
        }

        //adds replenishment func (must be called when captured resource card)
        public void AddReplenishableResource(ResourceCard resourceCard)
        {
            resourceCard.Replenish = Replenish;
        }

        public void RemoveReplenishableResource(ResourceCard resourceCard)
        {
            resourceCard.Replenish = null;
        }

        private void Replenish(ResourceCard resourceCard)
        {
            AddResource(resourceCard.GetResource(),resourceCard.GetResourceCount());
        }
    }
}