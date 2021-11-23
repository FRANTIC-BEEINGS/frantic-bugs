using System;
using UnityEngine;

public class ResourceCard : Card
{
    private ResourceType _resource;
    private int _quantity;  //initial amount of resource
    private int _replenishmentQuantity; //how much of the resource is added per replenishment
    private int _replenishmentSpeed;    //how many turns must pass before a replenishment occurs
    private int _turnsToNextReplenishment;

    public void Initialize(Sprite face, ResourceType resource, int quantity, int replenishmentQuantity,
        int replenishmentSpeed)
    {
        Face = face;
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
}