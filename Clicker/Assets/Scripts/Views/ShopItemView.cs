using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemView : MonoBehaviour
{
    [Header("Level Value")]
    [SerializeField] private TextMeshProUGUI _levelText;

    [Header("Price Value")]
    [SerializeField] private TextMeshProUGUI _priceValueText;

    [Header("Button")]
    [SerializeField] private Button _buyButton;

    private ShopItemModel _item;
    private ShopManager _shopManager;

    [Header("Item content")]
    [SerializeField] private GameObject _itemContentPrefab;
    [SerializeField] private GameObject _itemMaxLvlPrefab;

    public void Init(ShopItemModel item, ShopManager shopManager)
    {
        _item = item;
        _shopManager = shopManager;

        _buyButton.onClick.RemoveAllListeners();
        _buyButton.onClick.AddListener(Buy);

        Refresh();
    }

    private void Buy()
    {
        _shopManager.BuyItem(_item);
    }

    public void Refresh()
    {
        if (_item == null || _shopManager == null)
            return;

        bool unlocked = _shopManager.IsUnlocked(_item);
        bool canBuy = _shopManager.CanBuy(_item);

        _levelText.text = _item.Level.ToString();
        Debug.Log(_item.Level);
        Debug.Log(_item.MaxLvl);
        Debug.Log(_item.Level >= _item.MaxLvl);
        if (_item.Level >= _item.MaxLvl)
        {
            _itemContentPrefab.SetActive(false);
            _itemMaxLvlPrefab.SetActive(true);
            return;
        }


        if (!unlocked)
        {

            _priceValueText.text = Mathf.RoundToInt(_item.UnlockPrice) + "$";
            _buyButton.interactable = false;
            return;
        }

        _priceValueText.text = Mathf.RoundToInt(_item.Price) + "$";

        _buyButton.interactable = canBuy;
    }
}
