// 2. 일반/키 아이템 데이터
using UnityEngine;

[CreateAssetMenu(fileName = "GeneralItemDataSO", menuName = "Data/GeneralItemDataSO")]
public class GeneralItemDataSO : ItemDataSO
{
    [Header("소비 속성")]
    public bool IsConsumable;       // 소비 가능한 아이템인지
    public int MaxUsageCount;      // 1회용이면 1, 여러번이면 N (0이면 소비 불가)

}
