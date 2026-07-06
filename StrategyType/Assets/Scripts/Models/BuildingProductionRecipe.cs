using UnityEngine;

[CreateAssetMenu(fileName = "BuildingProductionRecipe", menuName = "Scriptable Objects/BuildingProductionRecipe")]
public class BuildingProductionRecipe : ScriptableObject
{
    public BuildingData Building;
    public ResourceAmount[] Costs;
    public float BuildTime;

    public GameObject Prefab => Building != null ? Building.Prefab : null;
}
