using System.Collections;
using UnityEngine;

public class Spike : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private float _growTime = 0.2f;
    [SerializeField] private float _lifeTime = 1.2f;
    [SerializeField] private float _hideTime = 0.2f;

    [Header("Scale")]
    [SerializeField] private Vector3 _startScale = new Vector3(0.1f, 0.1f, 0.1f);
    [SerializeField] private Vector3 _targetScale = new Vector3(0.2f, 2f, 0.2f);

    private void Start()
    {
        StartCoroutine(SpikeRoutine());
    }

    private IEnumerator SpikeRoutine()
    {
        yield return ScaleOverTime(_startScale, _targetScale, _growTime);

        yield return new WaitForSeconds(_lifeTime);

        yield return ScaleOverTime(_targetScale, Vector3.zero, _hideTime);

        Destroy(gameObject);
    }

    private IEnumerator ScaleOverTime(Vector3 from, Vector3 to, float duration)
    {
        float timer = 0f;

        transform.localScale = from;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = timer / duration;
            t = Mathf.Clamp01(t);

            transform.localScale = Vector3.Lerp(from, to, t);

            yield return null;
        }

        transform.localScale = to;
    }
}
