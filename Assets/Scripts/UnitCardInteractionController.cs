using System;
using System.Text.RegularExpressions;
using Cards;
using ResourceManagment;

public static class UnitCardInteractionController
{
    public static void FightEnemyCard(EnemyCard enemyCard, Unit unit, ResourceManager resourceManager)
    {
        if (unit.Force < enemyCard.GetLevel() || resourceManager.GetResource(ResourceType.Energy) < unit.FightEnergy)
        {
            unit.Death();
        }
        else
        {
            resourceManager.AddResource(ResourceType.Energy, -unit.FightEnergy);
            unit.IncreaseExperience(enemyCard.GetExpReward());
            foreach (var r in enemyCard.GetResourceReward())
            {
                resourceManager.AddResource(r.Key, r.Value);
            }
            //todo: add die method for enemyCard
        }
    }

    public static bool CanCaptureCard(Card card, Unit unit)
    {
        return card is ICapturable && card.GetCurrentUnit() == unit;
    }

    public static bool HaveEnoughResourceToCaptureCard(Card card, ResourceManager resourceManager, Unit unit)
    {
        return resourceManager.GetResource(ResourceType.Energy) >= unit.CaptureEnergy;
    }
    
    public static bool CanGetResource(Card card, Unit unit)
    {
        return card is ResourceCard && card.GetCurrentUnit() == unit;
    }

    public static bool HaveEnoughResourceToGetResourceCard(Card card, ResourceManager resourceManager, Unit unit)
    {
        return resourceManager.GetResource(ResourceType.Energy) >= unit.ResourceEnergy;
    }
    
    public static void CaptureCard(ICapturable card, ulong captorId, Unit unit, ResourceManager resourceManager)
    {
        if (resourceManager.GetResource(ResourceType.Energy) < unit.CaptureEnergy)
            return;
        resourceManager.AddResource(ResourceType.Energy, -unit.CaptureEnergy);
        card.Capture(captorId);
        if (card is ResourceCard)
        {
            resourceManager.AddReplenishableResource((ResourceCard) card);
        }
    }
    
    public static void GetResource(ResourceCard resourceCard, Unit unit, ResourceManager resourceManager)
    {
        if (resourceManager.GetResource(ResourceType.Energy) < unit.ResourceEnergy)
            return;
        resourceManager.AddResource(ResourceType.Energy, -unit.ResourceEnergy);
        resourceManager.AddResource(resourceCard.GetResource(), resourceCard.GetResourceCount());
    }
    
    public static bool StepOnCard(Unit unit, Card card)
    {
        return card.StepOn(unit);
    }
}