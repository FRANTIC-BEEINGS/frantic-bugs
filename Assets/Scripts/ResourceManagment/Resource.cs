using System;
using ResourceManagment;

public struct Resource : IEquatable<Resource>
{
    public ResourceType ResourceType;
    public int Amount;

    public Resource(ResourceType resourceType, int amount)
    {
        ResourceType = resourceType;
        Amount = amount;
    }
    
    public bool Equals(Resource other)
    {
        return this.ResourceType == other.ResourceType && this.Amount == other.Amount;
    }
}