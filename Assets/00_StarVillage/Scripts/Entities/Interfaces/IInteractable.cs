using UnityEngine;

public interface IInteractable
{
    public string EntityName { get; }
    public bool IsInteractable { get; }

    public void OnInteract(Transform interactor);
}
