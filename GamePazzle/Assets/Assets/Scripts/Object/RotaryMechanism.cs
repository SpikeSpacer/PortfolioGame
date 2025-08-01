using UnityEngine;

public class RotaryMechanism : MonoBehaviour
{
    [Header("��������� ���������")]
    [Tooltip("�������� �������� ���������. ��� ���� ��������, ��� ������� ���������.")]
    [SerializeField] private float _rotationSpeed = 50f; // ������� � ������� �� ������� �������� ���������� �������

    [Tooltip("�������� �������� �� ��� Y (����������� ����).")]
    [SerializeField] private float minRotationY = -90f; // ��������, -90 ��������
    [Tooltip("�������� �������� �� ��� Y (������������ ����).")]
    [SerializeField] private float maxRotationY = 90f;  // ��������, 90 ��������

    public float currentRotationY; // ������� ���� �������� �� Y
    private Rigidbody _rb;

    private void Awake()
    {
        currentRotationY = transform.localEulerAngles.y;
    }

    public void RotateBy(float amount) {
        currentRotationY += amount * _rotationSpeed * Time.deltaTime;

        currentRotationY = Mathf.Clamp(currentRotationY, minRotationY, maxRotationY);

        transform.localRotation = Quaternion.Euler(0, currentRotationY, 0);
    }
}
