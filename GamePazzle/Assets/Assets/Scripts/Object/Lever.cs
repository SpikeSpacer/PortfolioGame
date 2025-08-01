using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Lever : MonoBehaviour, IInteractable
{
    public string InteractionPrompt => "Тянуть";
    public string InfoText => string.Empty;
    public bool IsContinuous => true; // Это длительное взаимодействие
    public bool IsInfoPanelActive { get; set; }

    private readonly string layer = "Interactable";

    [SerializeField] private Animator _exitDoorAnimator;
    private Animator _animator;

    private readonly int _pullLeverHash = Animator.StringToHash("pullLever");
    private readonly int _exitDoorUpHash = Animator.StringToHash("exitDoorUp");

    [Header("Настройки прикрепления")]
    [Tooltip("Место (локальные координаты), куда игрок должен 'встать' при взаимодействии.")]
    [SerializeField] private Vector3 _playerAttachOffset = new Vector3(0, 0, -0.5f);
    [Tooltip("Поворот игрока (локальные координаты) при взаимодействии.")]
    [SerializeField] private Quaternion _playerAttachLocalRotation = Quaternion.identity;

    private void Awake()
    {
        if (_exitDoorAnimator == null)
        {
            Debug.Log("Animator DoorExit не прикреплен");
        }

        _animator = GetComponent<Animator>();

        if (gameObject.layer != LayerMask.NameToLayer(layer))
        {
            Debug.Log($"Слой объекта не является {layer}.");
        }
    }

    public void BeginInteraction(PlayerController player)
    {
        player.AttachTo(this.transform, _playerAttachOffset, _playerAttachLocalRotation);
    }


    public void UpdateInteraction(PlayerController player)
    {
        player.SetRightHandIKWeightSmoothly(1f);

        _animator.SetTrigger(_pullLeverHash);
        _exitDoorAnimator.SetTrigger(_exitDoorUpHash);

    }

    public void EndInteraction(PlayerController player)
    {
        player.SetRightHandIKWeightSmoothly(0f);
        player.Detach();
        gameObject.layer = LayerMask.NameToLayer("Default");
    }



    private void OnDrawGizmosSelected()
    {
        // Убедитесь, что _connectedRotaryMechanism назначен, чтобы Gizmo был виден
        // только при выборе объекта в редакторе.


        // Вычисляем глобальную позицию, куда будет перемещен игрок
        // transform.TransformPoint() преобразует локальные координаты в глобальные
        Vector3 globalAttachPosition = transform.TransformPoint(_playerAttachOffset);

        // Вычисляем глобальное вращение, которое будет применено к игроку
        // Умножаем вращение родителя на локальное смещение вращения
        Quaternion globalAttachRotation = transform.rotation * _playerAttachLocalRotation;

        Gizmos.color = Color.green; // Цвет для сферы позиции
        Gizmos.DrawWireSphere(globalAttachPosition, 0.15f); // Маленькая сфера, показывающая центр игрока

        // Рисуем оси, чтобы показать ориентацию игрока:
        // Синяя линия: направление вперед (Z) игрока
        // Красная линия: направление вправо (X) игрока
        // Зеленая линия: направление вверх (Y) игрока
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(globalAttachPosition, globalAttachRotation * Vector3.forward * 0.5f); // "Нос" игрока
        Gizmos.color = Color.red;
        Gizmos.DrawRay(globalAttachPosition, globalAttachRotation * Vector3.right * 0.3f);  // "Правое плечо" игрока
        Gizmos.color = Color.green;
        Gizmos.DrawRay(globalAttachPosition, globalAttachRotation * Vector3.up * 0.3f);     // "Голова" игрока

    }
}
