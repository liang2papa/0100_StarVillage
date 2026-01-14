using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 필드에서 상호작용할 수 있는 객체, ItemData를 소유
/// </summary>
public abstract class InteractiveEntity : MonoBehaviour
{
    public abstract void OnInteract(Transform interactor);
}
