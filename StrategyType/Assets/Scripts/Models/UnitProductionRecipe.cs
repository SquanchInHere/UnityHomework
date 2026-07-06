using UnityEngine;

[CreateAssetMenu(fileName = "UnitProductionRecipe", menuName = "Scriptable Objects/UnitProductionRecipe")]
public class UnitProductionRecipe : ScriptableObject
{
    public UnitData Unit;
    public ResourceAmount[] Costs;
    public float ProductionTime = 8f;
}
