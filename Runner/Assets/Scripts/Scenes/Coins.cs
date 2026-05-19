using UnityEngine;
using UnityEngine.UI;

public class Coins : MonoBehaviour
{
    [SerializeField] private int _coins = 0;

    [SerializeField] private Text _coinText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            _coins++;

            _coinText.text = "Coins: " + _coins;

            Destroy(other.gameObject);
        }
    }
}
