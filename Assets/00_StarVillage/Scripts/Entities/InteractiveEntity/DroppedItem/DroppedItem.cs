using UnityEngine;

public class DroppedItem : WorldEntity
{
    [SerializeField] protected ItemDataSO m_unitData;

    [SerializeField] protected EItemCategory m_name;

    [SerializeField] protected bool m_isInteractable;

    [SerializeField] protected bool m_isStackable;

    [SerializeField] protected bool m_isConsumable;
}
