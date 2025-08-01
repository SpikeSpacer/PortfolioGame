// --- InteractableInfoDisplay.cs ---
using UnityEngine;

public class InteractableInfoDisplay : MonoBehaviour, IInteractable
{
    [Header("������")]
    [SerializeField] private string _promptText = "���������";

    [SerializeField] private HintTextPrompt _infoTextToShow;

    // --- ���������� ���������� IInteractable ---
    public string InteractionPrompt => _promptText;
    public string InfoText => _infoTextToShow != null ? _infoTextToShow.InfoText : "";
    public bool IsContinuous => false; // ��� ���������� ��������������
    public bool IsInfoPanelActive { get; set; }

    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Interactable");
    }

    public void BeginInteraction(PlayerController player)
    {
        if (InteractionInfoUIManager.Instance == null) return;

        // ���� ������ ��� ������� ��� ����� �������, �������� ��. ����� - ����������.
        if (IsInfoPanelActive)
        {
            InteractionInfoUIManager.Instance.HideInfo();
        }
        else
        {
            InteractionInfoUIManager.Instance.ShowInfo(this);
        }
    }

    // ��� ������ �� ����� ��� ����������� ��������������, ��������� �� �������.
    public void UpdateInteraction(PlayerController player) { }
    public void EndInteraction(PlayerController player) { }
}
