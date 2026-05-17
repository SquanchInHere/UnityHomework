using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadShop : MonoBehaviour
{
    [SerializeField] private string shopSceneName = "ShopMenu";

    public void OnTriggerEnter()
    {
        Time.timeScale = 0f;
        SceneManager.LoadScene(shopSceneName, LoadSceneMode.Additive);
    }

}
