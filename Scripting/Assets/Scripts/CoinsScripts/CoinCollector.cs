using UnityEngine;
using UnityEngine.UI;

public class CoinCollector : MonoBehaviour
{
    [SerializeField] private Text _coinsText;

    private void Start()
    {
        FindCoinsText();
        UpdateCoinsText();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Coin") && !other.CompareTag("BigCoin"))
            return;

        OnLoadObj.Instance.AddCoin(other.CompareTag("BigCoin") ? 5 : 1);

        UpdateCoinsText();

        other.gameObject.SetActive(false);
    }

    private void FindCoinsText()
    {
        if (_coinsText != null)
            return;

        GameObject textObject = GameObject.FindWithTag("CoinsText");

        if (textObject != null)
            _coinsText = textObject.GetComponent<Text>();
    }

    private void UpdateCoinsText()
    {
        if (_coinsText == null)
            FindCoinsText();

        if (_coinsText != null)
            _coinsText.text = OnLoadObj.Instance._coins.ToString();
    }
}
