using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDataSO", menuName = "Enemy/Create enemy data")]
public class EnemyDataSO : ScriptableObject
{
    [Header("Model:")]
    public GameObject Model;

    [Header("Health:")]
    public int MaxHealth;

    [Header("Movement:")]
    public float WalkSpeed;
    public float SprintSpeed;
    public float JumpHeight;
}
