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
    public AnimationClip anim;

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
    public float attackAnimationLength = .8f;

    public Vector2 DA_BoxSize_Left = new Vector2(2f, 2f);
    public float DA_CastDistance_Left = 0f;

    public Vector2 DA_BoxSize_Right = new Vector2(2f, 2f);
    public float DA_CastDistance_Right = 0f;
    public float DA_CastDistance_Vertical = 0f;

    public bool hasDefaultAttacked = false;

    [SerializeField] public GameObject GameoverCanvas = null;

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
        leftSwordAnimator = leftSword.GetComponent<Animator>();
        rightSwordAnimator = rightSword.GetComponent<Animator>();
        //play the "WithoutSwords" animation
        animator.Play("WithoutSwords");

        //set anim to the right sword animator's current animation
        anim = rightSwordAnimator.runtimeAnimatorController.animationClips[0];



        //duration of animation should match attack interval
        
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // defaultAttackTimer += Time.deltaTime;
        // if(defaultAttackTimer >= defaultAttackInterval)
        // {
        //     //attack
        //     defaultAttackTimer = 0f;
            
        // }
        if (hasDefaultAttacked == true && (rightSwordAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime % anim.length) <= anim.length*.2f) {
            hasDefaultAttacked = false;
        }
        if(hasDefaultAttacked == false && (rightSwordAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime % anim.length) >= anim.length*.9f){
            // Debug.Log("animator current time: " + rightSwordAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);
            // Debug.Log("animator length: " + anim.length);
            // Debug.Log("Attack Animation Finished");
            hasDefaultAttacked = true;
            DoAttacks();

        }


    }

    void FixedUpdate(){
        rb.linearVelocity = new Vector2(currentInputMovmentDir.x * speed, currentInputMovmentDir.y * speed);

        

        
    }

    public void TakeDamage(int damage)
    {
        healthScript.takeDamage(damage);
        if (healthScript.getHP() <= 0)
        {
            
            //gameover
            GameoverCanvas.SetActive(true);

            Destroy(gameObject);
        }
    }


    public void DoAttacks(){
        Debug.Log("Attacking with sword");
        //raycast to check if there is an enemy in front of the sword
        RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position - transform.right * DA_CastDistance_Left - transform.up * DA_CastDistance_Vertical, DA_BoxSize_Left, 0f, Vector2.zero, 0f, LayerMask.GetMask("Enemies"));
        RaycastHit2D[] hits2 = Physics2D.BoxCastAll(transform.position - transform.right * DA_CastDistance_Right - transform.up * DA_CastDistance_Vertical, DA_BoxSize_Right, 0f, Vector2.zero, 0f, LayerMask.GetMask("Enemies"));

        foreach (RaycastHit2D hit in hits){
            if(hit.collider.gameObject != this.gameObject && hit.collider.gameObject.GetComponent<babySlime>())
            {
                babySlime slime = hit.collider.gameObject.GetComponent<babySlime>();
                slime.TakeDamage(3, new Vector2(-1, 0), 4f);
            }
        }
        foreach (RaycastHit2D hit in hits2){
            if(hit.collider.gameObject != this.gameObject && hit.collider.gameObject.GetComponent<babySlime>())
            {
                babySlime slime = hit.collider.gameObject.GetComponent<babySlime>();
                slime.TakeDamage(3, new Vector2(1, 0), 4f);
            }
        }

    }   

    void OnMovement(InputValue value)
    {
        //debug.log("Movement: " + value.Get<Vector2>());
        currentInputMovmentDir = value.Get<Vector2>();
        //need to give this movement a deadzone so if the the abxolute value of x or y is less than .1 then it is 0
        currentInputMovmentDir.x = Mathf.Abs(currentInputMovmentDir.x) < .6 ? 0 : currentInputMovmentDir.x;
        currentInputMovmentDir.y = Mathf.Abs(currentInputMovmentDir.y) < .6 ? 0 : currentInputMovmentDir.y;
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position - transform.right * DA_CastDistance_Left - transform.up * DA_CastDistance_Vertical, DA_BoxSize_Left);
        Gizmos.DrawWireCube(transform.position - transform.right * DA_CastDistance_Right - transform.up * DA_CastDistance_Vertical, DA_BoxSize_Right);
    }
}
