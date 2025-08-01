using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DoorOpenClose : MonoBehaviour
{
    private Animator _animator;
    private readonly int _animatorSpeedHash = Animator.StringToHash("speed");

    private void Awake()
    {
        // Получаем компонент Animator на том же игровом объекте
        _animator = GetComponent<Animator>();

        // Добавлена проверка на наличие Animator
        if (_animator == null)
        {
            Debug.LogError($"DoorOpenClose: Компонент Animator не найден на объекте {gameObject.name}. Скрипт не будет работать корректно.", this);
            enabled = false; // Отключаем скрипт, если Animator не найден
        }
    }

    // Метод для открытия двери
    public void OpenDoor()
    {
        // Проверка, что _animator не равен null, перед использованием
        if (_animator != null)
        {
            // Устанавливаем скорость анимации в 1, чтобы она проигрывалась вперёд (открывалась)
            _animator.SetFloat(_animatorSpeedHash, 1f);
            Debug.Log("Дверь открывается!");
        }
    }

    // Метод для закрытия двери
    public void CloseDoor()
    {
        // Проверка, что _animator не равен null, перед использованием
        if (_animator != null)
        {
            // Устанавливаем скорость анимации в -3, чтобы она проигрывалась назад (закрывалась)
            _animator.SetFloat(_animatorSpeedHash, -3f);
            Debug.Log("Дверь закрывается!");
        }
    }
}