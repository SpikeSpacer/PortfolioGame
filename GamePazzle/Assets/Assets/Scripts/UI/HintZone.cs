using UnityEngine;
using TMPro; // Для работы с TextMeshPro

public class HintZone : MonoBehaviour
{
    [Header("Hint Settings")]
    [Tooltip("Текст, который будет отображаться при входе в эту зону.")]
    [TextArea(1, 3)] // Чтобы удобнее было вводить многострочный текст в инспекторе
    [SerializeField] private string _hintText = "Нажмите [E] для взаимодействия.";

    [Header("Detection Settings")]
    [Tooltip("Тэг игрового объекта, который будет активировать подсказку (обычно 'Player').")]
    [SerializeField] private string _playerTag = "Player";

    [Header("World Space UI Settings")]
    [Tooltip("Префаб Canvas с TextMeshPro в режиме World Space.")]
    [SerializeField] private GameObject _worldSpaceHintUIPrefab;
    [Tooltip("Вертикальное смещение подсказки над объектом.")]
    [SerializeField] private float _hintVerticalOffset = 1.5f;

    private GameObject _spawnedHintUI; // Экземпляр UI подсказки в мире
    private TextMeshProUGUI _spawnedHintTextMesh; // Текстовый компонент подсказки
    private Camera _mainCamera; // Ссылка на основную камеру

    private void Awake()
    {
        _mainCamera = Camera.main; // Кэшируем основную камеру

        // Проверка, что коллайдер является триггером
        if (TryGetComponent<Collider>(out Collider col))
        {
            if (!col.isTrigger)
            {
                Debug.LogWarning("HintZone: Коллайдер на объекте '" + gameObject.name + "' не установлен как триггер! Подсказка не будет работать корректно.", this);
            }
        }
        else
        {
            Debug.LogError("HintZone: На объекте '" + gameObject.name + "' отсутствует компонент Collider! Подсказка не будет работать.", this);
            enabled = false; // Отключаем скрипт
        }

        if (_worldSpaceHintUIPrefab == null)
        {
            Debug.LogError("HintZone: Поле _worldSpaceHintUIPrefab не назначено! Перетащите префаб WorldSpaceHintCanvas.", this);
            enabled = false; // Отключаем скрипт
        }
    }

    private void LateUpdate()
    {
        // Если подсказка отображается, заставляем ее смотреть на камеру
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
            // Создаем экземпляр префаба
            _spawnedHintUI = Instantiate(_worldSpaceHintUIPrefab, transform.position + Vector3.up * _hintVerticalOffset, Quaternion.identity, transform);
            // Получаем компонент TextMeshProUGUI
            _spawnedHintTextMesh = _spawnedHintUI.GetComponentInChildren<TextMeshProUGUI>();

            if (_spawnedHintTextMesh == null)
            {
                Debug.LogError("HintZone: В префабе WorldSpaceHintUIPrefab не найден TextMeshProUGUI!", this);
                Destroy(_spawnedHintUI); // Уничтожаем, если не нашли текст
                _spawnedHintUI = null;
                return;
            }
        }

        _spawnedHintTextMesh.text = _hintText; // Устанавливаем текст
        _spawnedHintUI.SetActive(true); // Активируем UI

        // Для плавного появления можно использовать CanvasGroup на _spawnedHintUI,
        // но это усложнит код, пока простое появление.
    }

    private void HideWorldHint()
    {
        if (_spawnedHintUI != null)
        {
            _spawnedHintUI.SetActive(false); // Деактивируем UI
            // Можно также уничтожить объект, если хотите экономить память
            // Destroy(_spawnedHintUI); 
            // _spawnedHintUI = null;
        }
    }

    private void OnDestroy()
    {
        // Очистка при уничтожении объекта HintZone
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