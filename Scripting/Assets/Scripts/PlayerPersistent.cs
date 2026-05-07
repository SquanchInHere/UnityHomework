using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPersistent : MonoBehaviour
{
    public static PlayerPersistent Instance;

    private CharacterController _characterController;
    private Vector3 _startPosition;
    private Quaternion _startRotation;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _characterController = GetComponent<CharacterController>();

        _startPosition = transform.position;
        _startRotation = transform.rotation;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetToStartPosition();
    }

    private void ResetToStartPosition()
    {
        if (_characterController != null)
            _characterController.enabled = false;

        transform.position = _startPosition;
        transform.rotation = _startRotation;

        if (_characterController != null)
            _characterController.enabled = true;

        Debug.Log("Player returned to start position");
    }
}