using UnityEngine;

public class Path : MonoBehaviour
{
    [SerializeField] private Transform[] _points;

    public Transform[] Points => _points;

    private void OnDrawGizmos()
    {
        if (_points == null || _points.Length == 0)
            return;

        Gizmos.color = Color.yellow;

        for (int i = 0; i < _points.Length; i++)
        {
            if (_points[i] == null)
                continue;

            Gizmos.DrawSphere(_points[i].position, 0.12f);

            if (i + 1 < _points.Length && _points[i + 1] != null)
                Gizmos.DrawLine(_points[i].position, _points[i + 1].position);
        }
    }
}
