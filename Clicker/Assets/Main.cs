using UnityEngine;
using UnityEngine.InputSystem;

public class Main : MonoBehaviour
{
    [Header("Wallet")]
    [SerializeField] private Wallet _wallet;

    [Header("Objects")]
    [SerializeField] private GameObject _shop;

    public void AddCoin()
    {
        _wallet.AddClickCoins();
    }

    public void ToggleShop()
    {
        _shop.SetActive(!_shop.activeSelf);
    }
}