using System;
using UnityEngine;

[Serializable]
public class ResourceDrop
{
    [SerializeField] private ItemDefinition _item;
    [SerializeField] private int _minAmount = 1;
    [SerializeField] private int _maxAmount = 1;
    [SerializeField, Range(0f, 1f)] private float _chance = 1f;
}
