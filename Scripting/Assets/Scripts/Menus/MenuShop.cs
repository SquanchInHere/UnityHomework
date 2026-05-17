using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  
using UnityEngine.UI; 

public class MenuShop : MonoBehaviour
{
    [Header("Shop")]
    [SerializeField] private Transform _itemsGrid;
    [SerializeField] private ItemView _itemPrefab;


    [Header("Items")]
    [SerializeField] private List<ShopItems> items = new();

    private void Start()
    {
        GenerateItems();
    }

    private void GenerateItems()
    {
        ClearItems();

        foreach (ShopItems item in items)
        {
            ItemView itemView = Instantiate(_itemPrefab, _itemsGrid);
            itemView.Init(item);
        }
    }

    private void ClearItems()
    {
        for (int i = _itemsGrid.childCount - 1; i >= 0; i--)
        {
            Destroy(_itemsGrid.GetChild(i).gameObject);
        }
    }
}
