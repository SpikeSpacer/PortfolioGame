using UnityEngine;

public class RotaryMechanism : MonoBehaviour
{
    [Header("Настройки Механизма")]
    [Tooltip("Скорость вращения механизма. Чем выше значение, тем быстрее вращается.")]
    [SerializeField] private float _rotationSpeed = 50f; // Градусы в секунду на единицу движения толкаемого объекта

    [Tooltip("Диапазон вращения по оси Y (минимальный угол).")]
    [SerializeField] private float minRotationY = -90f; // Например, -90 градусов
    [Tooltip("Диапазон вращения по оси Y (максимальный угол).")]
    [SerializeField] private float maxRotationY = 90f;  // Например, 90 градусов

    public float currentRotationY; // Текущий угол вращения по Y
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
