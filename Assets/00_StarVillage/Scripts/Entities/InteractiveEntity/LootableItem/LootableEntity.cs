using System.Collections.Generic;
using UnityEngine;

public abstract class LootableEntity : InteractiveEntity
{
    [Header("Loot Settings")]
    [SerializeField] protected DropTableDataSO m_dropTable; // 드랍 테이블
    [SerializeField] protected List<InventoryItem> m_contents = new List<InventoryItem>(); // 실제 들어있는 아이템들

    protected bool m_isGenerated = false; // 드랍 아이템이 생성되었는지 여부

    [SerializeField] protected float m_openDelay;


}
