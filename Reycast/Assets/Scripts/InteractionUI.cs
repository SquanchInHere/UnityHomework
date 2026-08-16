using TMPro;
using UnityEngine;

public class InteractionUI : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private TMP_Text _promptText;

    public void Show(string text, KeyCode key)
    {
        _promptText.text = $"{text} [{key}]";
        _root.SetActive(true);
    }

    public void Hide()
    {
        _root.SetActive(false);
    }
}