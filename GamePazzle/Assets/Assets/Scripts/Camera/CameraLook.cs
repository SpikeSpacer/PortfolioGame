using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// ��������� ��������� ������ ���������� �� ��������������� �������� ������.
/// ������ ����� ���� �������� �������� ������ (��� ���������� �������) ��� �����������.
/// �������������� �������� ���� ����������� �������� � ������, � �� � playerBody.
/// </summary>
public class CameraLook : MonoBehaviour
{
    // -------------------------------------------------------------------
    // 1. ���� � ��������
    // -------------------------------------------------------------------

    [Header("��������� ����������������")]
    [Tooltip("����� ���������������� ����.")]
    [Range(0.1f, 10f)]
    [SerializeField] private float mouseSensitivity = 1f;

    [Header("��������� ���������")]
    [Tooltip("��������, � ������� ������ �������� ������� ��������. ������ �������� = �������/����� ������.")]
    [Range(5f, 50f)]
    [SerializeField] private float rotationSmoothSpeed = 15f;

    [Header("����������� �������")]
    [Tooltip("����������� ���� ������� ������ ���� (� ��������).")]
    [Range(-90f, 0f)]
    [SerializeField] private float minVerticalAngle = -90f;
    [Tooltip("������������ ���� ������� ������ ����� (� ��������).")]
    [Range(0f, 90f)]
    [SerializeField] private float maxVerticalAngle = 90f;

    private float _currentXRotation; // ������� ����������� ���� �������� ������ �� X (�����/����)
    private float _currentYRotation; // ������� ����������� ���� �������� ������ �� Y (�����/������)
    private Vector2 _lookInputDelta; // ������ �������� ���� �� ����

    // ������ �� ��� ��������� ������ PlayerInput (���� �� ������������ ��� �����)
    private PlayerInput _playerInputScript;

    // -------------------------------------------------------------------
    // 2. UNITY LIFECYCLE METHODS
    // -------------------------------------------------------------------

    private void Awake()
    {
        // ��� ����������� ������ PlayerInput ����� ���������� ��� ������.
        // ���������� FindObjectOfType, ����� ����� ��� � �����.
        _playerInputScript = FindFirstObjectByType<PlayerInput>();
        if (_playerInputScript == null)
        {
            Debug.LogError("CameraLook: ��������� 'PlayerInput' �� ������ � �����. ���������, ��� �� ������������.", this);
        }

        // �������������� ������� �������� ������, ����� �������� ����� ��� ������.
        // ����� ������� ���� ������ � ����������� ��.
        Vector3 currentEuler = transform.eulerAngles; // ���������� eulerAngles, ��� ��� ������ �� ������ ���� �������� �� ��������
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
    /// ��������� � ��������� �������� ������ � ������.
    /// </summary>
    private void ApplyRotation()
    {
        float mouseX = _lookInputDelta.x * mouseSensitivity;
        float mouseY = _lookInputDelta.y * mouseSensitivity;

        // --- 1. ������������ �������� ������ (�����/����) ---
        _currentXRotation -= mouseY;
        _currentXRotation = Mathf.Clamp(_currentXRotation, minVerticalAngle, maxVerticalAngle);

        // --- 2. �������������� �������� ������ (�����/������) ---
        // ������ �������������� �������� ����������� �������� � ������
        _currentYRotation += mouseX;

        // ��������� ������� �������� ��� ������
        Quaternion targetCameraRotation = Quaternion.Euler(_currentXRotation, _currentYRotation, 0f);

        // ������ ������������� ������� �������� ������ � ��������.
        // ���������� transform.rotation (����������) ������ transform.localRotation,
        // ��� ��� ������ ������ ��������� ����������.
        transform.rotation = Quaternion.Slerp(transform.rotation, targetCameraRotation, Time.deltaTime * rotationSmoothSpeed);

        // ���������� ������ ����� ����.
        _lookInputDelta = Vector2.zero;
    }

    /// <summary>
    /// ����������� ��������� ���������� � ��������� ������� �� ������� ������� Escape.
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