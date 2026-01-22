using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// [Lv 3] 객체의 행동 조율
/// 1. 하위 모듈 초기화
/// 2. 받은 값으로 모듈 및 뷰어에게 동작 지시
/// </summary>
public class RobotCoordinator : MonoBehaviour
{
    [Header("Robot Refs")]
    [SerializeField] private Rigidbody m_rb;
    [SerializeField] private MovementModule m_movementModule;
    [SerializeField] private InteractionModule m_interactionModule;
    [SerializeField] private InteractionViewer m_interactionViewer;
    
    [Header("Service")]
    [SerializeField] private InputService m_inputService;
    [SerializeField] private LootService m_lootService;

    [Tooltip("버튼을 눌러 현재 상호작용 중인 대상")]
    [SerializeField] private IInteractable m_currentTarget;

    [Header("Service's InputValues")]
    [SerializeField] private Vector2 m_currentMovementInput;
    // 패드 포지션
    [SerializeField] private Vector2 m_currentLookInput;
    // 마우스 포지션
    [SerializeField] private Vector2 m_currentMousePos;
    [SerializeField] private bool m_isInteractionInput = false;

    [Header("targetPosition")]
    [SerializeField] private Vector3 m_targetPos;

    [Header("텍스트 상수, 추후 데이터로 빼야 함")]
    private const string MSG_LOOT = "Open <color=yellow>[E]</color>";
    private const string MSG_TALK = "Talk <color=yellow>[E]</color>";
    private const string MSG_GATHER = "Collection <color=yellow>[E]</color>";
    private const string MSG_DEFAULT = "Interaction <color=yellow>[E]</color>";

    private bool m_isInitialized = false;
    /// <summary>
    /// 같은 게임오브젝트 내 참조 초기화
    /// </summary>
    private void InitRefs()
    {
        TryGetComponent(out m_rb);
        TryGetComponent(out m_movementModule);
        TryGetComponent(out m_interactionModule);
        // TryGetComponent(out m_interactionViewer);
        m_interactionViewer = FindFirstObjectByType<InteractionViewer>();
        
    }

    public void InitClass(InputService inputService, LootService lootService, RobotDataSO data, Camera camera, UICoordinator uiCoordinator)
    {
        // 같은 게임오브젝트 내 하위 모듈 참조 초기화
        InitRefs();

        // 받아온 참조 초기화
        m_inputService = inputService;
        m_lootService = lootService;
        m_isInitialized = true;

        // 하위 모듈 초기화
        m_movementModule.InitClass(m_rb, data, camera.transform);

        m_interactionViewer.InitClass(camera.transform);

        m_interactionViewer.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!m_isInitialized) return;

        m_inputService.UpdateInputState();
        m_currentMovementInput = m_inputService.GetMoveInput();
        m_currentLookInput = m_inputService.GetLookInput();
        m_currentMousePos = m_inputService.GetMousePosition();
        m_isInteractionInput = m_inputService.GetInteractionButton();

        // 타겟과의 거리 체크, 타겟이 있을 때 너무 멀면 UI를 닫기
        // 타겟이 가까워지면 타겟의 UI를 열기
        if (m_currentTarget != null)
        {
            float distance = Vector3.Distance(this.transform.position, m_targetPos);
            if (distance > 3f)
            {
                CloseInteractionUI();
            }
        }
        // 이 타겟은 로봇이 감지거리 밖으로 나가는 순간 무조건 없어지므로 거리를 체크할 수 없음
        if (m_interactionModule.CurrentTarget != null)
        {
            UpdateInteractionMenuUI(true);
            // 타겟이 있는 상황에서 상호작용 버튼을 눌렀을 때 상호작용 시도, 중복 상호작용 방지
            if (m_isInteractionInput)
            {
                TryInteract();
            }
        }
        else
        {
            UpdateInteractionMenuUI(false);
        }

    }
    private void FixedUpdate()
    {
        if (!m_isInitialized) return;

        m_movementModule.MovementFixed(m_currentMovementInput);
        m_movementModule.RotationFixed(m_currentLookInput, m_currentMousePos, m_inputService.CurrentControlScheme);
    }

    /// <summary>
    /// 상호작용 가능한 경우 표시, 받아온 객체에 따라 텍스트를 다르게 표시
    /// </summary>
    private void UpdateInteractionMenuUI(bool open)
    {
        if (open)
        {
            // 방어적 코드
            var currentTarget = m_interactionModule.CurrentTarget;
            string message = currentTarget.InteractionPrompt;
            if (currentTarget is MonoBehaviour targetMono)
            {
                m_interactionViewer.DisplayUI(targetMono.transform, message);
            }
        }
        else
        {
            m_interactionViewer.Hide();
        }

    }

    /// <summary>
    /// 열기 상호작용 시 호출
    /// </summary>
    /// <param name="target"></param>
    private void OnLootingUI(LootableEntity target)
    {
        m_currentTarget = target;
        m_lootService.ProcessLooting(target, target.Contents);
    }
    /// <summary>
    /// NPC와 상호작용 시 호출
    /// </summary>
    /// <param name="target"></param>
    private void OnTalkingUI(NPCCoordinator target)
    {
        m_currentTarget = target;
        // 대화 UI 오픈 로직 추후 구현, 여기서는 NPC와 대화 관련 서비스 클래스를 추가로 구현해야 하는지 고민 필요
    }
    private void CloseInteractionUI()
    {
        if (m_currentTarget != null)
        {
            m_currentTarget = null;
            m_targetPos = this.transform.position;
            m_lootService.ProcessCloseLooting();
        }
    }

    private void TryInteract()
    {
        var detectedTarget = m_interactionModule.CurrentTarget;
        if (detectedTarget == null) return;

        // 기존 타겟과 동일한 타겟이고 UI가 열려있는 상태라면 상호작용 종료
        if (m_currentTarget == detectedTarget)
        {
            return;
        }
        // 타겟 교체, 기존 타겟과 다르면 UI 종료
        else
        {
            CloseInteractionUI();
        }

        // 타겟을 캐싱
        m_currentTarget = detectedTarget;

        // 타겟의 위치를 받기 위해 MonoBehaviour로 캐스팅, 게임에 엔티티가 존재하는 한 모노는 null이 될 수 없음
        if (m_currentTarget is MonoBehaviour target)
        {
            m_targetPos = target.transform.position;
        }
        else
        {
            m_targetPos = this.transform.position;
        }

        // 루팅 가능한 오브젝트
        if (m_currentTarget is LootableEntity lootableTarget)
        {
            OnLootingUI(lootableTarget);
        }
        else if (m_currentTarget is NPCCoordinator npcTarget)
        {
            // 나중에 NPC와 상호작용을 한다면 대화를 하는 경우도 있으므로 어디서 나눌지 고민
            OnTalkingUI(npcTarget);
        }
        // 추후 제작대나 건물 등 상호작용 대상이 추가되어야 함

    }
}
