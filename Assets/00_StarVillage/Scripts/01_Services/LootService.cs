using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// [Lv 1] 루팅, 아이템 획득 로직 담당
/// </summary>
[System.Serializable]
public class LootService
{
    [SerializeField] private UICoordinator m_uiCoordinator;

    // UICoordinator와의 상호작용 담당
    // 루팅 시작
    public event Action<LootableEntity> OnLootingStart;
    // 루팅 종료
    public event Action OnLootingEnd;

    // 현재 루팅 중인 대상
    public LootableEntity CurrentLootTarget { get; private set; }

    public LootService(UICoordinator uiCoordinator)
    {
        m_uiCoordinator = uiCoordinator;
    }
    public void ProcessLooting(LootableEntity target, List<InventoryItem> items)
    {
        m_uiCoordinator.DisplayItems(target, items);
        target.SetChecked(true);

    }
    /// <summary>
    /// 빈 오브젝트를 열고자 할 때 호출
    /// </summary>
    public void ProcessEmptyLooting()
    {
        m_uiCoordinator.DisplayMessage();
    }
    /// <summary>
    /// 플레이어와 오브젝트가 멀어졌을 때, 자신이 아닌 다른 오브젝트를 열 때, 루팅 UI를 닫을 때 호출
    /// </summary>
    public void ProcessCloseLooting()
    {
        m_uiCoordinator.CloseLootUI();

    }
    /// <summary>
    /// 아이템 획득 시 호출, UI가 서비스를 호출
    /// 1. 획득한 아이템을 오브젝트의 Contents에서 제거
    /// 2. 인벤토리에 아이템을 추가
    /// 3. 오브젝트가 빈 상태가 되었는지 확인
    /// 4. 빈 상태라면 오브젝트의 상태를 빈 상태로 변경
    /// </summary>
    /// <param name="target"></param>
    public void OnItemTaken(LootableEntity target)
    {
        if (target.Contents.Count == 0)
        {
            target.SetEmpty(true);
        }
    }
}
