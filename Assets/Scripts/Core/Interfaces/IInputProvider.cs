using System;
using UnityEngine;

[System.Serializable]
public class InputData
{
    public Vector2 Movement;
    public Vector2 Look;
    public bool Jump;
    public bool Aim;
    public bool Shoot;
    public bool Reload;
    public bool Melee;
    public bool Grenade;
    public bool Interact;
}

public interface IInputProvider
{
    public event Action<InputData> OnInputUpdated;
}
