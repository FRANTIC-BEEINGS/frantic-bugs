using System;
using System.Collections.Generic;
using Cards;
using UnityEngine;

namespace ResourceManagment
{
    public class ResourceManager
    {
        private Dictionary<ResourceType, int> resources;
        private int _maxEnergy;
        private int _extraEnergy;
        
        public Action<ResourceCard> GlobalReplenish;
        
        public ResourceManager()
        {
            resources = new Dictionary<ResourceType, int>();
        }
        
        public ResourceManager(int maxEnergy)
        {
            resources = new Dictionary<ResourceType, int>();
            _maxEnergy = maxEnergy;
        }

        public int GetEnergy()
        {
            return resources.ContainsKey(ResourceType.Energy) ? resources[ResourceType.Energy] : 0;
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
        
        public void AddResource(ResourceType resource, int quantity)
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
        }

        //adds replenishment func (must be called when captured resource card)
        public void AddReplenishableResource()
        {
            //todo: fix this
            GlobalReplenish += Replenish;
        }

        public void RemoveReplenishableResource()
        {
            GlobalReplenish -= Replenish;
        }

        private void Replenish(ResourceCard resourceCard)
        {
            bool replenishNow = resourceCard.ReplenishTick();
            if (!replenishNow)
                return;
            AddResource(resourceCard.GetResource(),resourceCard.GetReplenishResourceCount());
        }
    }
}