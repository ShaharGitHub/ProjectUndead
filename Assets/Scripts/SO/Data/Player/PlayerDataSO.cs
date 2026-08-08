using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDataSO", menuName = "Player/Create player data")]
public class PlayerDataSO : ScriptableObject
{
    [Header("Model:")]
    public GameObject Model;

    [Header("Health:")]
    public int MaxHealth;

    [Header("Movement:")]
    public float MovementSpeed;
    public float LookSpeed;
}
