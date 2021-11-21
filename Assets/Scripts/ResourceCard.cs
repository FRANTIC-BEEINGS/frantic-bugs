using System;

public class ResourceCard : Card
{
    private ResourceType _resource;
    private int _quantity;  //initial amount of resource
    private int _replenishmentRate;

    //private Action<ResourceType,int> GatherResource;

}