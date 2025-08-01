using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private InputActions _inputAction;

    // ������� ��� �������� WASD
    public System.Action<Vector2> OnMovePerformed;
    public System.Action OnMoveCanceled;

    // ������� ��� ���� (Shift)
    public System.Action OnRunPerformed;
    public System.Action OnRunCanceled;

    // ������������ ������� ��� ����������/�������� ������ � �����
    public System.Action OnEquipDisarmPerformed; // �������, ������� ����� ���������� ��� ������� Q

    public System.Action OnInteractePerformed;    // E button
    public System.Action OnInteracteCancel;

    // --- �����: ������� ��� �������� ������ (Look) ---
    public System.Action<Vector2> OnLookPerformed;


    private void Awake()
    {
        _inputAction = new InputActions();
    }

    private void OnEnable()
    {
        _inputAction.Enable();

        // ������������� �� ������� �������� WASD
        _inputAction.Player.Move.performed += HandleMovePerformed;
        _inputAction.Player.Move.canceled += HandleMoveCanceled;

        // ������������� �� ������� ����
        _inputAction.Player.Run.performed += HandleRunPerformed;
        _inputAction.Player.Run.canceled += HandleRunCanceled;

        _inputAction.Player.Interact.performed += Interact_Handler_performed;
        _inputAction.Player.Interact.canceled += Interact_Handler_cancel;

        // --- �����: ������������� �� ������� Look ---
        _inputAction.Player.Look.performed += HandleLookPerformed;
        _inputAction.Player.Look.canceled += HandleLookCanceled; // ����� �������� ��� ������, ���� �����
    }

    void OnDisable()
    {
        _inputAction.Disable();

        // ������������ �� ������� �������� WASD
        _inputAction.Player.Move.performed -= HandleMovePerformed;
        _inputAction.Player.Move.canceled -= HandleMoveCanceled;

        // ������������ �� ������� ����
        _inputAction.Player.Run.performed -= HandleRunPerformed;
        _inputAction.Player.Run.canceled -= HandleRunCanceled;

        _inputAction.Player.Interact.performed -= Interact_Handler_performed;
        _inputAction.Player.Interact.canceled -= Interact_Handler_cancel;

        // --- �����: ������������ �� ������� Look ---
        _inputAction.Player.Look.performed -= HandleLookPerformed;
        _inputAction.Player.Look.canceled -= HandleLookCanceled;
    }

    // ���������� ��� ��������� ������� �������� �� WASD
    private void HandleMovePerformed(InputAction.CallbackContext context)
    {
        Vector2 inputVector = context.ReadValue<Vector2>();
        OnMovePerformed?.Invoke(inputVector);
    }

    // ���������� ��� ���������� WASD (��������� ��������)
    private void HandleMoveCanceled(InputAction.CallbackContext context)
    {
        OnMoveCanceled?.Invoke();
    }

    // ����������� ��� ���� (Shift)
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

    // --- �����: ���������� ��� ��������� ����� � ���� (Look) ---
    private void HandleLookPerformed(InputAction.CallbackContext context)
    {
        Vector2 lookInput = context.ReadValue<Vector2>();
        OnLookPerformed?.Invoke(lookInput);
    }

    private void HandleLookCanceled(InputAction.CallbackContext context)
    {
        // ��������, ��� �� ����� ������ ������ ��� ������ Look, ��� ��� ��� ���������� ����.
        // �� ��� ������� ����� �������� OnLookPerformed.
        OnLookPerformed?.Invoke(Vector2.zero);
    }

    // ���� ����� OnRotateObject � ��� � PlayerInput.cs ������,
    // ���� �� ��� ����������� HandleLookPerformed � OnLookPerformed.
    // ������� ���, ����� �������� ��������.
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