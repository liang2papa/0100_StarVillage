using UnityEngine;

public class NPCCoordinator : MonoBehaviour, IInteractable
{
    [field: SerializeField] public string EntityName { get; private set; }
    [field: SerializeField] public bool IsInteractable { get; private set; }
    [field: SerializeField] public string InteractionPrompt { get; private set; } = "<color=yellow>[E]</color>";

    public void OnInteract(Transform interactor)
    {
        // NPC와 상호작용하는 로직 구현
        Debug.Log($"Interacted with NPC: {EntityName}");
    }

}
