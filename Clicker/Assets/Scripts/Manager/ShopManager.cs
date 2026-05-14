using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [Header("Wallet Manager")]
    [SerializeField] private Wallet _wallet;

    [Header("Shop Objects")]
    [SerializeField] private Transform _itemsParent;
    [SerializeField] private ShopItemView _itemPrefab;

    [Header("Shop Items list")]
    private readonly List<ShopItemModel> _items = new List<ShopItemModel>();
    private readonly List<ShopItemView> _views = new List<ShopItemView>();

    private void Start()
    {
        CreateShop();
    }

    private void Update()
    {
        RefreshShop();
    }

    private void CreateShop()
    {
        _items.Clear();
        _views.Clear();

        _items.AddRange(ItemDataSeed.GetItems());

        foreach (ShopItemModel item in _items)
        {
            ShopItemView view = Instantiate(_itemPrefab, _itemsParent);
            view.Init(item, this);

            _views.Add(view);
        }

        RefreshShop();
    }

    public void BuyItem(ShopItemModel item)
    {
        Debug.Log(item.Level <= item.MaxLvl);
        if (!IsUnlocked(item))
        {
            Debug.Log("Item is locked");
            return;
        }

        if (!_wallet.TrySpend(item.Price))
        {
            Debug.Log("Not enough coins");
            return;
        }

        item.Level++;

        ApplyItemEffect(item);

        item.Price *= item.PriceMultiplier;

        RefreshShop();
    }

    public bool CanBuy(ShopItemModel item)
    {
        return IsUnlocked(item) && _wallet.Coins >= item.Price && item.Level < item.MaxLvl;
    }

    public bool IsUnlocked(ShopItemModel item)
    {
        return _wallet.Coins >= item.UnlockPrice || item.Level > 0;
    }

    private void ApplyItemEffect(ShopItemModel item)
    {
        switch (item.Type)
        {
            case ShopItemType.ClickPower:
                _wallet.AddCoinsPerClick(item.Value);
                break;

            case ShopItemType.AutoIncome:
                _wallet.AddCoinsPerSecond(item.Value);
                break;

            case ShopItemType.SuperClick:
                _wallet.AddCoinsPerClick(item.Value);
                break;
        }
    }

    private void RefreshShop()
    {
        foreach (ShopItemView view in _views)
        {
            view.Refresh();
        }
    }
}
