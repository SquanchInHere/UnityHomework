using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemView : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _itemNameText;
    [SerializeField] private TMP_Text _priceText;
    [SerializeField] private Button _buyButton;

    private ShopItems itemData;

    public void Init(ShopItems data)
    {
        itemData = data;

        _itemNameText.text = data.Name;
        _priceText.text =data.Price.ToString();

        if (data.icon != null)
        {
            _iconImage.sprite = data.icon;
            _iconImage.enabled = true;
            _iconImage.preserveAspect = true;
        }
        else
        {
            _iconImage.enabled = false;
        }

        _buyButton.onClick.RemoveAllListeners();
        _buyButton.onClick.AddListener(Buy);
    }

    private void Buy()
    {
        Debug.Log($"Buy item: {itemData.Name}, price: {itemData.Price}");
    }
}
