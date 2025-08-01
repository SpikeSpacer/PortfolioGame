// PressurePlate.cs
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class PressurePlateFake : MonoBehaviour
{
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
            Debug.LogWarning($"Collider на {gameObject.name} не является триггером!");
        }

        if (_plateAnimator == null)
        {
            Debug.LogError($"Animator для плиты не назначен на {gameObject.name} или не найден!", this);
        }
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
            if (_plateAnimator != null)
            {
                _plateAnimator.SetFloat(_animatorSpeedHash, 1f); // Плита проигрывает анимацию нажатия вперёд
            }
        }
        else
        {
            // Плита отпущена: дверь закрывается, плита поднимается
            if (_plateAnimator != null)
            {
                _plateAnimator.SetFloat(_animatorSpeedHash, -1f); // Плита проигрывает анимацию отпускания назад
            }
        }
    }
}