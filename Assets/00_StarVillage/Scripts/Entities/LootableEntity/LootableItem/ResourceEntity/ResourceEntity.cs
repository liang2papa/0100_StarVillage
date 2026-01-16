using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 광물 등 자원 채집이 가능한 엔티티
/// 파괴된 이후에 열 수 있도록 처리
/// </summary>
public class ResourceEntity : LootableEntity
{
    [SerializeField] private ResourceDataSO m_resourceData;

    [Header("리소스 값")]
    [SerializeField] private float m_currentHP;
    [SerializeField] private List<EToolType> m_validTool;


    /// <summary>
    /// 주입으로 초기화 예정, 필드 디렉터가 생성하거나 주입함
    /// 방안1. Instantiate로 생성해서 Start를 받아 초기화
    /// 방안2. 팩토리 메서드로 생성해서 메서드 주입하여 초기화
    /// </summary>
    private void Start()
    {
        m_currentHP = m_resourceData.MaxHP;
        m_validTool = m_resourceData.ValidToolTypes;
        
        IsChecked = false;
        IsEmpty = false;
    }


    /// <summary>
    /// 오브젝트가 피해를 받으면 호출
    /// 1. 피해를 어떻게 받을지 고민해보아야 함
    /// 2. 
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="tool"></param>
    public void OnGathering(float damage, EToolType tool)
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
            GatheringComplete();
        }
        return;
    }
    // 제작 중
    public void GatheringComplete()
    {
        // 획득 가능한 모델로 변경되어야 함
        
    }


}
