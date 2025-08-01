// PressurePlate.cs
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class PressurePlate : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private DoorOpenClose _doorToControl; // Ссылка на дверь, которую нужно контролировать

    [Header("Plate Animation Settings")]
    private Animator _plateAnimator; // Ссылка на Animator дочернего объекта плиты (VisualPlate)

    private readonly int _animatorSpeedHash = Animator.StringToHash("speed");

    // Список объектов, которые в данный момент находятся на плите и имеют Rigidbody
    private List<Rigidbody> _objectsOnPlate = new List<Rigidbody>();

    private void Awake()
    {

        _plateAnimator = GetComponent<Animator>(); 

        Collider plateCollider = GetComponent<Collider>();
        if (!plateCollider.isTrigger)
        {
            Debug.LogWarning($"Collider на {gameObject.name} не является триггером! Исправлено.", this);
            plateCollider.isTrigger = true;
        }

        if (_plateAnimator == null)
        {
            Debug.LogError($"Animator для плиты не назначен на {gameObject.name} или не найден!", this);
        }
    }

    private void OnEnable() // Вызывается при включении объекта, в том числе при старте сцены
    {
        // Очищаем список на случай, если он содержит устаревшие ссылки
        _objectsOnPlate.Clear();
        // Принудительно проверяем состояние плиты, чтобы обновить анимацию и дверь
        CheckPlateState();
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody enteredRigidbody = other.GetComponent<Rigidbody>();
        if (enteredRigidbody != null)
        {
            if (!_objectsOnPlate.Contains(enteredRigidbody))
            {
                _objectsOnPlate.Add(enteredRigidbody);
                CheckPlateState();
                Debug.Log($"Объект {other.name} с Rigidbody вошел на плиту.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody exitedRigidbody = other.GetComponent<Rigidbody>();
        if (exitedRigidbody != null)
        {
            if (_objectsOnPlate.Contains(exitedRigidbody))
            {
                // !!! ВАЖНО: Удаляем объект перед вызовом CheckPlateState()
                _objectsOnPlate.Remove(exitedRigidbody);
                CheckPlateState();
                Debug.Log($"Объект {other.name} с Rigidbody покинул плиту.");
            }
        }
    }

    private void CheckPlateState()
    {
        if (_objectsOnPlate.Count > 0)
        {
            // Плита нажата: дверь открывается, плита продавливается
            _doorToControl.OpenDoor(); // Это вызовет _animator.SetFloat(_animatorSpeedHash, 1f) в Door.cs
            if (_plateAnimator != null)
            {
                _plateAnimator.SetFloat(_animatorSpeedHash, 2f); // Плита проигрывает анимацию нажатия вперёд
            }
        }
        else
        {
            // Плита отпущена: дверь закрывается, плита поднимается
            _doorToControl.CloseDoor(); // Это вызовет _animator.SetFloat(_animatorSpeedHash, -1f) в Door.cs
            if (_plateAnimator != null)
            {
                _plateAnimator.SetFloat(_animatorSpeedHash, -2f); // Плита проигрывает анимацию отпускания назад
            }
        }
    }
}