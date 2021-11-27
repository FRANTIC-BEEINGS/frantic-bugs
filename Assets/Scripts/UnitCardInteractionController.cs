﻿using System;
using System.Text.RegularExpressions;
using Cards;
using ResourceManagment;

public class UnitCardInteractionController
{
    private ResourceManager _resourceManager;
    public void FightEnemyCard(EnemyCard enemyCard, Unit unit)
    {
        if (unit.Force < enemyCard.GetLevel() || _resourceManager.GetResource(ResourceType.Energy) < unit.FightEnergy)
        {
            unit.Death();
        }
        else
        {
            _resourceManager.AddResource(ResourceType.Energy, -unit.FightEnergy);
            unit.IncreaseExperience(enemyCard.GetExpReward());
            foreach (var r in enemyCard.GetResourceReward())
            {
                _resourceManager.AddResource(r.Key, r.Value);
            }
            //todo: add die method for enemyCard
        }
    }

    public void CaptureCard(ICapturable card, ulong captorId, Unit unit)
    {
        if (_resourceManager.GetResource(ResourceType.Energy) < unit.CaptureEnergy)
            return;
        _resourceManager.AddResource(ResourceType.Energy, -unit.CaptureEnergy);
        card.Capture(captorId);
        if (card is ResourceCard)
        {
            _resourceManager.AddReplenishableResource((ResourceCard) card);
        }
    }
    
    public void GetResource(ResourceCard resourceCard, Unit unit)
    {
        if (_resourceManager.GetResource(ResourceType.Energy) < unit.ResourceEnergy)
            return;
        _resourceManager.AddResource(ResourceType.Energy, -unit.ResourceEnergy);
        _resourceManager.AddResource(resourceCard.GetResource(), resourceCard.GetResourceCount());
    }
    
    public bool StepOnCard(Unit unit, Card card)
    {
        return card.StepOn(unit);
    }
}