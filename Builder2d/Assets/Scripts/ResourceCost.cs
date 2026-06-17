using UnityEngine;

[System.Serializable]
public class ResourceCost
{
    public ResourceType type;

    [Min(0)]
    public int amount;
}
