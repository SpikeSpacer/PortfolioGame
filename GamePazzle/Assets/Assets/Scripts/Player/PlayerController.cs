// --- PlayerController.cs ---
using System.Collections; // Используется для корутин, например, в SmoothlyChangeIKWeight
using TMPro;
using UnityEngine;
using UnityEngine.Animations.Rigging; // Используется для TwoBoneIKConstraint
using UnityEngine.InputSystem; // Используется для новой системы ввода Input System

// Атрибуты, гарантирующие наличие необходимых компонентов на GameObject
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    // ====================================================================================================
    // 1. ПОЛЯ И СВОЙСТВА (FIELDS AND PROPERTIES)
    //    Здесь объявляются все переменные и ссылки на компоненты, используемые в скрипте.
    // ====================================================================================================

    [Header("Camera Reference")]
    [SerializeField] private Transform _cameraTransform; // Ссылка на Transform камеры

    [Header("Movement Settings")]
    [SerializeField] private float _walkSpeed = 2.0f;     // Скорость ходьбы игрока
    [SerializeField] private float _runSpeed = 5.0f;      // Скорость бега игрока
    [SerializeField] private float _rotationSpeed = 10f;  // Скорость поворота игрока

    [Header("Interaction Settings")]
    [SerializeField] private float _interactionDistance = 2.0f; // Максимальная дистанция для Raycast взаимодействия
    [SerializeField] private LayerMask _interactableLayer;     // Слой объектов, с которыми можно взаимодействовать

    private highlighting _currentHighlighter;

    // Внутренние ссылки на компоненты GameObject
    private Rigidbody _rb;
    private Animator _animator;
    private PlayerInput _playerInput;
    private readonly int _animatorSpeedHash = Animator.StringToHash("speed"); // Хэш для параметра "speed" в аниматоре

    // Переменные для обработки ввода и управления движением
    private Vector3 _currentInputDirection;  // Направление ввода от игрока (WASD)
    private Vector2 _currentLookInput;       // Ввод от мыши/геймпада для обзора
    private Vector3 _movementVector;         // Итоговый вектор движения в мировых координатах
    private bool _runInputHeld = false;      // Флаг: зажата ли кнопка бега
    private bool _isMovementEnabled = true;  // Флаг: разрешено ли игроку двигаться
    private bool _isStandardAnimationEnabled = true; // Флаг: разрешены ли стандартные анимации движения

    // Переменные для системы взаимодействия
    private IInteractable _focusedInteractable; // Объект, на который игрок смотрит в данный момент
    private IInteractable _activeInteractable;  // Объект, с которым игрок активно взаимодействует (для длительных действий)

    // Настройки для Inverse Kinematics (IK)
    [SerializeField] private TwoBoneIKConstraint _rightHandIKForLever; // Ссылка на IK-констрейнт для правой руки
    [SerializeField] private float _ikTransitionSpeed = 5.0f;       // Скорость изменения веса IK
    private Coroutine _ikTransitionCoroutine; // Ссылка на запущенную корутину IK перехода


    // ====================================================================================================
    // 2. ПУБЛИЧНЫЙ API (PUBLIC API)
    //    Методы, которые могут быть вызваны из других скриптов для управления игроком.
    // ====================================================================================================

    public Rigidbody PlayerRigidbody => _rb;          // Предоставляет доступ к Rigidbody игрока
    public Vector3 WorldMovementVector => _movementVector; // Предоставляет текущий вектор движения в мировых координатах
    public Vector3 RawInputDirection => _currentInputDirection; // Предоставляет сырое направление ввода

    /// <summary>
    /// Включает или отключает управление движением и стандартные анимации игрока.
    /// </summary>
    /// <param name="enabled">True для включения, False для отключения.</param>
    public void SetMovementAndAnimationControl(bool enabled)
    {
        _isMovementEnabled = enabled;
        _isStandardAnimationEnabled = enabled;
        if (!enabled)
        {
            _rb.linearVelocity = Vector3.zero; // Обнуляем скорость игрока при отключении
            _animator.SetFloat(_animatorSpeedHash, 0); // Сбрасываем параметр скорости анимации
        }
    }

    /// <summary>
    /// Включает или отключает только стандартные анимации движения.
    /// </summary>
    /// <param name="enabled">True для включения, False для отключения.</param>
    public void EnableStandardAnimations(bool enabled)
    {
        _isStandardAnimationEnabled = enabled;
    }

    /// <summary>
    /// Прикрепляет игрока к указанному родительскому Transform.
    /// Используется для взаимодействия, где игрок должен занять определенную позицию.
    /// </summary>
    /// <param name="parent">Новый родительский Transform.</param>
    /// <param name="localPosition">Локальная позиция относительно нового родителя.</param>
    /// <param name="localRotation">Локальное вращение относительно нового родителя.</param>
    public void AttachTo(Transform parent, Vector3 localPosition, Quaternion localRotation)
    {
        SetMovementAndAnimationControl(false); // Отключаем движение
        transform.SetParent(parent);          // Устанавливаем нового родителя
        transform.localPosition = localPosition; // Сбрасываем локальную позицию
        transform.localRotation = localRotation; // Сбрасываем локальное вращение
        _rb.isKinematic = true;               // Делаем Rigidbody кинематическим (управляется трансформацией)
    }

    /// <summary>
    /// Открепляет игрока от текущего родительского Transform.
    /// </summary>
    public void Detach()
    {
        transform.SetParent(null);         // Сбрасываем родителя
        _rb.isKinematic = false;           // Возвращаем Rigidbody в режим физики
        SetMovementAndAnimationControl(true); // Включаем движение
    }

    /// <summary>
    /// Устанавливает значение float-параметра в Animator.
    /// </summary>
    /// <param name="hash">Хэш параметра Animator (полученный через Animator.StringToHash).</param>
    /// <param name="value">Целевое значение.</param>
    /// <param name="dampTime">Время демпфирования для плавного перехода.</param>
    public void SetAnimatorFloat(int hash, float value, float dampTime = 0.1f)
    {
        _animator.SetFloat(hash, value, dampTime, Time.deltaTime);
    }

    /// <summary>
    /// Плавно изменяет вес (влияние) Inverse Kinematics для правой руки.
    /// </summary>
    /// <param name="targetWeight">Целевой вес IK (от 0 до 1).</param>
    public void SetRightHandIKWeightSmoothly(float targetWeight)
    {
        if (_rightHandIKForLever == null) return; // Проверка на null

        if (_ikTransitionCoroutine != null)
        {
            StopCoroutine(_ikTransitionCoroutine); // Останавливаем предыдущую корутину, если есть
        }
        _ikTransitionCoroutine = StartCoroutine(SmoothlyChangeIKWeight(targetWeight)); // Запускаем новую корутину
    }

    // ====================================================================================================
    // 3. МЕТОДЫ ЖИЗНЕННОГО ЦИКЛА UNITY (UNITY LIFECYCLE METHODS)
    //    Вызываются Unity в определенные моменты жизни объекта.
    // ====================================================================================================

    private void Awake()
    {
        // Получаем ссылки на необходимые компоненты
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _playerInput = GetComponent<PlayerInput>();

        _rb.freezeRotation = true; // Замораживаем вращение Rigidbody, чтобы игрок не переворачивался

        // Инициализация ссылки на камеру, если не назначена вручную
        if (_cameraTransform == null && Camera.main != null)
        {
            _cameraTransform = Camera.main.transform;
        }
    }

    private void OnEnable()
    {
        // Подписываемся на события ввода при активации скрипта
        _playerInput.OnMovePerformed += OnMovePerformedHandler;
        _playerInput.OnMoveCanceled += OnMoveCanceledHandler;
        _playerInput.OnRunPerformed += OnRunPerformedHandler;
        _playerInput.OnRunCanceled += OnRunCanceledHandler;
        _playerInput.OnInteractePerformed += OnInteractePerformedHandler;
    }

    private void OnDisable()
    {
        // Отписываемся от событий ввода при деактивации скрипта, чтобы избежать утечек памяти
        _playerInput.OnMovePerformed -= OnMovePerformedHandler;
        _playerInput.OnMoveCanceled -= OnMoveCanceledHandler;
        _playerInput.OnRunPerformed -= OnRunPerformedHandler;
        _playerInput.OnRunCanceled -= OnRunCanceledHandler;
        _playerInput.OnInteractePerformed -= OnInteractePerformedHandler;
    }

    private void Update()
    {
        // Если игрок активно взаимодействует с объектом, обновляем это взаимодействие
        if (_activeInteractable != null)
        {
            _activeInteractable.UpdateInteraction(this);
        }
        else
        {
            // Иначе, постоянно проверяем, на какой интерактивный объект смотрит игрок
            CheckForInteractableObject();
        }

        // Обновляем анимации движения, если они разрешены
        if (_isStandardAnimationEnabled)
        {
            UpdateAnimations();
        }
    }

    private void FixedUpdate()
    {
        // Физические расчеты движения выполняются в FixedUpdate для стабильности
        if (!_isMovementEnabled) return; // Выход, если движение отключено

        CalculateMovementVector(); // Вычисляем желаемый вектор движения
        MovePlayer();             // Перемещаем Rigidbody игрока
        RotatePlayer();           // Поворачиваем игрока
    }


    // ====================================================================================================
    // 4. ОБРАБОТЧИКИ ВВОДА (INPUT HANDLERS)
    //    Методы, вызываемые системой ввода при получении команд от игрока.
    // ====================================================================================================

    private void OnMovePerformedHandler(Vector2 input) => _currentInputDirection = new Vector3(input.x, 0, input.y);
    private void OnMoveCanceledHandler() => _currentInputDirection = Vector3.zero;
    private void OnRunPerformedHandler() => _runInputHeld = true;
    private void OnRunCanceledHandler() => _runInputHeld = false;


    /// <summary>
    /// Обработчик нажатия кнопки взаимодействия.
    /// </summary>
    private void OnInteractePerformedHandler()
    {
        // Если игрок уже выполняет длительное взаимодействие, завершаем его.
        if (_activeInteractable != null)
        {
            _activeInteractable.EndInteraction(this);
            _activeInteractable = null;
        }
        // Если игрок смотрит на новый объект, начинаем взаимодействие.
        else if (_focusedInteractable != null)
        {
            // Проверяем тип взаимодействия через новое свойство IsContinuous
            if (_focusedInteractable.IsContinuous)
            {
                // ДЛИТЕЛЬНОЕ ВЗАИМОДЕЙСТВИЕ (например, активация рычага)
                // "Прикрепляем" игрока к объекту, сохраняя его в _activeInteractable.
                _activeInteractable = _focusedInteractable;
                _activeInteractable.BeginInteraction(this);
            }
            else
            {
                // МГНОВЕННОЕ ВЗАИМОДЕЙСТВИЕ (например, однократное нажатие)
                // Просто вызываем действие, но НЕ сохраняем объект в _activeInteractable.
                _focusedInteractable.BeginInteraction(this);
                // Игрок остается свободным.
            }
        }
    }


    // ====================================================================================================
    // 5. ОСНОВНАЯ ЛОГИКА (CORE LOGIC)
    //    Главные функции, управляющие поведением игрока.
    // ====================================================================================================

    /// <summary>
    /// Выполняет Raycast для обнаружения интерактивных объектов в поле зрения игрока.
    /// </summary>
    private void CheckForInteractableObject()
    {
        _focusedInteractable = null; // Сбрасываем текущий сфокусированный объект
        highlighting newHighlightingScriptInFocus = null;

        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f; // Начало луча (примерно на уровне глаз игрока)

        // Выполняем Raycast
        if (Physics.Raycast(rayOrigin, transform.forward, out RaycastHit hit, _interactionDistance, _interactableLayer))
        {
            // Если Raycast попал в объект, пытаемся получить компонент IInteractable
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                _focusedInteractable = interactable; // Устанавливаем сфокусированный объект

                if (hit.collider.TryGetComponent(out highlighting highlightScript))
                {
                    newHighlightingScriptInFocus = highlightScript;
                }
                else
                {
                    Debug.Log($"На объекте {hit.collider.gameObject.name} нет highlightScript");
                }
            }
        }
        if(newHighlightingScriptInFocus != _currentHighlighter)
        {
            _currentHighlighter?.DisableHighlighting();
            _currentHighlighter = newHighlightingScriptInFocus;
            _currentHighlighter?.EnableHighlighting();
        }

        
        // Визуализация Raycast для отладки в редакторе
        Debug.DrawRay(rayOrigin, transform.forward * _interactionDistance, _focusedInteractable != null ? Color.green : Color.red);
    }

    /// <summary>
    /// Вычисляет итоговый вектор движения игрока на основе ввода и ориентации камеры.
    /// </summary>
    private void CalculateMovementVector()
    {
        // Специальная логика движения, если игрок активно взаимодействует с объектом
        if (_activeInteractable != null)
        {
            Vector3 localInput = new Vector3(_currentInputDirection.x, 0, _currentInputDirection.z);
            if (Mathf.Approximately(localInput.z, 0f))
                localInput.x = 0f; // Если нет движения вперёд/назад — обнуляем X (запрещаем чисто влево/вправо)
            _movementVector = (transform.forward * localInput.z + transform.right * localInput.x).normalized;
            return;
        }

        // Если камера не назначена, движение относительно мировых осей
        if (_cameraTransform == null)
        {
            _movementVector = _currentInputDirection.normalized;
            return;
        }

        // Получаем векторы "вперед" и "вправо" от камеры, но без вертикальной составляющей
        Vector3 camF = _cameraTransform.forward;
        camF.y = 0;
        Vector3 camR = _cameraTransform.right;
        camR.y = 0;

        // Рассчитываем итоговый вектор движения относительно камеры
        _movementVector = (camF.normalized * _currentInputDirection.z + camR.normalized * _currentInputDirection.x).normalized;
    }

    /// <summary>
    /// Перемещает Rigidbody игрока с учетом текущей скорости и вектора движения.
    /// </summary>
    private void MovePlayer()
    {
        bool isMoving = _movementVector.sqrMagnitude > 0.01f; // Проверяем, движется ли игрок
        float currentSpeed = 0f;
        if (isMoving)
        {
            // Определяем текущую скорость (ходьба или бег)
            bool canRun = _runInputHeld && (_activeInteractable == null); // Бежать можно только если нет активного взаимодействия
            currentSpeed = canRun ? _runSpeed : _walkSpeed;
        }

        Vector3 velocity = _movementVector * currentSpeed; // Вычисляем желаемую горизонтальную скорость
        velocity.y = _rb.linearVelocity.y;                // Сохраняем вертикальную скорость (гравитацию)
        _rb.linearVelocity = velocity;                    // Применяем скорость к Rigidbody
    }

    /// <summary>
    /// Плавно поворачивает персонажа в сторону движения или в зависимости от обзора.
    /// </summary>
    private void RotatePlayer()
    {
        // Если игрок активно взаимодействует с объектом, он не должен вращаться свободно
        if (_activeInteractable != null)
        {
            // Здесь может быть логика для принудительного поворота игрока к объекту взаимодействия
            return;
        }

        // Обычное поведение: поворот в сторону движения
        if (_movementVector.sqrMagnitude > 0.01f) // Если игрок движется
        {
            Quaternion targetRotation = Quaternion.LookRotation(_movementVector); // Целевое вращение в сторону движения
            // Плавно поворачиваем игрока
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * _rotationSpeed);
        }

        // _currentLookInput = Vector2.zero; // <-- Если _currentLookInput используется для поворота игрока, а не камеры,
        //                                     то его сброс здесь может быть уместен.
    }

    /// <summary>
    /// Обновляет параметры аниматора в зависимости от скорости и состояния движения.
    /// </summary>
    private void UpdateAnimations()
    {
        float speedValue = 0f;
        if (_movementVector.sqrMagnitude > 0.01f) // Если есть движение
        {
            speedValue = _runInputHeld ? 1.0f : 0.5f; // Устанавливаем скорость для бега (1.0) или ходьбы (0.5)
        }
        _animator.SetFloat(_animatorSpeedHash, speedValue, 0.1f, Time.deltaTime); // Плавно обновляем параметр speed в аниматоре
    }


    // ====================================================================================================
    // 6. ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ (HELPER METHODS)
    //    Корутины и другие служебные методы.
    // ====================================================================================================

    /// <summary>
    /// Корутина для плавного изменения веса IK-констрейнта.
    /// </summary>
    /// <param name="targetWeight">Целевой вес IK.</param>
    private IEnumerator SmoothlyChangeIKWeight(float targetWeight)
    {
        float currentWeight = _rightHandIKForLever.weight; // Начальный вес IK
        float timer = 0f;

        while (timer < 1f)
        {
            timer += Time.deltaTime * _ikTransitionSpeed; // Увеличиваем таймер
            // Плавно интерполируем вес IK
            _rightHandIKForLever.weight = Mathf.Lerp(currentWeight, targetWeight, timer);
            yield return null; // Ждем следующего кадра
        }
        _rightHandIKForLever.weight = targetWeight; // Убеждаемся, что вес точно равен целевому значению
    }
}