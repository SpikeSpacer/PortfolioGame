using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DoorOpenClose : MonoBehaviour
{
    private Animator _animator;
    private readonly int _animatorSpeedHash = Animator.StringToHash("speed");

    private void Awake()
    {
        // �������� ��������� Animator �� ��� �� ������� �������
        _animator = GetComponent<Animator>();

        // ��������� �������� �� ������� Animator
        if (_animator == null)
        {
            Debug.LogError($"DoorOpenClose: ��������� Animator �� ������ �� ������� {gameObject.name}. ������ �� ����� �������� ���������.", this);
            enabled = false; // ��������� ������, ���� Animator �� ������
        }
    }

    // ����� ��� �������� �����
    public void OpenDoor()
    {
        // ��������, ��� _animator �� ����� null, ����� ��������������
        if (_animator != null)
        {
            // ������������� �������� �������� � 1, ����� ��� ������������� ����� (�����������)
            _animator.SetFloat(_animatorSpeedHash, 1f);
            Debug.Log("����� �����������!");
        }
    }

    // ����� ��� �������� �����
    public void CloseDoor()
    {
        // ��������, ��� _animator �� ����� null, ����� ��������������
        if (_animator != null)
        {
            // ������������� �������� �������� � -3, ����� ��� ������������� ����� (�����������)
            _animator.SetFloat(_animatorSpeedHash, -3f);
            Debug.Log("����� �����������!");
        }
    }
}