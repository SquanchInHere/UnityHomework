using UnityEngine;
using UnityEngine.SceneManagement;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] private GameObject _btnMenu;
    [SerializeField] private GameObject _btnStart;
    [SerializeField] private GameObject _btnExit;
    [SerializeField] private GameObject _panelOptions;

    public void StartGame()
    {
        SceneManager.LoadScene("lvl_1");
    }

    public void TogleOption()
    {
        _panelOptions.SetActive(!_panelOptions.activeSelf);

        _btnMenu.SetActive(!_btnMenu.activeSelf);
        _btnStart.SetActive(!_btnStart.activeSelf);
        _btnExit.SetActive(!_btnExit.activeSelf);
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }
}
