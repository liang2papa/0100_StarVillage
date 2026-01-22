using UnityEngine;

public class ContainerEntity : LootableEntity
{
    public ItemDataSO ItemData;

    private void Start()
    {
        Debug.Log(ItemData.ItemName);
    }
}
