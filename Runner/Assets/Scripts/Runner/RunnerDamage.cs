using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunnerDamage : MonoBehaviour
{
    [SerializeField] private Material[] _materials;

    private Renderer _runnerRender;
    private int _currentMaterial = 0;

    void Start()
    {
        _runnerRender = GetComponent<Renderer>();

        _runnerRender.material = _materials[_currentMaterial];
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (_currentMaterial < _materials.Length - 1)
            {
                _currentMaterial++;
                _runnerRender.material = _materials[_currentMaterial];
                Destroy(collision.gameObject);
            }
            else
            {
                //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
}
