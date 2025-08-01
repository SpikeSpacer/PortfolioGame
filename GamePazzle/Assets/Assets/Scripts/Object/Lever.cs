using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Lever : MonoBehaviour, IInteractable
{
    public string InteractionPrompt => "������";
    public string InfoText => string.Empty;
    public bool IsContinuous => true; // ��� ���������� ��������������
    public bool IsInfoPanelActive { get; set; }

    private readonly string layer = "Interactable";

    [SerializeField] private Animator _exitDoorAnimator;
    private Animator _animator;

    private readonly int _pullLeverHash = Animator.StringToHash("pullLever");
    private readonly int _exitDoorUpHash = Animator.StringToHash("exitDoorUp");

    [Header("��������� ������������")]
    [Tooltip("����� (��������� ����������), ���� ����� ������ '������' ��� ��������������.")]
    [SerializeField] private Vector3 _playerAttachOffset = new Vector3(0, 0, -0.5f);
    [Tooltip("������� ������ (��������� ����������) ��� ��������������.")]
    [SerializeField] private Quaternion _playerAttachLocalRotation = Quaternion.identity;

    private void Awake()
    {
        if (_exitDoorAnimator == null)
        {
            Debug.Log("Animator DoorExit �� ����������");
        }

        _animator = GetComponent<Animator>();

        if (gameObject.layer != LayerMask.NameToLayer(layer))
        {
            Debug.Log($"���� ������� �� �������� {layer}.");
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
        // ���������, ��� _connectedRotaryMechanism ��������, ����� Gizmo ��� �����
        // ������ ��� ������ ������� � ���������.


        // ��������� ���������� �������, ���� ����� ��������� �����
        // transform.TransformPoint() ����������� ��������� ���������� � ����������
        Vector3 globalAttachPosition = transform.TransformPoint(_playerAttachOffset);

        // ��������� ���������� ��������, ������� ����� ��������� � ������
        // �������� �������� �������� �� ��������� �������� ��������
        Quaternion globalAttachRotation = transform.rotation * _playerAttachLocalRotation;

        Gizmos.color = Color.green; // ���� ��� ����� �������
        Gizmos.DrawWireSphere(globalAttachPosition, 0.15f); // ��������� �����, ������������ ����� ������

        // ������ ���, ����� �������� ���������� ������:
        // ����� �����: ����������� ������ (Z) ������
        // ������� �����: ����������� ������ (X) ������
        // ������� �����: ����������� ����� (Y) ������
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(globalAttachPosition, globalAttachRotation * Vector3.forward * 0.5f); // "���" ������
        Gizmos.color = Color.red;
        Gizmos.DrawRay(globalAttachPosition, globalAttachRotation * Vector3.right * 0.3f);  // "������ �����" ������
        Gizmos.color = Color.green;
        Gizmos.DrawRay(globalAttachPosition, globalAttachRotation * Vector3.up * 0.3f);     // "������" ������

    }
}
