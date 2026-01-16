using UnityEngine;

public class NPCCoordinator : MonoBehaviour, IInteractable
{
    public string EntityName { get; private set; }
    public bool IsInteractable { get; private set; }

    public void OnInteract(Transform interactor)
    {
        // NPC와 상호작용하는 로직 구현
        Debug.Log($"Interacted with NPC: {EntityName}");
    }

}
