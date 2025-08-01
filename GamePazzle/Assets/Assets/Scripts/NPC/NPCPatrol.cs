using System.Collections;
// using TMPro; // Вам это не нужно, если вы не используете TextMeshPro, можно удалить 
using UnityEngine;
using UnityEngine.AI;

public class NPCPatrol : MonoBehaviour
{
    // Имя параметра в Animator, который управляет значением для бленд-дерева (0=Idle, 0.5=Walk, 1.0=Run) 
    const string SPEED_ANIM_PARAM = "speed";

    // НОВОЕ: ID параметра для множителя скорости анимации (например, для слоя или состояний) 
    // Используем Animator.StringToHash для производительности 
    private readonly int _animationPatrolSpeedMultiplierHash = Animator.StringToHash("patrolSpeedMultiplier"); // <--- ИЗМЕНЕНО НА НОВОЕ ИМЯ 

    private Animator _animator;

    [Header("Patrol Settings")]
    [Tooltip("Точки, по которым будет патрулировать NPC")]
    [SerializeField] private Transform[] _patrolPoints;
    [Tooltip("Скорость движения NPC при патруле")]
    [SerializeField] private float _patrolSpeed = 3.5f;
    [Tooltip("Время остановки на каждой точке патрулирования")]
    [SerializeField] private float _stopDuration = 2f;
    [Tooltip("Угловая скорость поворота NPC при патрулировании")]
    [SerializeField] private float _patrolAngularSpeed = 360f; // Начальное значение для патрулирования 

    [Header("Detection Settings")]
    [Tooltip("Радиус, в котором NPC ищет игрока")]
    [SerializeField] private float _detectionRadius = 10f;
    [Range(0, 360), Tooltip("Угол обзора NPC")]
    [SerializeField] private float _fieldOfViewAngle = 120f;
    [Tooltip("Слой, на котором находится игрок")]
    [SerializeField] private LayerMask _playerLayer;
    [Tooltip("Слои препятствий для Raycast")]
    [SerializeField] private LayerMask _obstacleLayer;

    [Header("Chase Settings")]
    [Tooltip("Скорость NPC при преследовании")]
    [SerializeField] private float _chaseSpeed = 5f;
    [Tooltip("Минимальное расстояние до цели при преследовании")]
    [SerializeField] private float _chaseStoppingDistance = 1.5f;
    [Tooltip("Угловая скорость поворота NPC при преследовании")]
    [SerializeField] private float _chaseAngularSpeed = 720f; // Начальное значение для преследования (быстрее) 


    [Header("Animation Settings")]
    [Tooltip("Скорость, с которой изменяется параметр Speed в Animator")]
    [SerializeField] private float _animationTransitionSpeed = 5f;
    [Tooltip("Множитель скорости анимации для патрулирования (Walk)")]
    [SerializeField] private float _patrolAnimationSpeedMultiplier = 1f; // Новый множитель для патруля 
    [Tooltip("Множитель скорости анимации для преследования (Run)")]
    [SerializeField] private float _chaseAnimationSpeedMultiplier = 1.5f; // Новый множитель для преследования 

    private NavMeshAgent _agent;
    private Transform _playerTransform;
    private Transform _currentTarget;
    private int _currentPatrolIndex = 0;
    private bool _isWaiting = false;

    private enum NPCState { Patrolling, Chasing }
    private NPCState _currentState;

    private enum NPCStateMovement { Idle, Walk, Run }
    private NPCStateMovement _currentMovementMode = NPCStateMovement.Walk;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        if (_agent == null)
        {
            Debug.LogError($"[{name}] NavMeshAgent не найден; скрипт отключен.", this);
            enabled = false;
            return;
        }
        if (_animator == null)
        {
            Debug.LogWarning($"[{name}] Animator не найден; анимации не будут работать.", this);
        }
        _agent.autoBraking = false;
    }

    private void Start()
    {
        _currentState = NPCState.Patrolling;

        _playerTransform = GameObject.FindWithTag("Player")?.transform;

        if (_patrolPoints == null || _patrolPoints.Length == 0) // Исправлено: использовано _patrolPoints
        {
            Debug.LogWarning($"[{name}] Точки патрулирования не заданы; NPC остановится.", this);
            _agent.isStopped = true;
            enabled = false;
            return;
        }

        _agent.speed = _patrolSpeed; // Исправлено: использовано _patrolSpeed
        _agent.stoppingDistance = 0f;
        _agent.angularSpeed = _patrolAngularSpeed; // Устанавливаем угловую скорость при старте
        GoToNextPatrolPoint();
    }

    private void Update()
    {
        UpdateAnimations();

        if (_currentState == NPCState.Patrolling)
            UpdatePatrollingState();
        else if (_currentState == NPCState.Chasing)
            UpdateChasingState();
    }

    private void UpdatePatrollingState()
    {
        if (_playerTransform != null && CanSeePlayer())
        {
            StartChasing(_playerTransform);
            return;
        }

        if (_isWaiting || _agent.pathPending)
            return;

        if (_agent.remainingDistance <= _agent.stoppingDistance)
            StartCoroutine(WaitAtPatrolPoint());
    }

    private void UpdateChasingState()
    {
        if (_currentTarget == null)
        {
            StopChasing();
            return;
        }

        if (_agent != null && _agent.enabled)
        {
            _agent.SetDestination(_currentTarget.position);
        }

        float dist = Vector3.Distance(transform.position, _currentTarget.position);
        if (dist > _detectionRadius * 1.5f) // Исправлено: использовано _detectionRadius
            StopChasing();
    }

    private void GoToNextPatrolPoint()
    {
        if (_patrolPoints.Length == 0) return; // Исправлено: использовано _patrolPoints

        _currentMovementMode = NPCStateMovement.Walk;
        _agent.SetDestination(_patrolPoints[_currentPatrolIndex].position); // Исправлено: использовано _patrolPoints
        Debug.Log($"[{name}] Патруль к точке #{_currentPatrolIndex}: {_patrolPoints[_currentPatrolIndex].name}"); // Исправлено: использовано _patrolPoints
        _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrolPoints.Length; // Исправлено: использовано _patrolPoints
    }

    private IEnumerator WaitAtPatrolPoint()
    {
        _isWaiting = true;
        _agent.isStopped = true;

        _currentMovementMode = NPCStateMovement.Idle;
        yield return new WaitForSeconds(_stopDuration); // Исправлено: использовано _stopDuration

        _agent.isStopped = false;
        _isWaiting = false;
        GoToNextPatrolPoint();
    }

    private bool CanSeePlayer()
    {
        // 1) Радиус 
        Collider[] hits = Physics.OverlapSphere(transform.position, _detectionRadius, _playerLayer); // Исправлено: использовано _detectionRadius, _playerLayer
        if (hits.Length == 0) return false;

        // 2) Угол обзора 
        Vector3 dir = (_playerTransform.position - transform.position).normalized;
        if (Vector3.Angle(transform.forward, dir) > _fieldOfViewAngle / 2f) // Исправлено: использовано _fieldOfViewAngle
            return false;

        // 3) Raycast на прямую видимость 
        Vector3 origin = transform.position + Vector3.up * 1.5f;
        if (Physics.Raycast(origin, dir, out RaycastHit hit, _detectionRadius, _obstacleLayer | _playerLayer)) // Исправлено: использовано _detectionRadius, _obstacleLayer, _playerLayer
        {
            if (((1 << hit.collider.gameObject.layer) & _playerLayer) != 0) // Исправлено: использовано _playerLayer
                return true;
        }
        return false;
    }

    public void StartChasing(Transform target)
    {
        _currentState = NPCState.Chasing;
        _currentMovementMode = NPCStateMovement.Run;
        _currentTarget = target;
        _agent.speed = _chaseSpeed; // Исправлено: использовано _chaseSpeed
        _agent.stoppingDistance = _chaseStoppingDistance; // Исправлено: использовано _chaseStoppingDistance
        _agent.angularSpeed = _chaseAngularSpeed; // Устанавливаем угловую скорость для преследования
        Debug.Log($"[{name}] Начало преследования {target.name}");
        StopAllCoroutines();
        _isWaiting = false;
    }

    private void StopChasing()
    {
        _currentState = NPCState.Patrolling;
        _currentMovementMode = NPCStateMovement.Walk;
        _currentTarget = null;
        _agent.speed = _patrolSpeed; // Исправлено: использовано _patrolSpeed
        _agent.stoppingDistance = 0f;
        _agent.angularSpeed = _patrolAngularSpeed; // Возвращаем угловую скорость патрулирования
        Debug.Log($"[{name}] Возврат к патрулю");
        GoToNextPatrolPoint();
    }

    void UpdateAnimations()
    {
        if (_animator == null) return;

        float targetAnimSpeedValue = 0f; // Значение для параметра "speed" (Idle/Walk/Run) 
        float targetSpeedMultiplier = 1f; // Целевое значение для множителя скорости анимации 

        switch (_currentMovementMode)
        {
            case NPCStateMovement.Idle:
                targetAnimSpeedValue = 0f;
                targetSpeedMultiplier = 1f; // Множитель для покоя (обычно 1) 
                break;
            case NPCStateMovement.Walk:
                targetAnimSpeedValue = 0.5f;
                targetSpeedMultiplier = _patrolAnimationSpeedMultiplier; // Исправлено: использовано _patrolAnimationSpeedMultiplier
                break;
            case NPCStateMovement.Run:
                targetAnimSpeedValue = 1.0f;
                targetSpeedMultiplier = _chaseAnimationSpeedMultiplier; // Исправлено: использовано _chaseAnimationSpeedMultiplier
                break;
        }

        // Плавный переход для параметра "speed" (Idle/Walk/Run) 
        float currentAnimSpeed = _animator.GetFloat(SPEED_ANIM_PARAM);
        float newAnimSpeed = Mathf.Lerp(currentAnimSpeed, targetAnimSpeedValue, Time.deltaTime * _animationTransitionSpeed); // Исправлено: использовано _animationTransitionSpeed
        _animator.SetFloat(SPEED_ANIM_PARAM, newAnimSpeed);

        // Установка множителя скорости анимации 
        float currentSpeedMultiplier = _animator.GetFloat(_animationPatrolSpeedMultiplierHash);
        float newSpeedMultiplier = Mathf.Lerp(currentSpeedMultiplier, targetSpeedMultiplier, Time.deltaTime * _animationTransitionSpeed); // Исправлено: использовано _animationTransitionSpeed
        _animator.SetFloat(_animationPatrolSpeedMultiplierHash, newSpeedMultiplier);
    }

    private void OnDrawGizmos()
    {
        if (_patrolPoints != null) // Исправлено: использовано _patrolPoints
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < _patrolPoints.Length; i++) // Исправлено: использовано _patrolPoints
            {
                if (_patrolPoints[i] == null) continue; // Исправлено: использовано _patrolPoints
                Gizmos.DrawWireSphere(_patrolPoints[i].position, 0.5f); // Исправлено: использовано _patrolPoints
                if (_patrolPoints.Length > 1) // Исправлено: использовано _patrolPoints
                {
                    var next = (i + 1) % _patrolPoints.Length; // Исправлено: использовано _patrolPoints
                    Gizmos.DrawLine(_patrolPoints[i].position, _patrolPoints[next].position); // Исправлено: использовано _patrolPoints
                }
            }
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius); // Исправлено: использовано _detectionRadius
    }

    private void OnDrawGizmosSelected()
    {
        if (_agent == null) _agent = GetComponent<NavMeshAgent>();
        if (_agent == null) return;

        Vector3 origin = transform.position + Vector3.up * 1.5f;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, _detectionRadius); // Исправлено: использовано _detectionRadius

        Gizmos.color = new Color(1f, 1f, 0f, 0.5f);

        int segments = 30;
        float angleStep = _fieldOfViewAngle / segments; // Исправлено: использовано _fieldOfViewAngle
        Vector3 prevPoint = origin + Quaternion.Euler(0, -_fieldOfViewAngle / 2f, 0) * transform.forward * _detectionRadius; // Исправлено: использовано _fieldOfViewAngle, _detectionRadius

        for (int i = 1; i <= segments; i++)
        {
            float currentAngle = -_fieldOfViewAngle / 2f + angleStep * i; // Исправлено: использовано _fieldOfViewAngle
            Vector3 nextPoint = origin + Quaternion.Euler(0, currentAngle, 0) * transform.forward * _detectionRadius; // Исправлено: использовано _detectionRadius

            Gizmos.DrawLine(origin, nextPoint);
            Gizmos.DrawLine(prevPoint, nextPoint);

            prevPoint = nextPoint;
        }
    }
}