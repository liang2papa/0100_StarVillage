using System.Collections.Generic;
using UnityEngine;
/// [Lv 3] 객체의 행동 조율
/// 1. 각 UI Viewer에 메소드 주입
/// 2. UIViewer의 행동 조율
/// </summary>
public class UICoordinator : MonoBehaviour
{
    [SerializeField] private InventoryViewer m_inventoryViewer;
    [SerializeField] private GameObject m_promptPanel;

    public void InitClass()
    {

    }
    public void DisplayItems(LootableEntity target, List<InventoryItem> items)
    {
        Debug.Log("아이템 표시");
    }
    /// <summary>
    /// 추후 ItemViewer에서 표시하도록 처리, Interface를 활용 예정
    /// </summary>
    public void DisplayMessage()
    {
        Debug.Log("아이템이 없어요");
    }
    public void CloseLootUI()
    {
        Debug.Log("루팅 UI 닫기");
    }
}
