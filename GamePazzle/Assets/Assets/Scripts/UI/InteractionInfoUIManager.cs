// --- InteractionInfoUIManager.cs ---
using UnityEngine;
using TMPro;
using System.Collections;

public class InteractionInfoUIManager : MonoBehaviour
{
    public static InteractionInfoUIManager Instance { get; private set; }

    [Header("Ссылки на UI")]
    [SerializeField] private TextMeshProUGUI _infoTextUI;

    [Header("Настройки")]
    [SerializeField] private float _autoHideDelay = 5f;

    private Coroutine _autoHideCoroutine;
    private IInteractable _currentInfoSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (_infoTextUI != null)
        {
            _infoTextUI.transform.parent.gameObject.SetActive(false); // Скрываем всю панель
        }
        else
        {
            Debug.LogError("InteractionInfoUIManager: Ссылка на _infoTextUI не установлена!", this);
            enabled = false;
        }
    }

    /// <summary>
    /// Показывает панель информации, получая данные напрямую из источника.
    /// </summary>
    public void ShowInfo(IInteractable source)
    {
        if (source == null || string.IsNullOrEmpty(source.InfoText)) return;

        if (_currentInfoSource != null)
        {
            HideInfo();
        }

        _currentInfoSource = source;
        _currentInfoSource.IsInfoPanelActive = true;

        _infoTextUI.text = _currentInfoSource.InfoText;
        _infoTextUI.transform.parent.gameObject.SetActive(true);

        if (_autoHideDelay > 0)
        {
            _autoHideCoroutine = StartCoroutine(AutoHidePanel());
        }
    }

    /// <summary>
    /// Скрывает панель информации.
    /// </summary>
    public void HideInfo()
    {
        if (_autoHideCoroutine != null)
        {
            StopCoroutine(_autoHideCoroutine);
            _autoHideCoroutine = null;
        }

        if (_currentInfoSource != null)
        {
            _currentInfoSource.IsInfoPanelActive = false;
            _currentInfoSource = null;
        }

        if (_infoTextUI != null)
        {
            _infoTextUI.transform.parent.gameObject.SetActive(false);
        }
    }

    private IEnumerator AutoHidePanel()
    {
        yield return new WaitForSeconds(_autoHideDelay);
        HideInfo();
    }
}
