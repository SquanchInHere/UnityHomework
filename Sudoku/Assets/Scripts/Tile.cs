using UnityEngine;
using TMPro;

public class Tile : MonoBehaviour
{
    public int Number;

    [SerializeField] private TextMeshProUGUI NumbertText;

    public void SetNumber(int number)
    {
        Number = number;

        NumbertText.text = number.ToString();
    }

}
