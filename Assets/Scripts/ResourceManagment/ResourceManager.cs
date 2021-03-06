using System;
using System.Collections.Generic;
using Cards;
using UnityEngine;

namespace ResourceManagment
{
    public class ResourceManager
    {
        public const int FoodNeededForLevelUp = 100;
        public const int MoneyNeededForLevelUp = 50;
        
        private List<ResourceCard> _replenishableResources;
        private int _maxEnergy;
        private int _extraEnergy;
        private List<Resource> resources;
        
        public Action<ResourceCard> ReplenishResource;
        public Action<Resource> OnResourceChange;
        
        public ResourceManager(int maxEnergy)
        {
            InitializeResources();
            _maxEnergy = maxEnergy;
            ReplenishEnergy();
        }

        private void InitializeResources()
        {
            resources = new List<Resource>();
            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
                resources.Add(new Resource(type, 0));
        }

        public int GetResource(ResourceType resourceType)
        {
            foreach (var resource in resources)
            {
                if (resource.ResourceType == resourceType)
                    return resource.Amount;
            }

            return 0;
        }

        public void ConsumeResource(ResourceType resourceType, int resourceAmount)
        {
            for (int i = 0; i < resources.Count; ++i)
            {
                if (resources[i].ResourceType == resourceType)
                {
                    var tmp = resources[i];
                    tmp.Amount = resources[i].Amount - resourceAmount;
                    resources[i] = tmp;
                    OnResourceChange?.Invoke(resources[i]);
                }
            }
        }
        
        public void ReplenishEnergy()
        {
            for (int i = 0; i < resources.Count; ++i)
            {
                if (resources[i].ResourceType == ResourceType.Energy)
                {
                    //через resources[i].Amount = _maxEnergy не работает
                    var tmp = resources[i];
                    tmp.Amount = _maxEnergy;
                    resources[i] = tmp;
                    OnResourceChange?.Invoke(resources[i]);
                }
            }
        }
        
        public void AddResource(ResourceType resource, int quantity)
        {
            for (int i = 0; i < resources.Count; ++i)
            {
                if (resources[i].ResourceType == resource)
                {
                    //через resources[i].Amount = _maxEnergy не работает
                    var tmp = resources[i];
                    tmp.Amount = Math.Max(tmp.Amount + quantity, 0);
                    resources[i] = tmp;
                    OnResourceChange?.Invoke(resources[i]);
                    if (resource == ResourceType.Energy)
                    {
                        _extraEnergy = resources[i].Amount > _maxEnergy ? resources[i].Amount - _maxEnergy : 0;
                    }
                }
            }
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