using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private UnitData _data;

    [SerializeField] private float _stopDistance = 0.3f;
    [SerializeField] private float _separationDistance = 1.2f;
    [SerializeField] private float _separationStrength = 3f;
    [SerializeField] private float _formationSpread = 1.5f;

    private float _currentHealth = 0;


    public UnitData Data => _data;

    public UnitType Type => _data != null ? _data.Type : UnitType.Worker;
    public float MaxHealth => _data != null ? _data.MaxHealth : 100f;
    public float MoveSpeed => _data != null ? _data.MoveSpeed : 5f;
    public float Damage => _data != null ? _data.Damage : 10f;
    public float Armor => _data != null ? _data.Armor : 0f;
    public int CarryCapacity => _data != null ? _data.CarryCapacity : 10;
    public float GatherRate => _data != null ? _data.GatherRate : 3f;

    private enum UnitState
    {
        Idle,
        Move,
        MoveToResource,
        Gather,
        MoveToBuilding,
        Deposit
    }

    private UnitState _state;
    private Vector3 _targetPosition;
    private bool _isMoving;
    private bool _isSelected;
    private GameObject _selectionIndicator;

    private ResourcePoint _targetResource;
    private Building _targetBuilding;
    private int _carryAmount;
    private float _gatherCooldown;

    private static readonly Collider[] _separationBuffer = new Collider[32];

    public bool IsWorker()
    {
        return Type == UnitType.Worker;
    }

    public bool IsWarrior()
    {
        return Type == UnitType.Warrior;
    }

    public bool IsArcher()
    {
        return Type == UnitType.Archer;
    }

    public bool IsMage()
    {
        return Type == UnitType.Mage;
    }


    private void Awake()
    {
        _currentHealth = MaxHealth;
        InitializeSelectionIndicator();
    }

    public bool CanGather => Type == UnitType.Worker;

    private void InitializeSelectionIndicator()
    {
        var existing = transform.Find("SelectionIndicator");
        if (existing != null)
        {
            _selectionIndicator = existing.gameObject;
            _selectionIndicator.SetActive(false);
            return;
        }

        _selectionIndicator = new GameObject("SelectionIndicator");
        _selectionIndicator.transform.SetParent(transform);
        _selectionIndicator.transform.localPosition = new Vector3(0, 0.05f, 0);

        var lr = _selectionIndicator.AddComponent<LineRenderer>();
        lr.startWidth = 0.08f;
        lr.endWidth = 0.08f;
        lr.loop = true;
        lr.material = new Material(Shader.Find("Standard"));
        lr.startColor = new Color(0, 1, 0, 0.7f);
        lr.endColor = new Color(0, 1, 0, 0.7f);

        var segments = 30;
        var radius = 0.7f;
        lr.positionCount = segments;

        for (int i = 0; i < segments; i++)
        {
            var angle = (float)i / segments * Mathf.PI * 2f;
            lr.SetPosition(i, new Vector3(Mathf.Sin(angle) * radius, 0, Mathf.Cos(angle) * radius));
        }

        _selectionIndicator.SetActive(false);
    }

    private void Update()
    {
        if (GroundBounds.Instance != null)
            transform.position = GroundBounds.Instance.ClampPosition(transform.position);

        switch (_state)
        {
            case UnitState.Idle:
                break;
            case UnitState.Move:
            case UnitState.MoveToResource:
            case UnitState.MoveToBuilding:
                UpdateMovement();
                break;
            case UnitState.Gather:
                UpdateGather();
                break;
            case UnitState.Deposit:
                UpdateDeposit();
                break;
        }
    }

    private void UpdateMovement()
    {
        if (!_isMoving)
        {
            switch (_state)
            {
                case UnitState.Move:
                    _state = UnitState.Idle;
                    break;
                case UnitState.MoveToResource:
                    _state = UnitState.Gather;
                    break;
                case UnitState.MoveToBuilding:
                    _state = UnitState.Deposit;
                    break;
            }
            return;
        }

        var toTarget = _targetPosition - transform.position;
        toTarget.y = 0;
        var distance = toTarget.magnitude;

        if (distance <= _stopDistance)
        {
            _isMoving = false;
            return;
        }

        var moveDirection = toTarget / distance;
        var moveVector = moveDirection * (MoveSpeed * Time.deltaTime);

        var separation = Vector3.zero;
        var nearbyCount = Physics.OverlapSphereNonAlloc(transform.position, _separationDistance, _separationBuffer);

        for (var i = 0; i < nearbyCount; i++)
        {
            var col = _separationBuffer[i];
            if (col.gameObject == gameObject) continue;

            var other = col.GetComponentInParent<Unit>();
            if (other == null) continue;

            var diff = transform.position - other.transform.position;
            diff.y = 0;
            var dist = diff.magnitude;
            if (dist < 0.01f) continue;

            var strength = 1f - (dist / _separationDistance);
            separation += diff.normalized * (strength * _separationStrength * Time.deltaTime);
        }

        var finalMove = moveVector + separation;
        transform.position += finalMove;

        if (finalMove != Vector3.zero)
        {
            var lookDir = new Vector3(finalMove.x, 0, finalMove.z).normalized;
            if (lookDir != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir),
                    10f * Time.deltaTime);
        }
    }

    private void UpdateGather()
    {
        _gatherCooldown -= Time.deltaTime;
        if (_gatherCooldown > 0) return;

        _gatherCooldown = 1f;

        if (_targetResource == null || !_targetResource.HasResources)
        {
            _state = UnitState.Idle;
            return;
        }

        var taken = _targetResource.Gather(Mathf.RoundToInt(GatherRate));
        _carryAmount += taken;

        if (_carryAmount >= CarryCapacity || !_targetResource.HasResources)
        {
            FindNearestBuilding();
            if (_targetBuilding != null)
            {
                _state = UnitState.MoveToBuilding;
                SetDestination(_targetBuilding.transform.position);
            }
            else
            {
                _state = UnitState.Idle;
            }
        }
    }

    private void UpdateDeposit()
    {
        if (_targetBuilding != null)
            _targetBuilding.Deposit(_carryAmount);

        _carryAmount = 0;

        if (_targetResource != null && _targetResource.HasResources)
        {
            _state = UnitState.MoveToResource;
            SetDestination(_targetResource.transform.position);
        }
        else
        {
            FindNearestResource();
            if (_targetResource != null)
            {
                _state = UnitState.MoveToResource;
                SetDestination(_targetResource.transform.position);
            }
            else
            {
                _state = UnitState.Idle;
            }
        }
    }

    private void FindNearestBuilding()
    {
        var buildings = FindObjectsByType<Building>(FindObjectsSortMode.None);
        float nearestDist = float.MaxValue;
        _targetBuilding = null;

        foreach (var b in buildings)
        {
            if (!b.isActiveAndEnabled) continue;
            if (b.Data == null || !b.Data.CanStoreResources) continue;

            var dist = Vector3.Distance(transform.position, b.transform.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                _targetBuilding = b;
            }
        }
    }

    private void FindNearestResource()
    {
        var resources = FindObjectsByType<ResourcePoint>(FindObjectsSortMode.None);
        float nearestDist = float.MaxValue;
        _targetResource = null;

        foreach (var r in resources)
        {
            if (!r.HasResources) continue;

            var dist = Vector3.Distance(transform.position, r.transform.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                _targetResource = r;
            }
        }
    }

    private void SetDestination(Vector3 destination)
    {
        _targetPosition = destination;
        _targetPosition.y = transform.position.y;
        _isMoving = true;
    }

    public void MoveToCommand(Vector3 destination)
    {
        _state = UnitState.Move;
        _targetResource = null;
        _targetBuilding = null;
        _carryAmount = 0;

        if (GroundBounds.Instance != null)
            destination = GroundBounds.Instance.ClampPosition(destination);

        var offset = Random.insideUnitCircle * _formationSpread;
        _targetPosition = destination + new Vector3(offset.x, 0, offset.y);
        _targetPosition.y = transform.position.y;

        if (GroundBounds.Instance != null)
            _targetPosition = GroundBounds.Instance.ClampPosition(_targetPosition);

        _isMoving = true;
    }

    public void StartGathering(ResourcePoint resource)
    {
        if (!CanGather) return;

        if (resource == null || !resource.HasResources) return;

        _targetResource = resource;
        _carryAmount = 0;
        _gatherCooldown = 0;
        _state = UnitState.MoveToResource;

        SetDestination(resource.transform.position);
    }

    public void SetSelected(bool selected)
    {
        _isSelected = selected;
        if (_selectionIndicator != null)
            _selectionIndicator.SetActive(selected);
    }


    public float CurrentHealth => _currentHealth;

    public bool IsDead => CurrentHealth <= 0f;

    public void TakeDamage(float rawDamage)
    {
        if (IsDead) return;

        float finalDamage = rawDamage - Armor;

        _currentHealth -= finalDamage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, MaxHealth);

        if (_currentHealth <= 0f) Die();
    }

    public void Heal(float amount)
    {
        if (IsDead) return;

        _currentHealth += amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, MaxHealth);
    }


    private void Die()
    {
        Destroy(gameObject);
    }
}