using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDataSO", menuName = "Player/Create player data")]
public class PlayerDataSO : ScriptableObject
{
    [Header("Model:")]
    public GameObject Model;

    [Header("Health:")]
    public int MaxHealth;

    [Header("Movement:")]
    public float LookSpeed;
    public float AimLookSpeed;
    public float WalkSpeed;
    public float SprintSpeed;
    public float JumpHeight;
}
