using UnityEngine;


public class ExitPazzle3 : MonoBehaviour
{
    [SerializeField] private RotaryMechanism _rotaryMechanism;

    [Header("Настройки движения выхода")]
    [Tooltip("Начальная локальная позиция объекта по Y.")]
    [SerializeField] private float _initialLocalYPosition;

    [Tooltip("На сколько градусов вращения механизма соответствует 1 единице опускания выхода.")]
    [SerializeField] private float _rotationToMovementRatio = 10f;

    [Tooltip("Максимальное опускание объекта по Y (отрицательное значение, например, -5f).")]
    [SerializeField] private float _maxLocalYOffset = -5f;


    private void Awake()
    {
        if (_rotaryMechanism == null)
        {
            Debug.Log("ExitPazzle3: RotaryMechanism reference is missing! Please assign it in the Inspector or ensure it's in the hierarchy.", this);
        }
        _initialLocalYPosition = transform.localPosition.y;
    }

    private void Update()
    {
        MoveExitDown();
    }

    private void MoveExitDown()
    {
        if (_rotaryMechanism == null) return; // Выходим, если механизм не назначен

        // Получаем текущее числовое значение из RotaryMechanism
        // Теперь мы используем public float currentRotationY напрямую, как вы указали
        float currentMechanismValue = _rotaryMechanism.currentRotationY;

        // Рассчитываем смещение по Y.
        // Мы хотим, чтобы объект опускался, когда currentMechanismValue становится отрицательным.
        float yOffset = 0f;
        if (currentMechanismValue < 0) // Опускаем только если значение отрицательное
        {
            // Например, если _valueToMovementRatio = 10:
            // currentMechanismValue = -10 -> yOffset = -1
            // currentMechanismValue = -20 -> yOffset = -2
            yOffset = currentMechanismValue / _rotationToMovementRatio;
        }

        // Ограничиваем смещение, чтобы объект не опустился ниже _maxLocalYOffset.
        // Mathf.Max используется, потому что _maxLocalYOffset отрицателен;
        // мы хотим получить "наименее отрицательное" (ближайшее к 0) из двух значений.
        yOffset = Mathf.Max(yOffset, _maxLocalYOffset);

        // Применяем рассчитанное смещение к локальной Y-позиции объекта.
        // Мы создаем новый Vector3, чтобы изменить только Y-компоненту, сохраняя X и Z.
        Vector3 newLocalPosition = transform.localPosition;
        newLocalPosition.y = _initialLocalYPosition + yOffset;
        transform.localPosition = newLocalPosition;
    }
}
