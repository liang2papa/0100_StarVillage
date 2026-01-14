using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataSO", menuName = "Data/ItemDataSO")]
public class ItemDataSO : ScriptableObject
{
    public int UniqueID;
    public EItemCategory Category;
    public bool IsInteractable;
    public bool IsStackable;
    public bool IsConsumable;
}
