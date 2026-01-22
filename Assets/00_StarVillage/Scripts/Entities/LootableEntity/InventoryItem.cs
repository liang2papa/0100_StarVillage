/// <summary>
/// 가변 데이터, 실제 인벤토리에 들어가는 아이템 클래스
/// </summary>
[System.Serializable]
public class InventoryItem
{
    public ItemDataSO Data; // 어떠한 아이템인가
    public int Count; // 몇 개 있는지

    public InventoryItem(ItemDataSO data, int amount)
    {
        Data = data;
        Count = amount;
        
    }
    // 수량 올리는 메소드
    public int AddCount(int amount)
    {
        int total = Count + amount;
        if (total <= Data.MaxStackSize)
        {
            Count = total;
            return 0;
        }
        else
        {
            Count = Data.MaxStackSize;
            return total - Data.MaxStackSize; // 넘치는 양만큼 반환
        }
    }
}