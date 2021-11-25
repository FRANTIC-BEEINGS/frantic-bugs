using System;
using System.Text.RegularExpressions;
using Cards;
using ResourceManagment;

public class UnitCardInteractionController
{
    private ResourceManager _resourceManager;
    public void FightEnemyCard(EnemyCard enemyCard, Unit unit)
    {
        // level or force? add get level or force from enemyCard
        if (/*unit.Force < enemyCard.level ||*/ _resourceManager.GetEnergy() < unit.FightEnergy)
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
            //add die method for enemyCard
        }
    }

    public void CaptureCard(ICapturable card, ulong captorId, Unit unit)
    {
        if (_resourceManager.GetEnergy() < unit.CaptureEnergy)
            return;
        _resourceManager.AddResource(ResourceType.Energy, -unit.CaptureEnergy);
        card.Capture(captorId);
    }
    
    public void GetResource(ResourceCard resourceCard, Unit unit)
    {
        if (_resourceManager.GetEnergy() < unit.ResourceEnergy)
            return;
        _resourceManager.AddResource(ResourceType.Energy, -unit.ResourceEnergy);
        _resourceManager.AddResource(resourceCard.GetResource(), resourceCard.GetResourceCount());
        //how is working replenishment?
    }
    
    public bool StepOnCard(Unit unit, Card card)
    {
        return card.StepOn(unit);
    }
}