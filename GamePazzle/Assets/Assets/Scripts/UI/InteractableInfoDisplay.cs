// --- InteractableInfoDisplay.cs ---
using UnityEngine;

public class InteractableInfoDisplay : MonoBehaviour, IInteractable
{
    [Header("Тексты")]
    [SerializeField] private string _promptText = "Осмотреть";

    [SerializeField] private HintTextPrompt _infoTextToShow;

    // --- Реализация интерфейса IInteractable ---
    public string InteractionPrompt => _promptText;
    public string InfoText => _infoTextToShow != null ? _infoTextToShow.InfoText : "";
    public bool IsContinuous => false; // Это мгновенное взаимодействие
    public bool IsInfoPanelActive { get; set; }

    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }

    public void BeginInteraction(PlayerController player)
    {
        if (InteractionInfoUIManager.Instance == null) return;

        // Если панель уже активна для этого объекта, скрываем ее. Иначе - показываем.
        if (IsInfoPanelActive)
        {
            InteractionInfoUIManager.Instance.HideInfo();
        }
        else
        {
            InteractionInfoUIManager.Instance.ShowInfo(this);
        }
    }

    // Эти методы не нужны для мгновенного взаимодействия, оставляем их пустыми.
    public void UpdateInteraction(PlayerController player) { }
    public void EndInteraction(PlayerController player) { }
}
