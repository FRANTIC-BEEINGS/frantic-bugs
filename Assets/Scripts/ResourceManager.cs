using System;
using System.Collections.Generic;
public class ResourceManager
{
    private Dictionary<ResourceType, int> resources;
    public Action<ResourceCard> GlobalReplenish;

    public ResourceManager()
    {
        resources = new Dictionary<ResourceType, int>();
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
    }

    //adds replenishment func (must be called when captured resource card)
    public void AddReplenishableResource()
    {
        //todo: write test function
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