using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FallingPlate : MonoBehaviour
{
    [Header("Настройки падения")]
    [Tooltip("Время в секундах, которое игрок должен непрерывно стоять на плите для её падения.")]
    //private float _timeRequiredToFall = 0.1f;

    private Rigidbody _rb;
    private bool _hasFallen = false; // Флаг, чтобы плита падала только один раз
    private Coroutine _fallCoroutine; // Ссылка на запущенную корутину, чтобы можно было её остановить

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
        // Проверяем, что на плиту наступил игрок И плита еще не падала
        if (collision.gameObject.CompareTag("Player") && !_hasFallen)
        {
            if (_rb != null)
            {
                _rb.isKinematic = false;
                _rb.useGravity = true;
            }
            _hasFallen = true;
            Destroy(gameObject, 5f);
            //Debug.Log("Игрок наступил на плиту. Запускаем таймер...");
            // Запускаем корутину и сохраняем на неё ссылку
            //_fallCoroutine = StartCoroutine(FallDelay(_timeRequiredToFall));
        }
    }

    //private void OnCollisionExit(Collision collision)
    //{
    //    // Если игрок сошел с плиты И плита еще не падала
    //    if (collision.gameObject.CompareTag("Player") && !_hasFallen)
    //    {
    //        // Если корутина запущена, останавливаем её
    //        if (_fallCoroutine != null)
    //        {
    //            StopCoroutine(_fallCoroutine);
    //            _fallCoroutine = null; // Обнуляем ссылку
    //            Debug.Log("Игрок сошел с плиты. Таймер сброшен.");
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
