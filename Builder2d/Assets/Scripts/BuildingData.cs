using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingData", menuName = "Build System/Building Data")]
public class BuildingData : ScriptableObject
{
    [Header("Identification")]
    public string id;
    public string displayName;

    [Tooltip("Icon for UI button")]
    public Sprite icon;

    [Tooltip("World preview sprite while dragging. If empty, icon will be used")]
    public Sprite worldSprite;

    [Header("World Prefab")]
    [Tooltip("Prefab that will be spawned in the slot")]
    public GameObject worldPrefab;

    [Header("Slot Compatibility")]
    [Tooltip("Slot accepts item only if acceptedCategory matches this value. Empty = any slot")]
    public string category = "";

    [Header("Build Cost")]
    [Tooltip("Resources required to build this item")]
    public List<ResourceCost> costs = new();

    [Header("Demolish")]
    [Tooltip("Part of the cost returned on demolish. 0 = nothing, 0.5 = half, 1 = full")]
    [Range(0f, 1f)]
    public float refundFraction = 0.5f;
}
