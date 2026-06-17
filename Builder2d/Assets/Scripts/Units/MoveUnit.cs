using UnityEngine;

public class MoveUnit : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _stopDistance = 0.05f;
    [SerializeField] private bool _loop;

    private Rigidbody2D _rb;
    private Transform[] _points;
    private int _currentPointIndex;
    private bool _isMoving;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        _rb.gravityScale = 0f;
        _rb.freezeRotation = true;
    }

    private void FixedUpdate()
    {
        if (!_isMoving)
            return;

        if (_points == null || _points.Length == 0)
            return;

        Transform target = _points[_currentPointIndex];

        if (target == null)
        {
            GoToNextPoint();
            return;
        }

        Vector2 currentPosition = _rb.position;
        Vector2 targetPosition = target.position;

        Vector2 nextPosition = Vector2.MoveTowards(
            currentPosition,
            targetPosition,
            _speed * Time.fixedDeltaTime
        );

        _rb.MovePosition(nextPosition);

        if (Vector2.Distance(nextPosition, targetPosition) <= _stopDistance)
        {
            GoToNextPoint();
        }
    }

    private void GoToNextPoint()
    {
        _currentPointIndex++;

        if (_currentPointIndex >= _points.Length)
        {
            if (_loop)
            {
                _currentPointIndex = 0;
            }
            else
            {
                StopMove();
            }
        }
    }

    public void SetPath(Transform[] points)
    {
        _points = points;
        _currentPointIndex = 0;
        _isMoving = _points != null && _points.Length > 0;
    }

    public void StopMove()
    {
        _isMoving = false;

        if (_rb != null)
            _rb.linearVelocity = Vector2.zero;
    }
}
