using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyFollower : MonoBehaviour
{
    private Rigidbody rb;
    private FollowerEntity followerEntity;

    [Header("Movement Settings")]
    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        followerEntity = GetComponent<FollowerEntity>();
    }

    void FixedUpdate()
    {
        // 1. Sync the AI's internal tracking with the physical Rigidbody position
        followerEntity.position = rb.position;
        followerEntity.rotation = rb.rotation;

        // 2. Get the direction to the next path node
        Vector3 nextWaypoint = followerEntity.steeringTarget;
        Vector3 direction = (nextWaypoint - rb.position);

        // Zero out the Y axis so the AI doesn't try to "fly" upward or downward into the floor
        direction.y = 0;
        Vector3 forceDirection = direction.normalized;

        // 3. Apply physics velocity (preserving existing gravity/vertical forces)
        Vector3 targetVelocity = forceDirection * moveSpeed;
        targetVelocity.y = rb.linearVelocity.y;

        rb.linearVelocity = targetVelocity;

        // 4. Smoothly rotate towards the movement direction
        if (forceDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(forceDirection);
            Quaternion nextRotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(nextRotation);
        }
    }
}