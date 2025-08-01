using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [Header("Настройки Возрождения")]
    [Tooltip("Ссылка на объект SpawnPoint в сцене.")]
    [SerializeField] private Transform spawnPoint1;
    [SerializeField] private Transform spawnPoint2;

    [Tooltip("Минимальная высота (Y-координата), при падении ниже которой игрок будет возвращен.")]
    [SerializeField] private float fallThresholdY = -10f; // Например, -10 метров по Y

    [Tooltip("Задержка в секундах перед возвращением игрока после падения.")]
    [SerializeField] private float respawnDelay = 1f; // Задержка перед телепортацией

    public bool _spawnpoint_1 = true;
    public bool _spawnpoint_2 = false;

    private Rigidbody _playerRb; // Ссылка на Rigidbody игрока (если есть)

    private void Awake()
    {
        _playerRb = GetComponent<Rigidbody>();

        if (spawnPoint1 == null)
        {
            Debug.LogError("PlayerRespawn: SpawnPoint не назначен! Пожалуйста, перетащите объект SpawnPoint в Инспектор.", this);
            enabled = false; // Отключаем скрипт, если нет SpawnPoint
        }
    }

    private void Update()
    {
        // Проверяем Y-координату игрока
        if (transform.position.y < fallThresholdY)
        {
            // Если игрок упал ниже порога, начинаем процесс возрождения
            RespawnPlayer();
        }
    }

    private void RespawnPlayer()
    {
        // Если игрок уже находится в процессе возрождения, не запускаем его снова
        // Можно использовать флаг, если нужно более сложное управление.
        // Для простоты здесь мы полагаемся на то, что Update вызывается часто.

        // Можно добавить небольшую задержку перед реальной телепортацией
        Invoke("PerformRespawn", respawnDelay); // Вызываем метод PerformRespawn через respawnDelay секунд

        // Опционально: можно временно отключить управление игроком или показать экран смерти
        // В этом простом примере просто выводим лог
        Debug.Log("Игрок упал! Возвращаем в точку спавна.");

        // Отключаем этот скрипт временно, чтобы не вызывать Invoke() несколько раз
        // Пока PerformRespawn не вернет игрока.
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