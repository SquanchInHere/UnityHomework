using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private SpriteRenderer _highlightRenderer;
    [SerializeField] private ParticleSystem _destroyEffect;

    private int _column;
    private int _row;
    private TileType _type;
    private Board _board;
    private Vector2 _targetPosition;
    private bool _isMoving;
    private float _moveSpeed = 10f;

    public int Column => _column;
    public int Row => _row;
    public TileType Type => _type;
    public bool IsMoving => _isMoving;

    public event System.Action<Tile> Clicked;

    public void Initialize(Board board, int column, int row, TileType type, Sprite sprite, Color color)
    {
        _board = board;
        _column = column;
        _row = row;
        _type = type;
        _spriteRenderer.sprite = sprite;
        _spriteRenderer.color = color;

        //_spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        FitSpriteToCell();

        _targetPosition = transform.position;
        SetHighlight(false);
    }

    private void FitSpriteToCell()
    {
        if (_board == null || _spriteRenderer == null || _spriteRenderer.sprite == null)
        {
            return;
        }

        float targetSize = _board.TileSize * 0.85f;

        transform.localScale = Vector3.one;

        Vector2 spriteSize = _spriteRenderer.bounds.size;

        if (spriteSize.x <= 0f || spriteSize.y <= 0f)
        {
            return;
        }

        float maxSpriteSize = Mathf.Max(spriteSize.x, spriteSize.y);
        float scale = targetSize / maxSpriteSize;

        transform.localScale = Vector3.one * scale;
    }

    public void SetGridPosition(int column, int row)
    {
        _column = column;
        _row = row;
    }

    public void MoveToPosition(Vector2 position)
    {

        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        _targetPosition = position;
        _isMoving = true;

        StopAllCoroutines();
        StartCoroutine(MoveCoroutine());
        //_targetPosition = position;
        //_isMoving = true;
        //StopAllCoroutines();
        //StartCoroutine(MoveCoroutine());
    }

    private IEnumerator MoveCoroutine()
    {
        while (Vector2.Distance(transform.position, _targetPosition) > 0.01f)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                _targetPosition,
                _moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.position = _targetPosition;
        _isMoving = false;
    }

    public void SetHighlight(bool state)
    {
        if (_highlightRenderer != null)
        {
            _highlightRenderer.gameObject.SetActive(state);
        }
    }

    public void PlayDestroyEffect()
    {
        if (_destroyEffect != null)
        {
            ParticleSystem effect = Instantiate(_destroyEffect, transform.position, Quaternion.identity);
            Destroy(effect.gameObject, effect.main.duration);
        }
    }

    private void OnMouseDown()
    {
        Clicked?.Invoke(this);
    }
}
