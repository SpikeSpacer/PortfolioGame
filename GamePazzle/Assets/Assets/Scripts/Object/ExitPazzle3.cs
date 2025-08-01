using UnityEngine;


public class ExitPazzle3 : MonoBehaviour
{
    [SerializeField] private RotaryMechanism _rotaryMechanism;

    [Header("��������� �������� ������")]
    [Tooltip("��������� ��������� ������� ������� �� Y.")]
    [SerializeField] private float _initialLocalYPosition;

    [Tooltip("�� ������� �������� �������� ��������� ������������� 1 ������� ��������� ������.")]
    [SerializeField] private float _rotationToMovementRatio = 10f;

    [Tooltip("������������ ��������� ������� �� Y (������������� ��������, ��������, -5f).")]
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
        if (_rotaryMechanism == null) return; // �������, ���� �������� �� ��������

        // �������� ������� �������� �������� �� RotaryMechanism
        // ������ �� ���������� public float currentRotationY ��������, ��� �� �������
        float currentMechanismValue = _rotaryMechanism.currentRotationY;

        // ������������ �������� �� Y.
        // �� �����, ����� ������ ���������, ����� currentMechanismValue ���������� �������������.
        float yOffset = 0f;
        if (currentMechanismValue < 0) // �������� ������ ���� �������� �������������
        {
            // ��������, ���� _valueToMovementRatio = 10:
            // currentMechanismValue = -10 -> yOffset = -1
            // currentMechanismValue = -20 -> yOffset = -2
            yOffset = currentMechanismValue / _rotationToMovementRatio;
        }

        // ������������ ��������, ����� ������ �� ��������� ���� _maxLocalYOffset.
        // Mathf.Max ������������, ������ ��� _maxLocalYOffset �����������;
        // �� ����� �������� "�������� �������������" (��������� � 0) �� ���� ��������.
        yOffset = Mathf.Max(yOffset, _maxLocalYOffset);

        // ��������� ������������ �������� � ��������� Y-������� �������.
        // �� ������� ����� Vector3, ����� �������� ������ Y-����������, �������� X � Z.
        Vector3 newLocalPosition = transform.localPosition;
        newLocalPosition.y = _initialLocalYPosition + yOffset;
        transform.localPosition = newLocalPosition;
    }
}
