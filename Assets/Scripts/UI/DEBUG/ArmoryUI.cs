using UnityEngine;

public class ArmoryUI : MonoBehaviour, IInteractable
{
    public EnemySpawner m_enemySpawner;
    public bool m_toActive;

    public string GetInteractPrompt() => "";

    public void Interact(PlayerInteract interactor)
    {
        Debug.Log("World canvas button active!");

        TriggerButtonLogic();
    }

    public void SetInteractAllowance(bool allow)
    {

    }

    private void TriggerButtonLogic()
    {
        if (m_toActive)
            m_enemySpawner.StartEnemySpawn();
        else
            m_enemySpawner.StopEnemySpawn();
    }
}
