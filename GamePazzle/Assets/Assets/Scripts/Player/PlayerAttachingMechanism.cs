using UnityEngine;



public class PlayerAttachingMechanism : MonoBehaviour, IInteractable // ��������� �������� IInteractable

{

� � // -------------------------------------------------------------------

� � // 1. ���� (Fields)

� � // -------------------------------------------------------------------



� � [Header("��������� ��������")]

    [Tooltip("������ �� ������ � RotaryMechanism, ������� ����� ���������.")]

    [SerializeField] private RotaryMechanism _connectedRotaryMechanism;



    [Header("��������� ������������")]

    [Tooltip("����� (��������� ����������), ���� ����� ������ '������' ��� ��������������.")]

    [SerializeField] private Vector3 _playerAttachOffset = new Vector3(0, 0, -0.5f);

    [Tooltip("������� ������ (��������� ����������) ��� ��������������.")]

    [SerializeField] private Quaternion _playerAttachLocalRotation = Quaternion.identity;



    private Animator _playerAnimatorCache;

    private readonly int _animatorSpeedHash = Animator.StringToHash("speed");



    // -------------------------------------------------------------------

    // 2. IINTERACTABLE IMPLEMENTATION

    // -------------------------------------------------------------------

    public string InteractionPrompt => "������";
    public string InfoText => string.Empty;
    public bool IsContinuous => true; // ��� ���������� ��������������
    public bool IsInfoPanelActive { get; set; }

� � /// <summary>

� � /// ������, ������� ����������� ��� ������ ��������������.

� � /// </summary>

� � public void BeginInteraction(PlayerController player)

    {

        _playerAnimatorCache = player.GetComponent<Animator>();

� � � � // �������� ��������� ����� ������, ����� �� ��� ���� "���������" � ���.

� � � � // �� �������� ���, ���� ������ ����� ������ � ��� �����������.

� � � � player.AttachTo(this.transform, _playerAttachOffset, _playerAttachLocalRotation);

    }



� � /// <summary>

� � /// ������, ������� ����������� ������ ���� �� ����� ��������������.

� � /// </summary>

� � public void UpdateInteraction(PlayerController player)

    {

        if (_connectedRotaryMechanism != null)

        {

� � � � � � // �������� "�����" ���� �� ��� Z (W/S -> 1/-1)

� � � � � � // ����������� ����: W ����� ������������� (+1), S ������������� (-1)

� � � � � � float verticalInput = player.RawInputDirection.z * -1f;



� � � � � � // �������� ��� �������� � ������ RotaryMechanism

� � � � � � _connectedRotaryMechanism.RotateBy(verticalInput);



� � � � � � // --- ���������� ��������� Blend Tree ---

� � � � � � if (_playerAnimatorCache != null)

            {

� � � � � � � � // ���� ���� �������� ���� (W/S �������)

� � � � � � � � if (Mathf.Abs(verticalInput) > 0.1f)

                {

� � � � � � � � � � // ������������ ����, ����� �� �������������� ������ ��������� Blend Tree (-2 �� 2)

� � � � � � � � � � // verticalInput ��� -1 ��� 1, �������� �� 2, ����� �������� -2 ��� 2

� � � � � � � � � � float blendValue = verticalInput * -2f;

                    _playerAnimatorCache.SetFloat(_animatorSpeedHash, blendValue);

                }

                else // ���� ����� ��� ��� �� ������� ���

� � � � � � � � {

� � � � � � � � � � // ���������� Blend Tree �������� � 0, ����� �������� ������������

� � � � � � � � � � _playerAnimatorCache.SetFloat(_animatorSpeedHash, 0f);

                }



            }

        }

    }



� � /// <summary>

� � /// ������, ������� ����������� ��� ���������� ��������������.

� � /// </summary>

� � public void EndInteraction(PlayerController player)

    {

� � � � // �������� ��������� ����� ������, ����� �� ��� ���� "��������".

� � � � player.Detach();

    }



    private void OnDrawGizmosSelected()

    {

� � � � // ���������, ��� _connectedRotaryMechanism ��������, ����� Gizmo ��� �����

� � � � // ������ ��� ������ ������� � ���������.

� � � � if (_connectedRotaryMechanism != null)

        {

� � � � � � // ��������� ���������� �������, ���� ����� ��������� �����

� � � � � � // transform.TransformPoint() ����������� ��������� ���������� � ����������

� � � � � � Vector3 globalAttachPosition = transform.TransformPoint(_playerAttachOffset);



� � � � � � // ��������� ���������� ��������, ������� ����� ��������� � ������

� � � � � � // �������� �������� �������� �� ��������� �������� ��������

� � � � � � Quaternion globalAttachRotation = transform.rotation * _playerAttachLocalRotation;



            Gizmos.color = Color.green; // ���� ��� ����� �������

� � � � � � Gizmos.DrawWireSphere(globalAttachPosition, 0.15f); // ��������� �����, ������������ ����� ������



� � � � � � // ������ ���, ����� �������� ���������� ������:

� � � � � � // ����� �����: ����������� ������ (Z) ������

� � � � � � // ������� �����: ����������� ������ (X) ������

� � � � � � // ������� �����: ����������� ����� (Y) ������

� � � � � � Gizmos.color = Color.blue;

            Gizmos.DrawRay(globalAttachPosition, globalAttachRotation * Vector3.forward * 0.5f); // "���" ������

� � � � � � Gizmos.color = Color.red;

            Gizmos.DrawRay(globalAttachPosition, globalAttachRotation * Vector3.right * 0.3f);� // "������ �����" ������

� � � � � � Gizmos.color = Color.green;

            Gizmos.DrawRay(globalAttachPosition, globalAttachRotation * Vector3.up * 0.3f);� � �// "������" ������

� � � � }

    }

}