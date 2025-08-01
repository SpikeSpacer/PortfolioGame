// --- IInteractable.cs ---
using UnityEngine;

/// <summary>
/// ������ ��������� ��� ���� ��������, � �������� ����� ����������������� �����.
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// ��������� ���������, ������������, ����� ����� ������� �� ������ (��������, "������������ �����").
    /// </summary>
    //string InteractionPrompt { get; }

    /// <summary>
    /// �����, ������� ����� ��������� � �������������� ������.
    /// ����� ���� ������, ���� �������������� �� ������������� ����� ����������.
    /// </summary>
    string InfoText { get; }

    /// <summary>
    /// ���������, �������� �� �������������� ���������� (true) ��� ���������� (false).
    /// </summary>
    bool IsContinuous { get; }

    /// <summary>
    /// ����, �����������, ������� �� �������������� ������ ��� ����� �������.
    /// ����������� �����, ��������, UI-����������.
    /// </summary>
    bool IsInfoPanelActive { get; set; }

    /// <summary>
    /// ���������� ��� ������ ��������������.
    /// </summary>
    void BeginInteraction(PlayerController player);

    /// <summary>
    /// ���������� ������ ���� �� ����� ����������� (continuous) ��������������.
    /// </summary>
    void UpdateInteraction(PlayerController player);

    /// <summary>
    /// ���������� ��� ���������� ����������� (continuous) ��������������.
    /// </summary>
    void EndInteraction(PlayerController player);
}
