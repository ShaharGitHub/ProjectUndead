using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        Application.targetFrameRate = 120;

        ModifyCursorStates();
    }

    private void ModifyCursorStates()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
