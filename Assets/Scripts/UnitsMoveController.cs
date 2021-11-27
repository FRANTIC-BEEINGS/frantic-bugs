using System.Collections.Generic;
using Cards;
using ResourceManagment;
using UnityEngine;

public class UnitsMoveController : MonoBehaviour
{
    [SerializeField] private PathBuilder PathBuilder;
    private ResourceManager _resourceManager;
    private void Start()
    {
        PathBuilder.pathBuilt += Move;
    }

    public void Move(List<Card> path)
    {
        path[0].GetCurrentUnit().MoveAlongPath(path, _resourceManager.GetResource(ResourceType.Energy));
    }

    public bool CanBuildPath
    {
        get => PathBuilder.CanBuild;
        set => PathBuilder.CanBuild = value;
    }
}