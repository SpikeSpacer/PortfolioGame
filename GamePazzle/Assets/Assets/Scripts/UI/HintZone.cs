using UnityEngine;
using TMPro; // ��� ������ � TextMeshPro

public class HintZone : MonoBehaviour
{
    [Header("Hint Settings")]
    [Tooltip("�����, ������� ����� ������������ ��� ����� � ��� ����.")]
    [TextArea(1, 3)] // ����� ������� ���� ������� ������������� ����� � ����������
    [SerializeField] private string _hintText = "������� [E] ��� ��������������.";

    [Header("Detection Settings")]
    [Tooltip("��� �������� �������, ������� ����� ������������ ��������� (������ 'Player').")]
    [SerializeField] private string _playerTag = "Player";

    [Header("World Space UI Settings")]
    [Tooltip("������ Canvas � TextMeshPro � ������ World Space.")]
    [SerializeField] private GameObject _worldSpaceHintUIPrefab;
    [Tooltip("������������ �������� ��������� ��� ��������.")]
    [SerializeField] private float _hintVerticalOffset = 1.5f;

    private GameObject _spawnedHintUI; // ��������� UI ��������� � ����
    private TextMeshProUGUI _spawnedHintTextMesh; // ��������� ��������� ���������
    private Camera _mainCamera; // ������ �� �������� ������

    private void Awake()
    {
        _mainCamera = Camera.main; // �������� �������� ������

        // ��������, ��� ��������� �������� ���������
        if (TryGetComponent<Collider>(out Collider col))
        {
            if (!col.isTrigger)
            {
                Debug.LogWarning("HintZone: ��������� �� ������� '" + gameObject.name + "' �� ���������� ��� �������! ��������� �� ����� �������� ���������.", this);
            }
        }
        else
        {
            Debug.LogError("HintZone: �� ������� '" + gameObject.name + "' ����������� ��������� Collider! ��������� �� ����� ��������.", this);
            enabled = false; // ��������� ������
        }

        if (_worldSpaceHintUIPrefab == null)
        {
            Debug.LogError("HintZone: ���� _worldSpaceHintUIPrefab �� ���������! ���������� ������ WorldSpaceHintCanvas.", this);
            enabled = false; // ��������� ������
        }
    }

    private void LateUpdate()
    {
        // ���� ��������� ������������, ���������� �� �������� �� ������
        if (_spawnedHintUI != null && _spawnedHintUI.activeSelf)
        {
            _spawnedHintUI.transform.LookAt(_spawnedHintUI.transform.position + _mainCamera.transform.rotation * Vector3.forward,
                                             _mainCamera.transform.rotation * Vector3.up);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_playerTag))
        {
            ShowWorldHint();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(_playerTag))
        {
            HideWorldHint();
        }
    }

    private void ShowWorldHint()
    {
        if (_spawnedHintUI == null)
        {
            // ������� ��������� �������
            _spawnedHintUI = Instantiate(_worldSpaceHintUIPrefab, transform.position + Vector3.up * _hintVerticalOffset, Quaternion.identity, transform);
            // �������� ��������� TextMeshProUGUI
            _spawnedHintTextMesh = _spawnedHintUI.GetComponentInChildren<TextMeshProUGUI>();

            if (_spawnedHintTextMesh == null)
            {
                Debug.LogError("HintZone: � ������� WorldSpaceHintUIPrefab �� ������ TextMeshProUGUI!", this);
                Destroy(_spawnedHintUI); // ����������, ���� �� ����� �����
                _spawnedHintUI = null;
                return;
            }
        }

        _spawnedHintTextMesh.text = _hintText; // ������������� �����
        _spawnedHintUI.SetActive(true); // ���������� UI

        // ��� �������� ��������� ����� ������������ CanvasGroup �� _spawnedHintUI,
        // �� ��� �������� ���, ���� ������� ���������.
    }

    private void HideWorldHint()
    {
        if (_spawnedHintUI != null)
        {
            _spawnedHintUI.SetActive(false); // ������������ UI
            // ����� ����� ���������� ������, ���� ������ ��������� ������
            // Destroy(_spawnedHintUI); 
            // _spawnedHintUI = null;
        }
    }

    private void OnDestroy()
    {
        // ������� ��� ����������� ������� HintZone
        if (_spawnedHintUI != null)
        {
            Destroy(_spawnedHintUI);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * _hintVerticalOffset, 0.1f);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * _hintVerticalOffset);
    }
}