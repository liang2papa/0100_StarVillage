using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// [Lv 3] 필드에서 상호작용할 수 있는 객체, ItemData를 소유
/// </summary>
public abstract class LootableEntity : MonoBehaviour, IInteractable
{
    [Header("엔티티 이름")]
    [field: SerializeField] public string EntityName { get; protected set; }

    [Header("상호작용 가능 여부")]
    [field: SerializeField] public bool IsInteractable { get; protected set; } = true;

    [Header("상호작용 시 텍스트")]
    [field: SerializeField] public string InteractionPrompt { get; protected set; } = "Open <color=yellow>[E]</color>";

    [Header("컨테이너를 열어보았는지 여부, 추후 UI 반영 예정")]
    [field: SerializeField] public bool IsChecked { get; protected set; } //  열어봤을 경우 주웠든 안주웠든 true

    [Header("컨테이너 안이 비었는지 확인 여부, 추후 UI 반영 예정")]
    [field: SerializeField] public bool IsEmpty { get; protected set; } // 비어있는지 여부

    [Header("Loot Settings")]
    [SerializeField] protected DropTableDataSO m_dropTable; // 드랍 테이블
    public List<InventoryItem> Contents { get; private set; } = new();// 실제 들어있는 아이템들

    [Header("외부 참조")]
    [SerializeField] protected RobotCoordinator m_robot; // 추후 인터페이스로 참조하도록 변경

    [Header("리소스의 상태")]
    [SerializeField] protected bool m_isGenerated = false; // 드랍 아이템이 생성되었는지 여부
    [SerializeField] protected float m_openDelay;


    /// <summary>
    ///  아이템 생성 로직 (최초 1회 실행)
    ///  방안1. 월드 생성 시 모든 루팅 아이템 생성 X
    ///  방안2. 루팅 UI를 열 때 생성 이쪽으로 선택 O
    /// </summary>
    protected virtual void GenerateLoot()
    {
        if (m_isGenerated) return;
        Contents = m_dropTable.DropTable;
        m_isGenerated = true;
        IsChecked = true;
    }

    public void OnInteract(Transform interactor)
    {
        // 기본 상호작용 로직은 없음, 상속받은 클래스에서 구현
    }

    public virtual void SetChecked(bool value = true)
    {
        IsChecked = value;
    }
    public virtual void SetEmpty(bool value = true)
    {
        IsEmpty = value;
    }
    
    protected virtual void OnDrawGizmos()
    {
        if (IsChecked)
        {
            Gizmos.color = Color.yellow;
            if (IsEmpty)
            {
                Gizmos.color = Color.gray;
            }
            else
            {
                Gizmos.color = Color.blue;
            }
        }
        else
        {
            Gizmos.color = Color.green;
        }

            Gizmos.DrawWireCube(transform.position, transform.lossyScale);
    }

}
