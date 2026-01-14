using UnityEngine;

/// <summary>
/// 유닛은 인벤토리에 소지할 수 있거나, 월드에 오브젝트로 존재할  수 있는 모든 개체의 기본 클래스
/// </summary>
public abstract class WorldEntity : MonoBehaviour
{
    [SerializeField] protected int m_uniqueID;
}
