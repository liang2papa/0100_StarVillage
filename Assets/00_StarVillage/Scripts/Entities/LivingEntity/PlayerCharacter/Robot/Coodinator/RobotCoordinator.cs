using System.Collections.Generic;
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
    [SerializeField] private InteractionModule m_interactionModule;

    [Header("Injection Refs")]
    [SerializeField] private UICoordinator m_uiCoordinator;

    [Header("Service")]
    private InputService m_inputService;

    [Header("Service's InputValues")]
    [SerializeField] private Vector2 m_currentMovementInput;
    [SerializeField] private Vector2 m_currentLookInput;
    [SerializeField] private Vector2 m_currentMousePos;

    private bool m_isInitialized = false;
    public void InitClass(InputService inputService, RobotDataSO data, Transform cameraTransform, UICoordinator uiCoordi)
    {
        // 같은 게임오브젝트 내 하위 모듈 참조 초기화
        InitRefs();

        // 받아온 참조 초기화
        m_inputService = inputService;
        m_uiCoordinator = uiCoordi;
        m_isInitialized = true;

        // 하위 모듈 초기화
        m_movementModule.InitClass(m_rb, data, cameraTransform);
    }
    /// <summary>
    /// 같은 게임오브젝트 내 참조 초기화
    /// </summary>
    private void InitRefs()
    {
        TryGetComponent(out m_rb);
        TryGetComponent(out m_movementModule);
        TryGetComponent(out m_interactionModule);

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
    public void DisplayItems(LootableEntity target, List<InventoryItem> items)
    {
        m_uiCoordinator.DisplayItems(target, items);
    }
    public void DisplayMessage()
    {
        m_uiCoordinator.DisplayMessage();
    }
}
