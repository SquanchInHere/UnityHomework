using UnityEngine;

public class MenuToggle : MonoBehaviour
{
    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private GameObject _resetButton;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _menuPanel.SetActive(!_menuPanel.activeSelf);
            _resetButton.SetActive(true);
        }
    }
}
