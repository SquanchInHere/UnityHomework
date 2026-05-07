using UnityEngine;
using UnityEngine.UI;

public class CoinsUi : MonoBehaviour
{
    [SerializeField] private Text _coinsText;

    private void Awake()
    {
        if (_coinsText == null)
            _coinsText = GetComponent<Text>();
    }

    private void Start()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        if (OnLoadObj.Instance == null)
            return;

        _coinsText.text = OnLoadObj.Instance._coins.ToString();
    }
}
