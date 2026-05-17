using UnityEngine;
using UnityEngine.SceneManagement;

public class CloseShop : MonoBehaviour
{
    [SerializeField] private string shopSceneName = "ShopMenu";

    public void CloseShopBtn()
    {
        SceneManager.UnloadSceneAsync(shopSceneName);
        Time.timeScale = 1f;
    }
}
