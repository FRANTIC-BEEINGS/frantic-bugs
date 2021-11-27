using System;
using System.Text.RegularExpressions;
using Cards;
using ResourceManagment;

public class UnitCardInteractionController
{
    private ResourceManager _resourceManager;
    public void FightEnemyCard(EnemyCard enemyCard)
    {
        
    }

    public void CaptureCard(ICapturable card, ulong captorId)
    {
        card.Capture(captorId);
        if (card is ResourceCard)
        {
            _resourceManager.AddReplenishableResource((ResourceCard) card);
        }
    }
    
    public void GetResource(ResourceCard resourceCard)
    {
        
    }
    
    public bool StepOnCard(Unit unit, Card card)
    {
        return true;
    }
}