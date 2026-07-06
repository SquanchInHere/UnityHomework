using System;
using UnityEngine;

[Serializable]
public class ResourceAmount
{
    public ResourceType Type;
    [Min(0)] public int Amount;
}
