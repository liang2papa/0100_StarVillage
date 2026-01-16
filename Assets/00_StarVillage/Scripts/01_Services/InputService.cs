using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// [Lv 1] 입력 담당
/// </summary>
public class InputService
{
    private readonly GameDirector m_director;
    private readonly InputSystem_Actions m_inputActions;
    public EControlScheme CurrentControlScheme { get; private set; } = EControlScheme.KeyboardMouse;
    public event Action<EControlScheme> OnControlSchemeChanged;
    public InputService(GameDirector director)
    {
        m_director = director;
        m_inputActions = new InputSystem_Actions();
        m_inputActions.Enable();
    }
    public void UpdateInputState()
    {
        EControlScheme nextScheme = CurrentControlScheme;

        // 하드웨어 감지 로직
        if (IsMouseMoved())
            nextScheme = EControlScheme.KeyboardMouse;
        if (IsPadMoved())
            nextScheme = EControlScheme.Gamepad;

        // 상태가 실제 바뀔 때만 이벤트 발생
        if (nextScheme != CurrentControlScheme)
        {
            CurrentControlScheme = nextScheme;
            OnControlSchemeChanged?.Invoke(CurrentControlScheme);
        }
    }


    public bool GetInteractionButton()
    {
        if (m_director.CurrentState != EGameState.Playing)
            return false;

        return m_inputActions.Player.Interact.WasPressedThisFrame();
    }
    public Vector2 GetMoveInput()
    {
        if (m_director.CurrentState != EGameState.Playing)
            return Vector2.zero;

        return m_inputActions.Player.Move.ReadValue<Vector2>();
    }
    public Vector2 GetLookInput()
    {
        if (m_director.CurrentState != EGameState.Playing)
            return Vector2.zero;

        return m_inputActions.Player.Look.ReadValue<Vector2>();
    }
    public Vector2 GetMousePosition()
    {
        if (m_director.CurrentState != EGameState.Playing)
            return Vector2.zero;

        return Mouse.current.position.ReadValue();
    }

    private bool IsMouseMoved()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        return mouseDelta.sqrMagnitude > 0.01f;
    }
    private bool IsPadMoved()
    {
        if (Gamepad.current == null) 
            return false;
        Vector2 rightStick = Gamepad.current.rightStick.ReadValue();
        Vector2 leftStick = Gamepad.current.leftStick.ReadValue();

        // [추가] 패드의 남쪽 버튼(A/X)이나 서쪽 버튼(X/Y) 등을 눌러도 패드 사용으로 간주
        bool isButtonPressed = Gamepad.current.buttonSouth.wasPressedThisFrame ||
                               Gamepad.current.buttonWest.wasPressedThisFrame;

        return rightStick.sqrMagnitude > 0.01f || leftStick.sqrMagnitude > 0.01f || isButtonPressed;
    }
    ~InputService()
    {
        m_inputActions.Disable();
    }
}
