using UnityEngine;
/// [Lv 3] 객체의 행동 조율
/// 1. 각 UI Viewer의 행동을 지시
/// </summary>
public class UICoordinator : MonoBehaviour
{
    /// <summary>
    /// 추후 ItemViewer로 받아서 표시하도록 처리
    /// </summary>
    public void DisplayItems()
    {
        Debug.Log("아이템 획득");
    }
    /// <summary>
    /// 추후 ItemViewer로 받아서 표시하도록 처리
    /// </summary>
    public void DisplayMessage()
    {
        Debug.Log("아이템이 없어요");
    }
}
