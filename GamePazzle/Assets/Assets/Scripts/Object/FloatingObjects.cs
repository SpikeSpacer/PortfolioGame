using System.Collections;
using UnityEngine;

public class FloatingObjects: MonoBehaviour
{
    // === ��������� �������� ===
    [SerializeField] private float speed = 2f;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _stopTime = 2f;

    // === ��������� �������� ===
    private Vector3 _pointA;
    private Vector3 _pointB;
    private Vector3 _currentTarget;
    private Coroutine _waitCoroutine;

    // === ������������� ===
    private void Awake()
    {
        _pointA = transform.position;
        _pointB = _pointA + _offset;
        _currentTarget = _pointB;
    }

    // === ���������� ������ ���� ===
    private void Update()
    {
        MoveBuild();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }

    // === �������� ������� ===
    private void MoveBuild()
    {
        transform.position = Vector3.MoveTowards(transform.position, _currentTarget, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, _currentTarget) < 0.1f && _waitCoroutine == null)
        {
            _waitCoroutine = StartCoroutine(Wait(_stopTime));
        }
    }

    // === ������������ ���� �������� ===
    private void SwitchDirection()
    {
        _currentTarget = (_currentTarget == _pointA) ? _pointB : _pointA;
    }

    // === �������� ����� ������ ����������� ===
    private IEnumerator Wait(float delay)
    {
        yield return new WaitForSeconds(delay);
        SwitchDirection();
        _waitCoroutine = null;
    }

    // === ������������ � ��������� ===
    private void OnDrawGizmos()
    {
        // ���������� ��������, ������� ����� ����������� � Awake ��� ��� �����������
        // ���� ���� �� ��������, _pointA ����� ���� Transform.position, ����� Gizmos ��������� ���������
        // ���� ���� ��������, _pointA ��� �������������.
        Vector3 gizmoPointA = transform.position; // ����������� ��������� �������
        Vector3 gizmoPointB = gizmoPointA + _offset;

        // ����� ��������� ������ ���� �� ����� ���������� (Play Mode),
        // ����� ������������ �������������� _pointA � _pointB, ���� ���� ��������.
        if (Application.isPlaying)
        {
            gizmoPointA = _pointA;
            gizmoPointB = _pointB;
        }
        // � ������ �������������� (Edit Mode) ���������� ������� �������, ����� ������ ��������
        else
        {
            gizmoPointA = transform.position;
            gizmoPointB = transform.position + _offset;
        }


        Gizmos.color = Color.green;
        Gizmos.DrawLine(gizmoPointA, gizmoPointB);

        // ������������� ����� ���������� ����� � ������
        Gizmos.DrawSphere(gizmoPointA, 0.1f);
        Gizmos.DrawSphere(gizmoPointB, 0.1f);
    }

}
