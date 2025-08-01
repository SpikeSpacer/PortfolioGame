using System.Collections;
// using TMPro; // ��� ��� �� �����, ���� �� �� ����������� TextMeshPro, ����� ������� 
using UnityEngine;
using UnityEngine.AI;

public class NPCPatrol : MonoBehaviour
{
    // ��� ��������� � Animator, ������� ��������� ��������� ��� �����-������ (0=Idle, 0.5=Walk, 1.0=Run) 
    const string SPEED_ANIM_PARAM = "speed";

    // �����: ID ��������� ��� ��������� �������� �������� (��������, ��� ���� ��� ���������) 
    // ���������� Animator.StringToHash ��� ������������������ 
    private readonly int _animationPatrolSpeedMultiplierHash = Animator.StringToHash("patrolSpeedMultiplier"); // <--- �������� �� ����� ��� 

    private Animator _animator;

    [Header("Patrol Settings")]
    [Tooltip("�����, �� ������� ����� ������������� NPC")]
    [SerializeField] private Transform[] _patrolPoints;
    [Tooltip("�������� �������� NPC ��� �������")]
    [SerializeField] private float _patrolSpeed = 3.5f;
    [Tooltip("����� ��������� �� ������ ����� ��������������")]
    [SerializeField] private float _stopDuration = 2f;
    [Tooltip("������� �������� �������� NPC ��� ��������������")]
    [SerializeField] private float _patrolAngularSpeed = 360f; // ��������� �������� ��� �������������� 

    [Header("Detection Settings")]
    [Tooltip("������, � ������� NPC ���� ������")]
    [SerializeField] private float _detectionRadius = 10f;
    [Range(0, 360), Tooltip("���� ������ NPC")]
    [SerializeField] private float _fieldOfViewAngle = 120f;
    [Tooltip("����, �� ������� ��������� �����")]
    [SerializeField] private LayerMask _playerLayer;
    [Tooltip("���� ����������� ��� Raycast")]
    [SerializeField] private LayerMask _obstacleLayer;

    [Header("Chase Settings")]
    [Tooltip("�������� NPC ��� �������������")]
    [SerializeField] private float _chaseSpeed = 5f;
    [Tooltip("����������� ���������� �� ���� ��� �������������")]
    [SerializeField] private float _chaseStoppingDistance = 1.5f;
    [Tooltip("������� �������� �������� NPC ��� �������������")]
    [SerializeField] private float _chaseAngularSpeed = 720f; // ��������� �������� ��� ������������� (�������) 


    [Header("Animation Settings")]
    [Tooltip("��������, � ������� ���������� �������� Speed � Animator")]
    [SerializeField] private float _animationTransitionSpeed = 5f;
    [Tooltip("��������� �������� �������� ��� �������������� (Walk)")]
    [SerializeField] private float _patrolAnimationSpeedMultiplier = 1f; // ����� ��������� ��� ������� 
    [Tooltip("��������� �������� �������� ��� ������������� (Run)")]
    [SerializeField] private float _chaseAnimationSpeedMultiplier = 1.5f; // ����� ��������� ��� ������������� 

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
            Debug.LogError($"[{name}] NavMeshAgent �� ������; ������ ��������.", this);
            enabled = false;
            return;
        }
        if (_animator == null)
        {
            Debug.LogWarning($"[{name}] Animator �� ������; �������� �� ����� ��������.", this);
        }
        _agent.autoBraking = false;
    }

    private void Start()
    {
        _currentState = NPCState.Patrolling;

        _playerTransform = GameObject.FindWithTag("Player")?.transform;

        if (_patrolPoints == null || _patrolPoints.Length == 0) // ����������: ������������ _patrolPoints
        {
            Debug.LogWarning($"[{name}] ����� �������������� �� ������; NPC �����������.", this);
            _agent.isStopped = true;
            enabled = false;
            return;
        }

        _agent.speed = _patrolSpeed; // ����������: ������������ _patrolSpeed
        _agent.stoppingDistance = 0f;
        _agent.angularSpeed = _patrolAngularSpeed; // ������������� ������� �������� ��� ������
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
        if (dist > _detectionRadius * 1.5f) // ����������: ������������ _detectionRadius
            StopChasing();
    }

    private void GoToNextPatrolPoint()
    {
        if (_patrolPoints.Length == 0) return; // ����������: ������������ _patrolPoints

        _currentMovementMode = NPCStateMovement.Walk;
        _agent.SetDestination(_patrolPoints[_currentPatrolIndex].position); // ����������: ������������ _patrolPoints
        Debug.Log($"[{name}] ������� � ����� #{_currentPatrolIndex}: {_patrolPoints[_currentPatrolIndex].name}"); // ����������: ������������ _patrolPoints
        _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrolPoints.Length; // ����������: ������������ _patrolPoints
    }

    private IEnumerator WaitAtPatrolPoint()
    {
        _isWaiting = true;
        _agent.isStopped = true;

        _currentMovementMode = NPCStateMovement.Idle;
        yield return new WaitForSeconds(_stopDuration); // ����������: ������������ _stopDuration

        _agent.isStopped = false;
        _isWaiting = false;
        GoToNextPatrolPoint();
    }

    private bool CanSeePlayer()
    {
        // 1) ������ 
        Collider[] hits = Physics.OverlapSphere(transform.position, _detectionRadius, _playerLayer); // ����������: ������������ _detectionRadius, _playerLayer
        if (hits.Length == 0) return false;

        // 2) ���� ������ 
        Vector3 dir = (_playerTransform.position - transform.position).normalized;
        if (Vector3.Angle(transform.forward, dir) > _fieldOfViewAngle / 2f) // ����������: ������������ _fieldOfViewAngle
            return false;

        // 3) Raycast �� ������ ��������� 
        Vector3 origin = transform.position + Vector3.up * 1.5f;
        if (Physics.Raycast(origin, dir, out RaycastHit hit, _detectionRadius, _obstacleLayer | _playerLayer)) // ����������: ������������ _detectionRadius, _obstacleLayer, _playerLayer
        {
            if (((1 << hit.collider.gameObject.layer) & _playerLayer) != 0) // ����������: ������������ _playerLayer
                return true;
        }
        return false;
    }

    public void StartChasing(Transform target)
    {
        _currentState = NPCState.Chasing;
        _currentMovementMode = NPCStateMovement.Run;
        _currentTarget = target;
        _agent.speed = _chaseSpeed; // ����������: ������������ _chaseSpeed
        _agent.stoppingDistance = _chaseStoppingDistance; // ����������: ������������ _chaseStoppingDistance
        _agent.angularSpeed = _chaseAngularSpeed; // ������������� ������� �������� ��� �������������
        Debug.Log($"[{name}] ������ ������������� {target.name}");
        StopAllCoroutines();
        _isWaiting = false;
    }

    private void StopChasing()
    {
        _currentState = NPCState.Patrolling;
        _currentMovementMode = NPCStateMovement.Walk;
        _currentTarget = null;
        _agent.speed = _patrolSpeed; // ����������: ������������ _patrolSpeed
        _agent.stoppingDistance = 0f;
        _agent.angularSpeed = _patrolAngularSpeed; // ���������� ������� �������� ��������������
        Debug.Log($"[{name}] ������� � �������");
        GoToNextPatrolPoint();
    }

    void UpdateAnimations()
    {
        if (_animator == null) return;

        float targetAnimSpeedValue = 0f; // �������� ��� ��������� "speed" (Idle/Walk/Run) 
        float targetSpeedMultiplier = 1f; // ������� �������� ��� ��������� �������� �������� 

        switch (_currentMovementMode)
        {
            case NPCStateMovement.Idle:
                targetAnimSpeedValue = 0f;
                targetSpeedMultiplier = 1f; // ��������� ��� ����� (������ 1) 
                break;
            case NPCStateMovement.Walk:
                targetAnimSpeedValue = 0.5f;
                targetSpeedMultiplier = _patrolAnimationSpeedMultiplier; // ����������: ������������ _patrolAnimationSpeedMultiplier
                break;
            case NPCStateMovement.Run:
                targetAnimSpeedValue = 1.0f;
                targetSpeedMultiplier = _chaseAnimationSpeedMultiplier; // ����������: ������������ _chaseAnimationSpeedMultiplier
                break;
        }

        // ������� ������� ��� ��������� "speed" (Idle/Walk/Run) 
        float currentAnimSpeed = _animator.GetFloat(SPEED_ANIM_PARAM);
        float newAnimSpeed = Mathf.Lerp(currentAnimSpeed, targetAnimSpeedValue, Time.deltaTime * _animationTransitionSpeed); // ����������: ������������ _animationTransitionSpeed
        _animator.SetFloat(SPEED_ANIM_PARAM, newAnimSpeed);

        // ��������� ��������� �������� �������� 
        float currentSpeedMultiplier = _animator.GetFloat(_animationPatrolSpeedMultiplierHash);
        float newSpeedMultiplier = Mathf.Lerp(currentSpeedMultiplier, targetSpeedMultiplier, Time.deltaTime * _animationTransitionSpeed); // ����������: ������������ _animationTransitionSpeed
        _animator.SetFloat(_animationPatrolSpeedMultiplierHash, newSpeedMultiplier);
    }

    private void OnDrawGizmos()
    {
        if (_patrolPoints != null) // ����������: ������������ _patrolPoints
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < _patrolPoints.Length; i++) // ����������: ������������ _patrolPoints
            {
                if (_patrolPoints[i] == null) continue; // ����������: ������������ _patrolPoints
                Gizmos.DrawWireSphere(_patrolPoints[i].position, 0.5f); // ����������: ������������ _patrolPoints
                if (_patrolPoints.Length > 1) // ����������: ������������ _patrolPoints
                {
                    var next = (i + 1) % _patrolPoints.Length; // ����������: ������������ _patrolPoints
                    Gizmos.DrawLine(_patrolPoints[i].position, _patrolPoints[next].position); // ����������: ������������ _patrolPoints
                }
            }
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius); // ����������: ������������ _detectionRadius
    }

    private void OnDrawGizmosSelected()
    {
        if (_agent == null) _agent = GetComponent<NavMeshAgent>();
        if (_agent == null) return;

        Vector3 origin = transform.position + Vector3.up * 1.5f;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, _detectionRadius); // ����������: ������������ _detectionRadius

        Gizmos.color = new Color(1f, 1f, 0f, 0.5f);

        int segments = 30;
        float angleStep = _fieldOfViewAngle / segments; // ����������: ������������ _fieldOfViewAngle
        Vector3 prevPoint = origin + Quaternion.Euler(0, -_fieldOfViewAngle / 2f, 0) * transform.forward * _detectionRadius; // ����������: ������������ _fieldOfViewAngle, _detectionRadius

        for (int i = 1; i <= segments; i++)
        {
            float currentAngle = -_fieldOfViewAngle / 2f + angleStep * i; // ����������: ������������ _fieldOfViewAngle
            Vector3 nextPoint = origin + Quaternion.Euler(0, currentAngle, 0) * transform.forward * _detectionRadius; // ����������: ������������ _detectionRadius

            Gizmos.DrawLine(origin, nextPoint);
            Gizmos.DrawLine(prevPoint, nextPoint);

            prevPoint = nextPoint;
        }
    }
}