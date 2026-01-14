using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 광물 등 자원 채집이 가능한 엔티티
/// </summary>
public class ResourceEntity : LootableEntity
{
    [SerializeField] private ResourceDataSO m_resourceData;
    [SerializeField] private float m_currentHP;
    [SerializeField] private List<EToolType> m_validTool;

    public override void OnInteract(Transform interactor)
    {
        m_currentHP = m_resourceData.MaxHP;
        m_validTool = m_resourceData.ValidToolTypes;

    }

    public void OnMine(float damage, EToolType tool)
    {
        foreach (EToolType validTool in m_validTool)
        {
            if (tool == validTool)
            {
                m_currentHP -= damage * m_resourceData.DamageMultiplier;
                if (m_currentHP <= 0)
                {
                    CompleteMining();
                }
                return;
            }
        }
    }
    // 제작 중
    public void CompleteMining()
    {

    }

}
