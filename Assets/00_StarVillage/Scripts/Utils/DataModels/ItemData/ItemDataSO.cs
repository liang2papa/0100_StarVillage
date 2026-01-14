using UnityEngine;

[CreateAssetMenu(fileName = "UnitDataSO", menuName = "Data/UnitDataSO")]
public class ItemDataSO : ScriptableObject
{
    public int UniqueID;
    public EItemCategory Category;
    public bool IsInteractable;
    public bool IsStackable;
    public bool IsConsumable;
}
