using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceDataSO", menuName = "Data/ResourceDataSO")]
public class ResourceDataSO : ScriptableObject
{
    [Header("광물 등 엔티티의 체력")]
    public float MaxHP;

    [Header("유효한 타입")]
    public List<EToolType> ValidToolTypes;

    [Header("유효한 타입의 대미지 배율")]
    public int DamageMultiplier;
}