using System;
using Cards;
using ResourceManagment;

public class UnitCardInteractionController
{
    private ResourceManager _resourceManager;
    public void FightEnemyCard(EnemyCard enemyCard)
    {
        
    }

    public void CaptureCard(Card card)  //поменять на тип интерфейса позже (или что-то такое надо подумать)
    {
    }
    
    public void GetResource(ResourceCard resourceCard)
    {
        
    }
    
    public bool StepOnCard(Unit unit, Card card)
    {
        return true;
    }
}