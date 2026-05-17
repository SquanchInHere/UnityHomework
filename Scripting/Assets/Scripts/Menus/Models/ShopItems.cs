
using System;
using UnityEngine;

[Serializable]
public class ShopItems
{
    public string id;
    public ShopItemType Type;
    public string Name;
    public Sprite icon;

    public int Price;
    public int Count;
}
