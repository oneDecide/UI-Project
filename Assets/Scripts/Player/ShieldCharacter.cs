using UnityEngine;
using System.Collections;

public class ShieldCharacter : CharacterBlueprint
{
    [Header("Shield Settings")]
    [SerializeField] private float shieldDuration = 3f;
    [SerializeField] private float shieldCooldown = 5f;
    [SerializeField] private GameObject shieldVisual;
    
    private bool shieldActive;
    private bool canUseShield = true;

    protected override void Update()
    {
        base.Update();
        HandleShield();
    }

    private void HandleShield()
    {
        if(Input.GetKeyDown(KeyCode.Q) && canUseShield)
        {
            StartCoroutine(ActivateShield());
        }
    }

    private IEnumerator ActivateShield()
    {
        canUseShield = false;
        shieldActive = true;
        shieldVisual.SetActive(true);
        
        yield return new WaitForSeconds(shieldDuration);
        
        shieldActive = false;
        shieldVisual.SetActive(false);
        yield return new WaitForSeconds(shieldCooldown);
        
        canUseShield = true;
    }
}