using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FallingPlate : MonoBehaviour
{
    [Header("��������� �������")]
    [Tooltip("����� � ��������, ������� ����� ������ ���������� ������ �� ����� ��� � �������.")]
    //private float _timeRequiredToFall = 0.1f;

    private Rigidbody _rb;
    private bool _hasFallen = false; // ����, ����� ����� ������ ������ ���� ���
    private Coroutine _fallCoroutine; // ������ �� ���������� ��������, ����� ����� ���� � ����������

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        if (_rb != null)
        {
            _rb.isKinematic = true;
            _rb.useGravity = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // ���������, ��� �� ����� �������� ����� � ����� ��� �� ������
        if (collision.gameObject.CompareTag("Player") && !_hasFallen)
        {
            if (_rb != null)
            {
                _rb.isKinematic = false;
                _rb.useGravity = true;
            }
            _hasFallen = true;
            Destroy(gameObject, 5f);
            //Debug.Log("����� �������� �� �����. ��������� ������...");
            // ��������� �������� � ��������� �� �� ������
            //_fallCoroutine = StartCoroutine(FallDelay(_timeRequiredToFall));
        }
    }

    //private void OnCollisionExit(Collision collision)
    //{
    //    // ���� ����� ����� � ����� � ����� ��� �� ������
    //    if (collision.gameObject.CompareTag("Player") && !_hasFallen)
    //    {
    //        // ���� �������� ��������, ������������� �
    //        if (_fallCoroutine != null)
    //        {
    //            StopCoroutine(_fallCoroutine);
    //            _fallCoroutine = null; // �������� ������
    //            Debug.Log("����� ����� � �����. ������ �������.");
    //        }
    //    }
    //}

    //private IEnumerator FallDelay(float delay)
    //{

    //    yield return new WaitForSeconds(delay);
    //    if (_rb != null)
    //    {
    //        _rb.isKinematic = false;
    //        _rb.useGravity = true;
    //    }
    //    _hasFallen = true;
    //    Destroy(gameObject, 5f);
    //}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, transform.lossyScale);

    }

}
