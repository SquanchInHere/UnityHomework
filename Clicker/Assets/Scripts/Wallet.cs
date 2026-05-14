using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Wallet : MonoBehaviour
{
    [SerializeField] private float _coins = 0.0f;
    [SerializeField] private float _coinsPerClick = 1f;
    [SerializeField] private float _coinsPerSecond = 0f;

    [SerializeField] private TextMeshProUGUI _coinText;

    public float Coins => _coins;

    private void Start()
    {
        UpdateView();
    }

    private void Update()
    {
        AddPassiveIncome();
    }

    public void AddClickCoins()
    {
        _coins += _coinsPerClick;
        UpdateView();
    }

    public bool TrySpend(float price)
    {
        if (_coins < price)
            return false;

        _coins -= price;
        UpdateView();

        return true;
    }

    public void AddCoinsPerClick(float value)
    {
        _coinsPerClick += value;
    }

    public void AddCoinsPerSecond(float value)
    {
        _coinsPerSecond += value;
    }

    private void AddPassiveIncome()
    {
        if (_coinsPerSecond <= 0f)
            return;

        _coins += _coinsPerSecond * Time.deltaTime;
        UpdateView();
    }

    private void UpdateView()
    {
        if (_coinText != null)
            _coinText.text = Mathf.FloorToInt(_coins) + "$";
    }
}
