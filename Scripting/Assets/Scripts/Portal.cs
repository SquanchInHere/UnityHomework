using UnityEngine;
using UnityEngine.SceneManagement;


public class Portal : MonoBehaviour
{
    [SerializeField] private string _firstSceneName = "lvl_1";
    [SerializeField] private string _secondSceneName = "lvl_2";

    private void OnTriggerEnter(Collider other)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName == _firstSceneName)
            SceneManager.LoadScene(_secondSceneName);
        else if (currentSceneName == _secondSceneName)
            SceneManager.LoadScene(_firstSceneName);
    }
}
