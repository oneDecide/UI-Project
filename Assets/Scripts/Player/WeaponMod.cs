using UnityEngine;

public abstract class WeaponMod : ScriptableObject
{
    public string modName;
    public Sprite icon;
    [TextArea] public string description;
    
    public abstract void ApplyMod(AssaultRifle weapon);
    public abstract void RemoveMod(AssaultRifle weapon);
}