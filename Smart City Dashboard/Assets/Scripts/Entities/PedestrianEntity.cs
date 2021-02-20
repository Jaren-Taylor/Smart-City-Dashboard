using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianEntity : Entity
{
    private const string PedestrianPrefabAddress = "Prefabs/Pedestrian/Pedestrian_Base";

    public override bool TrySetDestination(Vector2Int tileLocation) => TrySetDestination(tileLocation, NodeCollectionController.TargetUser.Pedestrians);

    public static PedestrianEntity Spawn(Vector2Int tileLocation) => Spawn<PedestrianEntity>(tileLocation, PedestrianPrefabAddress);
}
