using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataSO", menuName = "Data/ItemDataSO")]
public abstract class ItemDataSO : ScriptableObject
{
    [Header("기본 정보")]
    public int UniqueID;           // 고유 ID
    public EItemType ItemType;
    public string ItemName;         // 로컬라이제이션 키 (예: ITEM_NAME_1001)
    public string Description;         // 로컬라이제이션 키
    public Sprite Icon;            // Addressable로 불러올 예정
    public string IconPath;     // 경로 참조

    [Header("아이템 티어")]
    public int Tier;

    [Header("거래 정보")]
    public bool IsPurchasable;     // 상점 등장 여부
    public int Price;              // 구매 가격 (판매가는 로직에서 Price * 0.2f 처리)

    [Header("스택 정보")]
    public bool IsStackable;       // 겹치기 가능 여부 (장비는 false)
    public int MaxStackSize;       // 최대 스택 (장비는 1)

    [Header("키 아이템")]
    public bool IsKeyItem;              // 키 아이템인지, 장비도 키 아이템이 될 수 있음
    public int KeyItemID;               // 어떤 임무에 사용되는 키 아이템인지?               

    [Header("무게")]
    public float Weight;                // 제한 중량보다 높아지면 로봇의 이동이 느려짐, 추후 반영

    [Header("분해 속성")]
    public bool IsDecomposable;     // 분해 가능한지
    public List<DecomposeData> DecomposeList = new List<DecomposeData>();
}
/// <summary>
/// 분해 시 나오는 아이템
/// </summary>
[System.Serializable]
public struct DecomposeData
{
    public int ResultItemID;
    public int MinCount;
    public int MaxCount;
}