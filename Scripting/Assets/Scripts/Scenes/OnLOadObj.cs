using UnityEngine;

public class OnLoadObj : MonoBehaviour
{
    public static OnLoadObj Instance;

    public int _coins;
    public string _rightHandItemTag;
    public string _leftHandItemTag;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddCoin(int points)
    {
        _coins += points;
    }
}
