using UnityEngine;

/// <summary>
/// Smooth follow camera with optional vertical look-ahead.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0, 2, -10);
    public float lookAheadDistance = 1.5f;

    void LateUpdate()
    {
        if (target == null) return;

        Rigidbody2D rb = target.GetComponent<Rigidbody2D>();
        Vector3 lookAhead = Vector3.zero;
        if (rb != null)
        {
            lookAhead = new Vector3(rb.linearVelocity.x * 0.1f, Mathf.Max(rb.linearVelocity.y * 0.05f, 0), 0);
        }

        Vector3 desired = target.position + offset + lookAhead;
        transform.position = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);
    }
}
