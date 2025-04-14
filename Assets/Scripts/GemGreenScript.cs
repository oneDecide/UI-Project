using UnityEngine;

public class GemGreenScript : MonoBehaviour
{
    //create hitbox
    public Vector2 DA_BoxSize_Left = new Vector2(2f, 2f);
    public float DA_CastDistance_Left = 0f;
    public float DA_CastDistance_Vertical = 0f;

    public GameObject gemCounter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //get gem counter with tag GemCounter
        gemCounter = GameObject.FindGameObjectWithTag("GemCounter");

    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            transform.position - transform.right * DA_CastDistance_Left - transform.up * DA_CastDistance_Vertical,
            DA_BoxSize_Left,
            0f,
            Vector2.zero,
            0f,
            LayerMask.GetMask("GemCollector")
        );

        if (hit.collider != null)
        {
            Debug.Log("Hit: " + hit.collider.name);

            //TODO: logic for handling gem collection goes here
            gemCounter.GetComponent<GemCounter>().AddGems(1); //add 1 gem to the counter
            

            //destroy this object
            Destroy(gameObject);
        }
    }
    void OnDrawGizmos(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position - transform.right * DA_CastDistance_Left - transform.up * DA_CastDistance_Vertical, DA_BoxSize_Left);

    }
    
}
