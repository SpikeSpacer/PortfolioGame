using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [Header("��������� �����������")]
    [Tooltip("������ �� ������ SpawnPoint � �����.")]
    [SerializeField] private Transform spawnPoint1;
    [SerializeField] private Transform spawnPoint2;

    [Tooltip("����������� ������ (Y-����������), ��� ������� ���� ������� ����� ����� ���������.")]
    [SerializeField] private float fallThresholdY = -10f; // ��������, -10 ������ �� Y

    [Tooltip("�������� � �������� ����� ������������ ������ ����� �������.")]
    [SerializeField] private float respawnDelay = 1f; // �������� ����� �������������

    public bool _spawnpoint_1 = true;
    public bool _spawnpoint_2 = false;

    private Rigidbody _playerRb; // ������ �� Rigidbody ������ (���� ����)

    private void Awake()
    {
        _playerRb = GetComponent<Rigidbody>();

        if (spawnPoint1 == null)
        {
            Debug.LogError("PlayerRespawn: SpawnPoint �� ��������! ����������, ���������� ������ SpawnPoint � ���������.", this);
            enabled = false; // ��������� ������, ���� ��� SpawnPoint
        }
    }

    private void Update()
    {
        // ��������� Y-���������� ������
        if (transform.position.y < fallThresholdY)
        {
            // ���� ����� ���� ���� ������, �������� ������� �����������
            RespawnPlayer();
        }
    }

    private void RespawnPlayer()
    {
        // ���� ����� ��� ��������� � �������� �����������, �� ��������� ��� �����
        // ����� ������������ ����, ���� ����� ����� ������� ����������.
        // ��� �������� ����� �� ���������� �� ��, ��� Update ���������� �����.

        // ����� �������� ��������� �������� ����� �������� �������������
        Invoke("PerformRespawn", respawnDelay); // �������� ����� PerformRespawn ����� respawnDelay ������

        // �����������: ����� �������� ��������� ���������� ������� ��� �������� ����� ������
        // � ���� ������� ������� ������ ������� ���
        Debug.Log("����� ����! ���������� � ����� ������.");

        // ��������� ���� ������ ��������, ����� �� �������� Invoke() ��������� ���
        // ���� PerformRespawn �� ������ ������.
        enabled = false;
    }

    private void PerformRespawn()
    {
        if (_playerRb != null)
        {
            _playerRb.linearVelocity = Vector3.zero;
            _playerRb.angularVelocity = Vector3.zero;
        }

        if (_spawnpoint_1)
        {
            transform.position = spawnPoint1.position;
            transform.rotation = spawnPoint1.rotation;
            enabled = true;
        }
        if (_spawnpoint_2)
        {
            transform.position = spawnPoint2.position;
            transform.rotation = spawnPoint2.rotation;

            enabled = true;
        }
    }
}