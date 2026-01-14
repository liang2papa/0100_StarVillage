using UnityEngine;

/// <summary>
/// [Lv 3] 객체의 행동 조율
/// 1. 하위 모듈 초기화
/// 2. 인풋으로 받은 값으로 모듈의 동작 지시
/// </summary>
public class RobotCoordinator : MonoBehaviour
{
    [Header("Robot Refs")]
    [SerializeField] private Rigidbody m_rb; 
    [SerializeField] private MovementModule m_movementModule;

    [Header("Service")]
    private InputService m_inputService;

    [Header("Service's InputValues")]
    [SerializeField] private Vector2 m_currentMovementInput;
    [SerializeField] private Vector2 m_currentLookInput;
    [SerializeField] private Vector2 m_currentMousePos;

    private bool m_isInitialized = false;
    public void InitClass(InputService inputService, RobotDataSO data, Transform cameraTransform)
    {
        m_inputService = inputService;
        m_isInitialized = true;
        m_movementModule.InitClass(m_rb, data, cameraTransform);
    }
    

    private void Update()
    {
        if (!m_isInitialized) return;

        m_inputService.UpdateInputState();
        m_currentMovementInput = m_inputService.GetMoveInput();
        m_currentLookInput = m_inputService.GetLookInput();
        m_currentMousePos = m_inputService.GetMousePosition();

    }
    private void FixedUpdate()
    {
        if (!m_isInitialized) return;

        m_movementModule.MovementFixed(m_currentMovementInput);
        m_movementModule.RotationFixed(m_currentLookInput, m_currentMousePos, m_inputService.CurrentControlScheme);
        
    }
}
