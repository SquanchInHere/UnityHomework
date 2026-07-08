using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingData", menuName = "Scriptable Objects/BuildingData")]
public class BuildingData : ScriptableObject
{
    [Header("Main")]
    public string Id;
    public string DisplayName;
    public BuildingType Type = BuildingType.TownCenter;
    public Sprite Icon;

    [Header("Prefab")]
    public GameObject Prefab;

    [Header("Stats")]
    [Min(1)] public float MaxHealth = 300f;
    public float Armor = 0f;

    [Header("Stats damage")]
    public bool CanFight = false;
    [Min(0)] public float Damage = 7.5f;

    [Header("Storage")]
    public bool CanStoreResources;
    [Min(0)] public int StorageCapacity = 100;

    [Header("Production")]
    public bool CanProduceUnits;
    public UnitData[] ProducedUnits;

    [Header("Buildin Limits")]
    [Min(0)] public int BuildingCount = 1;
}
