using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Settings")]
    [Range(0.01f, 1f)] public float smoothSpeed = 0.125f;
    
    private Transform target = null;
    private Vector3 offset;
    private Vector3 _velocity = Vector3.zero;

    public void FindPlayerTarget()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player != null)
        {
            Debug.Log("aquired");
            target = player.transform;
            offset = transform.position - target.position;
        }
    }

    void LateUpdate()
    {
        if(target == null){
            FindPlayerTarget();
        }
        if(target != null)
        {
            Vector3 targetPosition = target.position + offset;
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref _velocity,
                smoothSpeed
            );
        }
    }
}