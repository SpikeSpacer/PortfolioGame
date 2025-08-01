using System.Collections;
using UnityEngine;

public class FloatingObjects: MonoBehaviour
{
    // === Настройки движения ===
    [SerializeField] private float speed = 2f;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _stopTime = 2f;

    // === Состояния движения ===
    private Vector3 _pointA;
    private Vector3 _pointB;
    private Vector3 _currentTarget;
    private Coroutine _waitCoroutine;

    // === Инициализация ===
    private void Awake()
    {
        _pointA = transform.position;
        _pointB = _pointA + _offset;
        _currentTarget = _pointB;
    }

    // === Обновление каждый кадр ===
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

    // === Движение объекта ===
    private void MoveBuild()
    {
        transform.position = Vector3.MoveTowards(transform.position, _currentTarget, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, _currentTarget) < 0.1f && _waitCoroutine == null)
        {
            _waitCoroutine = StartCoroutine(Wait(_stopTime));
        }
    }

    // === Переключение цели движения ===
    private void SwitchDirection()
    {
        _currentTarget = (_currentTarget == _pointA) ? _pointB : _pointA;
    }

    // === Задержка перед сменой направления ===
    private IEnumerator Wait(float delay)
    {
        yield return new WaitForSeconds(delay);
        SwitchDirection();
        _waitCoroutine = null;
    }

    // === Визуализация в редакторе ===
    private void OnDrawGizmos()
    {
        // Используем значения, которые будут установлены в Awake или уже установлены
        // Если игра не запущена, _pointA может быть Transform.position, чтобы Gizmos показывал корректно
        // Если игра запущена, _pointA уже зафиксирована.
        Vector3 gizmoPointA = transform.position; // Изначальное положение объекта
        Vector3 gizmoPointB = gizmoPointA + _offset;

        // Чтобы корректно видеть путь во время выполнения (Play Mode),
        // лучше использовать закешированные _pointA и _pointB, если игра запущена.
        if (Application.isPlaying)
        {
            gizmoPointA = _pointA;
            gizmoPointB = _pointB;
        }
        // В режиме редактирования (Edit Mode) используем текущую позицию, чтобы видеть смещение
        else
        {
            gizmoPointA = transform.position;
            gizmoPointB = transform.position + _offset;
        }


        Gizmos.color = Color.green;
        Gizmos.DrawLine(gizmoPointA, gizmoPointB);

        // Дополнительно можно нарисовать сферы в точках
        Gizmos.DrawSphere(gizmoPointA, 0.1f);
        Gizmos.DrawSphere(gizmoPointB, 0.1f);
    }

}
