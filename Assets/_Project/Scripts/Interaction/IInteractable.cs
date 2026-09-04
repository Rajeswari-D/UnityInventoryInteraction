public interface IInteractable
{
    string GetInteractionPrompt();

    bool CanInteract();

    bool Interact();
}