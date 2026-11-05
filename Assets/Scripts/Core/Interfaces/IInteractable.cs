
public interface IInteractable
{
    string GetInteractPrompt();
    void Interact(PlayerInteract playerInteract);
    public void SetInteractAllowance(bool allow);
}
