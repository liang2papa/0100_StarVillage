using UnityEngine;
/// <summary>
/// 가변 데이터, 실제 인벤토리에 들어가는 아이템 클래스
/// </summary>
[System.Serializable]
public class InventoryItem
{
    public ItemDataSO Data; // 어떠한 아이템인가
    public int Amount;    // 몇 개 들고 있는가
    public int CurerntDurability; // 현재 내구도 (내구도 아이템일 경우)

    public InventoryItem(ItemDataSO data, int amount)
    {
        Data = data;
        Amount = amount;
        
    }
}