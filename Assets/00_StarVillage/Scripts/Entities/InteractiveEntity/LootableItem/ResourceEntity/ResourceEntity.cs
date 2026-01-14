using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 광물 등 자원 채집이 가능한 엔티티
/// </summary>
public class ResourceEntity : LootableEntity
{
    [SerializeField] private ResourceDataSO m_resourceData;

    [Header("리소스 값")]
    [SerializeField] private float m_currentHP;
    [SerializeField] private List<EToolType> m_validTool;

    [Header("리소스의 상태")]
    // 
    [SerializeField] private bool m_isDepleted; 
    

    /// <summary>
    /// 주입으로 초기화 예정, 인스톨러가 해야 할 일
    /// </summary>
    private void Start()
    {
        m_currentHP = m_resourceData.MaxHP;
        m_validTool = m_resourceData.ValidToolTypes;
        m_isDepleted = false;
        
    }

    public override void OnInteract(Transform interactor)
    {

    }

    public void OnMine(float damage, EToolType tool)
    {
        foreach (EToolType validTool in m_validTool)
        {
            if (tool == validTool)
            {
                m_currentHP -= damage * m_resourceData.DamageMultiplier;
            }
            else
            {
                m_currentHP -= damage;
            }
        }
        if (m_currentHP <= 0)
        {
            CompleteMining();
        }
        return;
    }
    // 제작 중
    public void CompleteMining()
    {

    }

}
