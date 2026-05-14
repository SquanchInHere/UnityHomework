using System;
using UnityEngine;

[Serializable]
public class ShopItemModel
{
    public ShopItemType Type;

    public int Level;

    public float Price;

    public float PriceMultiplier;

    public float Value;

    public float UnlockPrice;

    public int MaxLvl;
}
