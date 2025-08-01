// --- IInteractable.cs ---
using UnityEngine;

/// <summary>
/// Единый интерфейс для всех объектов, с которыми может взаимодействовать игрок.
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// Текстовая подсказка, отображаемая, когда игрок смотрит на объект (например, "Активировать рычаг").
    /// </summary>
    //string InteractionPrompt { get; }

    /// <summary>
    /// Текст, который будет отображен в информационной панели.
    /// Может быть пустым, если взаимодействие не подразумевает показ информации.
    /// </summary>
    string InfoText { get; }

    /// <summary>
    /// Указывает, является ли взаимодействие длительным (true) или мгновенным (false).
    /// </summary>
    bool IsContinuous { get; }

    /// <summary>
    /// Флаг, указывающий, активна ли информационная панель для этого объекта.
    /// Управляется извне, например, UI-менеджером.
    /// </summary>
    bool IsInfoPanelActive { get; set; }

    /// <summary>
    /// Вызывается при начале взаимодействия.
    /// </summary>
    void BeginInteraction(PlayerController player);

    /// <summary>
    /// Вызывается каждый кадр во время длительного (continuous) взаимодействия.
    /// </summary>
    void UpdateInteraction(PlayerController player);

    /// <summary>
    /// Вызывается при завершении длительного (continuous) взаимодействия.
    /// </summary>
    void EndInteraction(PlayerController player);
}
