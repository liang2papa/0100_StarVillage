using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 루팅 가능한 아이템 목록을 담고 있는 엔티티
/// </summary>
public abstract class LootableEntity : InteractiveEntity
{
    [Header("Loot Settings")]
    [SerializeField] protected DropTableDataSO m_dropTable; // 드랍 테이블
    [SerializeField] protected List<InventoryItem> m_contents = new(); // 실제 들어있는 아이템들

    [SerializeField] protected bool m_isGenerated = false; // 드랍 아이템이 생성되었는지 여부
    [SerializeField] protected float m_openDelay;

    [SerializeField] protected UICoordinator m_lootUI; // 추후 인터페이스로 참조하도록 변경

    // 아이템 생성 로직 (최초 1회 실행)
    protected virtual void GenerateLoot()
    {
        if (m_isGenerated) return;
        m_contents = m_dropTable.DropTable;
        m_isGenerated = true;
    }

    // 공통 기능: 루팅 UI 열기
    public void OpenLootUI()
    {
        if (!m_isGenerated) GenerateLoot();

        // 빈 껍데기면 UI 안 열고 알림만 띄울 수도 있음
        if (m_contents.Count == 0)
        {
            m_lootUI.DisplayMessage();
            return;
        }
        else
        {
            m_lootUI.DisplayItems();
        }

    }
}
