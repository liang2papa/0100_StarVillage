// 3. 장비 데이터
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentDataSO", menuName = "Data/EquipmentDataSO")]
public class EquipmentDataSO : ItemDataSO
{
    [Header("장비 속성")]
    public EEquipType EquipType;   // 장착 부위
    public float MaxDurability;    // 최대 내구도

    [Header("세부 속성")]
    public int MinAtk;
    public int MaxAtk;
    public float AttackSpeed;
    public int MinDef;
    public int MaxDef;
    public int BackpackSize;
}