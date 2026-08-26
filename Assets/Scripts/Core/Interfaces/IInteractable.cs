
public interface IInteractable
{
    string GetInteractPrompt();
    void ShowPrompt();
    void HidePrompt();
    void Interact(PlayerInteract playerInteract);
}
