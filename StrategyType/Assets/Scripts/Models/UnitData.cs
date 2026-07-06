using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable Objects/UnitData")]
public class UnitData : ScriptableObject
{
    [Header("Main")]
    public string Id;
    public string DisplayName;
    public UnitType Type = UnitType.Worker;
    public Sprite Icon;

    [Header("Prefab")]
    public GameObject Prefab;

    [Header("Stats")]
    [Min(1)] public float MaxHealth = 100f;
    [Min(0)] public float MoveSpeed = 5f;

    public float Damage = 10f;
    public float Armor = 0f;

    [Header("Combat")]
    [Min(0)] public float AttackRange = 1.5f;
    [Min(0.05f)] public float AttackCooldown = 1f;

    [Header("Gathering")]
    [Min(0)] public int CarryCapacity = 10;
    [Min(0)] public float GatherRate = 3f;
}
