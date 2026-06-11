using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public bool isStart = false;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject StartButton;
    [SerializeField] private GameObject ResetButton;

    private void Start()
    {
        mainMenu.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePanel();
        }
    }

    public void TogglePanel()
    {
        mainMenu.SetActive(!mainMenu.activeSelf);

        if (isStart)
        {
            StartButton.SetActive(false);
            ResetButton.SetActive(true);
        }

    }

    public void RestartLevel()
    {
        TogglePanel();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Exit()
    {
        Debug.Log("Exit Game!");
    }
}
