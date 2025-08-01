// PressurePlate.cs
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class PressurePlateFake : MonoBehaviour
{
    [Header("Plate Animation Settings")]
    private Animator _plateAnimator; // ������ �� Animator ��������� ������� ����� (VisualPlate)

    private readonly int _animatorSpeedHash = Animator.StringToHash("speed");

    // ������ ��������, ������� � ������ ������ ��������� �� ����� � ����� Rigidbody
    private List<Rigidbody> _objectsOnPlate = new List<Rigidbody>();

    private void Awake()
    {

        _plateAnimator = GetComponent<Animator>(); 

        Collider plateCollider = GetComponent<Collider>();
        if (!plateCollider.isTrigger)
        {
            Debug.LogWarning($"Collider �� {gameObject.name} �� �������� ���������!");
        }

        if (_plateAnimator == null)
        {
            Debug.LogError($"Animator ��� ����� �� �������� �� {gameObject.name} ��� �� ������!", this);
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
                Debug.Log($"������ {other.name} � Rigidbody ����� �� �����.");
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
                // !!! �����: ������� ������ ����� ������� CheckPlateState()
                _objectsOnPlate.Remove(exitedRigidbody);
                CheckPlateState();
                Debug.Log($"������ {other.name} � Rigidbody ������� �����.");
            }
        }
    }

    private void CheckPlateState()
    {
        if (_objectsOnPlate.Count > 0)
        {
            // ����� ������: ����� �����������, ����� ��������������
            if (_plateAnimator != null)
            {
                _plateAnimator.SetFloat(_animatorSpeedHash, 1f); // ����� ����������� �������� ������� �����
            }
        }
        else
        {
            // ����� ��������: ����� �����������, ����� �����������
            if (_plateAnimator != null)
            {
                _plateAnimator.SetFloat(_animatorSpeedHash, -1f); // ����� ����������� �������� ���������� �����
            }
        }
    }
}