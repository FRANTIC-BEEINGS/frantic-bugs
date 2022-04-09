using System;
using System.Collections.Generic;
using Cards;
using ResourceManagment;
using UnityEngine;

public class UnitsMoveController
{
    private PathBuilder pathBuilder;
    private ResourceManager resourceManager;
    private Unit currentMovingUnit;
    public Action<Card> FinishedMovementAction;

    public UnitsMoveController(PathBuilder pathBuilder, ResourceManager resourceManager)
    {
        this.pathBuilder = pathBuilder;
        pathBuilder.pathBuilt += Move;
        this.resourceManager = resourceManager;
    }

    public void Move(List<Card> path)
    {
        pathBuilder.CanBuild = false;
        currentMovingUnit = path[0].GetCurrentUnit();
        if (currentMovingUnit == null)
            return;
        
        currentMovingUnit.Initialize(pathBuilder.Map);
        //не смотрите на эти две строки
        currentMovingUnit.FinishedMovement -= FinishedMovement;
        currentMovingUnit.FinishedMovement += FinishedMovement;
        currentMovingUnit.FightEnemy -= FightEnemy;
        currentMovingUnit.FightEnemy += FightEnemy;
        currentMovingUnit.MoveAlongPath(path, resourceManager);
    }

    private void FinishedMovement(Card card)
    {
        pathBuilder.CanBuild = true;
        FinishedMovementAction?.Invoke(card);
    }

    public bool UnitIsMoving()
    {
        return !pathBuilder.CanBuild;
    }

    public void StopMovement()
    {
        if (currentMovingUnit != null)
            currentMovingUnit.stopMovement();
    }

    private void FightEnemy(EnemyCard card)
    {
        UnitCardInteractionController.FightEnemyCard(card, currentMovingUnit, resourceManager);
    }
}