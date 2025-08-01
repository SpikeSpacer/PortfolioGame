using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Управляет вращением камеры независимо от горизонтального вращения игрока.
/// Камера может быть дочерним объектом игрока (для следования позиции) или независимой.
/// Горизонтальное вращение мыши применяется напрямую к камере, а не к playerBody.
/// </summary>
public class CameraLook : MonoBehaviour
{
    // -------------------------------------------------------------------
    // 1. ПОЛЯ И СВОЙСТВА
    // -------------------------------------------------------------------

    [Header("Настройки чувствительности")]
    [Tooltip("Общая чувствительность мыши.")]
    [Range(0.1f, 10f)]
    [SerializeField] private float mouseSensitivity = 1f;

    [Header("Настройки плавности")]
    [Tooltip("Скорость, с которой камера догоняет целевое вращение. Больше значение = быстрее/менее плавно.")]
    [Range(5f, 50f)]
    [SerializeField] private float rotationSmoothSpeed = 15f;

    [Header("Ограничения взгляда")]
    [Tooltip("Минимальный угол наклона камеры вниз (в градусах).")]
    [Range(-90f, 0f)]
    [SerializeField] private float minVerticalAngle = -90f;
    [Tooltip("Максимальный угол наклона камеры вверх (в градусах).")]
    [Range(0f, 90f)]
    [SerializeField] private float maxVerticalAngle = 90f;

    private float _currentXRotation; // Текущий накопленный угол вращения камеры по X (вверх/вниз)
    private float _currentYRotation; // Текущий накопленный угол вращения камеры по Y (влево/вправо)
    private Vector2 _lookInputDelta; // Дельта движения мыши за кадр

    // Ссылка на ваш кастомный скрипт PlayerInput (если он обрабатывает все вводы)
    private PlayerInput _playerInputScript;

    // -------------------------------------------------------------------
    // 2. UNITY LIFECYCLE METHODS
    // -------------------------------------------------------------------

    private void Awake()
    {
        // Для независимой камеры PlayerInput может находиться где угодно.
        // Используем FindObjectOfType, чтобы найти его в сцене.
        _playerInputScript = FindFirstObjectByType<PlayerInput>();
        if (_playerInputScript == null)
        {
            Debug.LogError("CameraLook: Компонент 'PlayerInput' не найден в сцене. Убедитесь, что он присутствует.", this);
        }

        // Инициализируем текущее вращение камеры, чтобы избежать рывка при старте.
        // Берем текущие углы Эйлера и нормализуем их.
        Vector3 currentEuler = transform.eulerAngles; // Используем eulerAngles, так как камера не должна быть дочерней по вращению
        _currentXRotation = currentEuler.x;
        if (_currentXRotation > 180) _currentXRotation -= 360;
        _currentYRotation = currentEuler.y;
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        if (_playerInputScript != null)
        {
            _playerInputScript.OnLookPerformed += HandleLookPerformed;
        }
    }

    private void OnDisable()
    {
        if (_playerInputScript != null)
        {
            _playerInputScript.OnLookPerformed -= HandleLookPerformed;
        }
    }

    private void Update()
    {
        ApplyRotation();
        ToggleCursorLock();
    }

    // -------------------------------------------------------------------
    // 3. INPUT HANDLER
    // -------------------------------------------------------------------

    private void HandleLookPerformed(Vector2 lookInput)
    {
        _lookInputDelta = lookInput;
    }

    // -------------------------------------------------------------------
    // 4. CORE LOGIC
    // -------------------------------------------------------------------

    /// <summary>
    /// Вычисляет и применяет вращение только к камере.
    /// </summary>
    private void ApplyRotation()
    {
        float mouseX = _lookInputDelta.x * mouseSensitivity;
        float mouseY = _lookInputDelta.y * mouseSensitivity;

        // --- 1. Вертикальное вращение камеры (вверх/вниз) ---
        _currentXRotation -= mouseY;
        _currentXRotation = Mathf.Clamp(_currentXRotation, minVerticalAngle, maxVerticalAngle);

        // --- 2. Горизонтальное вращение камеры (влево/вправо) ---
        // Теперь горизонтальное вращение применяется напрямую к камере
        _currentYRotation += mouseX;

        // Вычисляем целевое вращение для камеры
        Quaternion targetCameraRotation = Quaternion.Euler(_currentXRotation, _currentYRotation, 0f);

        // Плавно интерполируем текущее вращение камеры к целевому.
        // Используем transform.rotation (глобальное) вместо transform.localRotation,
        // так как камера теперь вращается независимо.
        transform.rotation = Quaternion.Slerp(transform.rotation, targetCameraRotation, Time.deltaTime * rotationSmoothSpeed);

        // Сбрасываем дельту ввода мыши.
        _lookInputDelta = Vector2.zero;
    }

    /// <summary>
    /// Переключает состояние блокировки и видимости курсора по нажатию клавиши Escape.
    /// </summary>
    private void ToggleCursorLock()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
}