using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 플레이어 데이터를 관리, 로봇 데이터는 해당 스크립터블 오브젝트를 상속하여 사용
/// </summary>
[CreateAssetMenu(fileName = "PlayerData", menuName = "Data/PlayerDataSO")]
public class PlayerDataSO : ScriptableObject
{
    [Header("Inventory Settings")]
    public int MaxSlots = 20; // 인벤토리 최대 칸 수

    // 실제 아이템이 저장되는 리스트
    public List<InventoryItem> Inventory = new List<InventoryItem>();

    /// <summary>
    /// 아이템 획득 로직
    /// </summary>
    /// <param name="newItem">획득할 아이템</param>
    /// <returns>성공 여부 (일부만 획득해도 true, 아예 못 넣으면 false)</returns>
    public bool AddItem(InventoryItem newItem)
    {
        // 1. 유효성 검사
        if (newItem == null || newItem.Data == null || newItem.Count <= 0) return false;

        // 2. 겹치기 가능한 아이템인 경우, 기존 슬롯에 합치기 시도
        if (newItem.Data.IsStackable)
        {
            foreach (var existingItem in Inventory)
            {
                if (existingItem.Data == newItem.Data) // 같은 아이템 발견
                {
                    int leftover = existingItem.AddCount(newItem.Count);
                    newItem.Count = leftover;

                    if (newItem.Count == 0) return true; // 모두 합쳐짐
                }
            }
        }

        // 3. 남은 아이템을 새 슬롯에 추가
        if (newItem.Count > 0)
        {
            // 빈 슬롯이 있는지 확인
            if (Inventory.Count < MaxSlots)
            {
                // 리스트에 새 인스턴스로 추가 (참조 문제 방지)
                Inventory.Add(new InventoryItem(newItem.Data, newItem.Count));
                return true;
            }
            else
            {
                Debug.LogWarning("인벤토리가 가득 찼습니다!");
                return false; // 공간 부족으로 실패
            }
        }

        return true;
    }
}
