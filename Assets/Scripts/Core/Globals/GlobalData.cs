public enum EnemyStates { Idle = 0, Walk = 1, Run = 2, Attack = 3, Dead = 4 }
public enum FireModes { Auto, Semi, Burst }
public enum VfxTypes
{ 
    MuzzleFlash,
    BulletImpact,
    GranadeExplode,
    SmokeExplode,
}

public static class GlobalData
{
    public static class Prompts
    {
        public const string Interact = "Press <color=#BDFF47>'E'</color> to pick up\n";
        public const string Price = "Price: ";
    }
}
