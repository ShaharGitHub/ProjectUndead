using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDataSO", menuName = "Player/Create player data")]
public class PlayerDataSO : ScriptableObject
{
    public int MaxHealth;
    public float MovementSpeed;
}
