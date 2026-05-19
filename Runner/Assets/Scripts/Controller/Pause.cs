using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private MonoBehaviour _runnerController;

    private bool _isOpen = false;
    private bool _isPaused = false;

    private void Start()
    {
        _menuPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _isPaused = !_isPaused;
            TogglePanel();
        }
    }

    private void TogglePanel()
    {
        _isOpen = !_isOpen;
        _menuPanel.SetActive(_isOpen);
        Time.timeScale = (_isOpen && _isPaused) ? 0f : 1f;

        _runnerController.enabled = !(_isOpen && _isPaused);

        Cursor.visible = _isOpen;
        Cursor.lockState = _isOpen ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void RestartLevel()
    {
        _isPaused = false;
        TogglePanel();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnGame()
    {
        _isPaused = false;
        TogglePanel();
    }
}
