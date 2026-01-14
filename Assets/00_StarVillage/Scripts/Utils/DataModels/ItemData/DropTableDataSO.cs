using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "DropTableDataSO", menuName = "Data/DropTableDataSO")]
public class DropTableDataSO : SerializedScriptableObject
{
    public List<InventoryItem> DropTable = new();
    


}
