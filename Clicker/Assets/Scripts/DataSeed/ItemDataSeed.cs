using System.Collections.Generic;

public class ItemDataSeed
{
    public static List<ShopItemModel> GetItems()
    {
        return new List<ShopItemModel>
        {
            new ShopItemModel
            {
                Type = ShopItemType.ClickPower,
                Level = 0,
                Price = 10f,
                PriceMultiplier = 1.4f,
                Value = 1f,
                UnlockPrice = 0f,
                MaxLvl = 30,
            },

            new ShopItemModel
            {
                Type = ShopItemType.AutoIncome,
                Level = 0,
                Price = 50f,
                PriceMultiplier = 1.2f,
                Value = 0.3f,
                UnlockPrice = 100f,
                MaxLvl = 20,
            },

            new ShopItemModel
            {
                Type = ShopItemType.SuperClick,
                Level = 0,
                Price = 150f,
                PriceMultiplier = 1.7f,
                Value = 5f,
                UnlockPrice = 300f,
                MaxLvl = 15,
            }
        };
    }
}
