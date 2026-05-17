using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

public static class ShopDataSeed
{
    public static List<ShopItems> CreateItems(
        Sprite healthPotionIcon,
        Sprite manaPotionIcon,
        Sprite poisonPotionIcon
    )
    {
        return new List<ShopItems>
        {
            new ShopItems
            {
                id = "health_potion",
                Name = "Health Potion",
                Price = 25,
                icon = healthPotionIcon,
                Count = 15,
            },
            new ShopItems
            {
                id = "mana_potion",
                Name = "Mana Potion",
                Price = 30,
                icon = manaPotionIcon,
                Count = 15,
            },
            new ShopItems
            {
                id = "poison_potion",
                Name = "Poison Potion",
                Price = 40,
                icon = poisonPotionIcon,
                Count = 5,
            }
        };
    }
}
