using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine;
using Unity.Collections;

public class characterController : MonoBehaviour
{
    //get health script
    private EntityHealth healthScript;


    // game objects for left and right swords
    [Header("Sword GameObjects")]
    [SerializeField] public GameObject leftSword;
    [SerializeField] public GameObject rightSword;
    // main animator for the character
    [Header("Animators")]

    [SerializeField] public Animator animator;

    // animator for the left sword
    [Header("Sword Animators")]
    [SerializeField] public Animator leftSwordAnimator;
    // animator for the right sword
    [SerializeField] public Animator rightSwordAnimator;

    [Header("General Attributes")]
    [SerializeField] public int speed = 9;

    public Vector2 currentInputMovmentDir = Vector2.zero;
    private Rigidbody2D rb;

    public float defaultAttackTimer = 0f;
    public float defaultAttackInterval = .8f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the EntityHealth component from the GameObject
        healthScript = GetComponent<EntityHealth>();
        if (healthScript == null)
        {
            //debug.logError("EntityHealth component not found on this GameObject.");
        }
        animator = GetComponent<Animator>();
        //play the "WithoutSwords" animation
        animator.Play("WithoutSwords");
        //duration of animation should match attack interval
        
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        defaultAttackTimer += Time.deltaTime;
        if(defaultAttackTimer >= defaultAttackInterval)
        {
            //attack
            defaultAttackTimer = 0f;
            
        }
    }

    void FixedUpdate(){
        rb.linearVelocity = new Vector2(currentInputMovmentDir.x * speed, currentInputMovmentDir.y * speed);

        

        DoAttacks();
    }

    public void TakeDamage(int damage)
    {
        healthScript.takeDamage(damage);
        if (healthScript.getHP() <= 0)
        {
            Destroy(gameObject);
        }
    }


    public void DoAttacks(){
        //debug.log("DoAttacks() called");
    }   

    void OnMovement(InputValue value)
    {
        //debug.log("Movement: " + value.Get<Vector2>());
        currentInputMovmentDir = value.Get<Vector2>();
        //need to give this movement a deadzone so if the the abxolute value of x or y is less than .1 then it is 0
        currentInputMovmentDir.x = Mathf.Abs(currentInputMovmentDir.x) < .6 ? 0 : currentInputMovmentDir.x;
        currentInputMovmentDir.y = Mathf.Abs(currentInputMovmentDir.y) < .6 ? 0 : currentInputMovmentDir.y;
    }

}
