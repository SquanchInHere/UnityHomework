using UnityEngine;

[CreateAssetMenu(fileName = "ResourceData", menuName = "Scriptable Objects/ResourceData")]
public class ResourceData : ScriptableObject
{
    [Header("Main")]
    public ResourceType Type = ResourceType.Gold;
    public string DisplayName;
    public Sprite Icon;

    [Header("Gameplay")]
    [Min(1)] public int DefaultAmount = 100;
}
