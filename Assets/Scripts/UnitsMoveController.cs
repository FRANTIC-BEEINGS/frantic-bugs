using System.Collections.Generic;
using Cards;
using ResourceManagment;
using UnityEngine;

public class UnitsMoveController
{
    private PathBuilder pathBuilder;
    private ResourceManager resourceManager;
    private Unit currentMovingUnit;

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
        //не смотрите на эти две строки
        currentMovingUnit.FinishedMovement -= FinishedMovement;
        currentMovingUnit.FinishedMovement += FinishedMovement;
        currentMovingUnit.MoveAlongPath(path, resourceManager);
    }

    private void FinishedMovement()
    {
        pathBuilder.CanBuild = true;
    }

    public bool UnitIsMoving()
    {
        return !pathBuilder.CanBuild;
    }

    public void StopMovement()
    {
        currentMovingUnit.stopMovement();
    }
}