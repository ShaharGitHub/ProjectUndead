using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        ModifyCursorStates();
    }

    private void ModifyCursorStates()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
