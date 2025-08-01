using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private InputActions _inputAction;

    // События для движения WASD
    public System.Action<Vector2> OnMovePerformed;
    public System.Action OnMoveCanceled;

    // События для бега (Shift)
    public System.Action OnRunPerformed;
    public System.Action OnRunCanceled;

    // Существующие события для экипировки/убирания оружия и атаки
    public System.Action OnEquipDisarmPerformed; // Событие, которое будет вызываться при нажатии Q

    public System.Action OnInteractePerformed;    // E button
    public System.Action OnInteracteCancel;

    // --- НОВОЕ: Событие для вращения камеры (Look) ---
    public System.Action<Vector2> OnLookPerformed;


    private void Awake()
    {
        _inputAction = new InputActions();
    }

    private void OnEnable()
    {
        _inputAction.Enable();

        // Подписываемся на события движения WASD
        _inputAction.Player.Move.performed += HandleMovePerformed;
        _inputAction.Player.Move.canceled += HandleMoveCanceled;

        // Подписываемся на события бега
        _inputAction.Player.Run.performed += HandleRunPerformed;
        _inputAction.Player.Run.canceled += HandleRunCanceled;

        _inputAction.Player.Interact.performed += Interact_Handler_performed;
        _inputAction.Player.Interact.canceled += Interact_Handler_cancel;

        // --- НОВОЕ: Подписываемся на событие Look ---
        _inputAction.Player.Look.performed += HandleLookPerformed;
        _inputAction.Player.Look.canceled += HandleLookCanceled; // Можно добавить для сброса, если нужно
    }

    void OnDisable()
    {
        _inputAction.Disable();

        // Отписываемся от событий движения WASD
        _inputAction.Player.Move.performed -= HandleMovePerformed;
        _inputAction.Player.Move.canceled -= HandleMoveCanceled;

        // Отписываемся от событий бега
        _inputAction.Player.Run.performed -= HandleRunPerformed;
        _inputAction.Player.Run.canceled -= HandleRunCanceled;

        _inputAction.Player.Interact.performed -= Interact_Handler_performed;
        _inputAction.Player.Interact.canceled -= Interact_Handler_cancel;

        // --- НОВОЕ: Отписываемся от события Look ---
        _inputAction.Player.Look.performed -= HandleLookPerformed;
        _inputAction.Player.Look.canceled -= HandleLookCanceled;
    }

    // Обработчик для получения вектора движения от WASD
    private void HandleMovePerformed(InputAction.CallbackContext context)
    {
        Vector2 inputVector = context.ReadValue<Vector2>();
        OnMovePerformed?.Invoke(inputVector);
    }

    // Обработчик для отпускания WASD (остановка движения)
    private void HandleMoveCanceled(InputAction.CallbackContext context)
    {
        OnMoveCanceled?.Invoke();
    }

    // Обработчики для бега (Shift)
    private void HandleRunPerformed(InputAction.CallbackContext context)
    {
        OnRunPerformed?.Invoke();
    }

    private void HandleRunCanceled(InputAction.CallbackContext context)
    {
        OnRunCanceled?.Invoke();
    }

    private void Interact_Handler_performed(InputAction.CallbackContext obj)
    {
        OnInteractePerformed?.Invoke();
    }

    private void Interact_Handler_cancel(InputAction.CallbackContext obj)
    {
        OnInteracteCancel?.Invoke();
    }

    // --- НОВОЕ: Обработчик для получения ввода с мыши (Look) ---
    private void HandleLookPerformed(InputAction.CallbackContext context)
    {
        Vector2 lookInput = context.ReadValue<Vector2>();
        OnLookPerformed?.Invoke(lookInput);
    }

    private void HandleLookCanceled(InputAction.CallbackContext context)
    {
        // Возможно, вам не нужно ничего делать при отмене Look, так как это постоянный ввод.
        // Но для полноты можно обнулить OnLookPerformed.
        OnLookPerformed?.Invoke(Vector2.zero);
    }

    // Этот метод OnRotateObject у вас в PlayerInput.cs лишний,
    // если вы уже используете HandleLookPerformed и OnLookPerformed.
    // Удалите его, чтобы избежать путаницы.
    // public event Action<Vector2> OnRotateObjectPerformed;
    // public void OnRotateObject(InputAction.CallbackContext context)
    // {
    //     if (context.performed)
    //     {
    //         Vector2 input = context.ReadValue<Vector2>();
    //         OnRotateObjectPerformed?.Invoke(input);
    //     }
    // }
}