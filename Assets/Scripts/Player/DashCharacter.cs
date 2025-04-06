using UnityEngine;
using System.Collections;

public class DashCharacter : CharacterBlueprint
{
    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 10f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 2f;
    
    private bool canDash = true;

    protected override void Update()
    {
        base.Update();
        HandleDash();
    }

    private void HandleDash()
    {
        if(Input.GetKeyDown(KeyCode.Space) && canDash)
        {
            StartCoroutine(PerformDash());
        }
    }

    private IEnumerator PerformDash()
    {
        canDash = false;
        float originalSpeed = moveSpeed;
        moveSpeed += dashForce;
        
        yield return new WaitForSeconds(dashDuration);
        
        moveSpeed = originalSpeed;
        yield return new WaitForSeconds(dashCooldown);
        
        canDash = true;
    }
}