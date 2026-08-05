using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDataSO", menuName = "Player/Create player data")]
public class PlayerDataSO : ScriptableObject
{
    [Header("Model:")]
    public GameObject Model;

    [Header("Stats:")]
    public int MaxHealth;
    public float MovementSpeed;
}
