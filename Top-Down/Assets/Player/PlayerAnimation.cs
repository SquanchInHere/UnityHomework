using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(InputReader))]
public class PlayerAnimation : MonoBehaviour
{
    private static readonly int MoveX = Animator.StringToHash("MoveX");
    private static readonly int MoveY = Animator.StringToHash("MoveY");
    private static readonly int Speed = Animator.StringToHash("Speed");

    private Animator _animator;
    private InputReader _inputReader;

    private Vector2 _lastDirection = Vector2.up;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _inputReader = GetComponent<InputReader>();
    }

    private void OnEnable()
    {
        if (_inputReader != null)
            _inputReader.OnMove += HandleMove;
    }

    private void OnDisable()
    {
        if (_inputReader != null)
            _inputReader.OnMove -= HandleMove;
    }

    private void HandleMove(Vector2 input)
    {
        float speed = input.sqrMagnitude;

        if (speed > 0.01f)
        {
            _lastDirection = GetCardinalDirection(input);

            _animator.SetFloat(MoveX, _lastDirection.x);
            _animator.SetFloat(MoveY, _lastDirection.y);
        }

        _animator.SetFloat(Speed, speed);
    }

    private static Vector2 GetCardinalDirection(Vector2 input)
    {
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            return input.x > 0 ? Vector2.right : Vector2.left;

        return input.y > 0 ? Vector2.up : Vector2.down;
    }
}