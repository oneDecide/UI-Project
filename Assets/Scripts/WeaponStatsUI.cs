using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class WeaponStatsUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AssaultRifle weapon;
    [SerializeField] private TMP_Text[] statTexts = new TMP_Text[8]; // Assign in inspector: StatText1 to StatText8

    [Header("Colors")]
    [SerializeField] private Color defaultColor = Color.yellow;
    [SerializeField] private Color modifiedColor = Color.green + (Color.yellow * 0.5f); // Green-yellow mix

    // Store base values for comparison
    private Dictionary<string, float> baseValues = new Dictionary<string, float>();

    private void Awake()
    {
        // Store the weapon's base values
        StoreBaseValues();
        
        // Initial update
        UpdateStatsUI();
    }

    private void OnEnable()
    {
        // Update whenever the UI becomes active
        UpdateStatsUI();
    }

    private void StoreBaseValues()
    {
        baseValues.Clear();
        baseValues.Add("Damage", weapon.damage);
        baseValues.Add("Penetration", weapon.punchThrough);
        baseValues.Add("Firerate", weapon.fireRate);
        baseValues.Add("CritRate", weapon.critChance);
        baseValues.Add("Multishot", weapon.multishot);
        baseValues.Add("CritMult", weapon.critMultiplier);
        baseValues.Add("Capacity", weapon.magazineCapacity);
        baseValues.Add("ReloadSpeed", weapon.reloadTime);
    }

    public void UpdateStatsUI()
    {
        if (weapon == null || statTexts.Length < 8) return;

        // Damage
        UpdateStatText(0, "Damage", weapon.damage, "DMG");
        
        // Penetration (Punch Through)
        UpdateStatText(1, "Penetration", weapon.punchThrough, "PEN");
        
        // Fire Rate
        UpdateStatText(2, "Firerate", weapon.fireRate, "FR", "0.0");
        
        // Critical Chance
        UpdateStatText(3, "CritRate", weapon.critChance * 100, "CRIT%", "0");
        
        // Multishot
        UpdateStatText(4, "Multishot", weapon.multishot, "MULTI", "0.0");
        
        // Critical Multiplier
        UpdateStatText(5, "CritMult", weapon.critMultiplier, "CRITx", "0.0");
        
        // Magazine Capacity
        UpdateStatText(6, "Capacity", weapon.magazineCapacity, "AMMO");
        
        // Reload Speed
        UpdateStatText(7, "ReloadSpeed", weapon.reloadTime, "RELOAD", "0.0s");
    }

    private void UpdateStatText(int index, string statName, float currentValue, string suffix, string format = "0")
    {
        if (index >= statTexts.Length || statTexts[index] == null) return;

        // Check if value has been modified from base
        bool isModified = false;
        if (baseValues.ContainsKey(statName))
        {
            isModified = !Mathf.Approximately(currentValue, baseValues[statName]);
        }

        // Set color based on modification
        statTexts[index].color = isModified ? modifiedColor : defaultColor;

        // Format the text
        if (statName == "CritRate")
        {
            // Special case for percentage
            statTexts[index].text = $"{currentValue.ToString(format)} {suffix}";
        }
        else if (statName == "ReloadSpeed")
        {
            // Special case for time with 's' suffix
            statTexts[index].text = $"{currentValue.ToString(format)}";
        }
        else
        {
            statTexts[index].text = $"{currentValue.ToString(format)} {suffix}";
        }
    }

    // Call this whenever mods are added/removed
    public void OnWeaponModsChanged()
    {
        UpdateStatsUI();
    }
}