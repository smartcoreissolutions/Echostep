using UnityEngine;

/// <summary>
/// Moves between two points when activated.
/// Wire Activate() / Deactivate() to PressurePlate events.
/// </summary>
public class MovingPlatform : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;
    public bool activeOnStart = false;

    bool moving;
    bool goingToB = true;

    void Start()
    {
        if (activeOnStart) Activate();
    }

    void Update()
    {
        if (!moving) return;

        Transform target = goingToB ? pointB : pointA;
        if (target == null) return;

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.05f)
        {
            goingToB = !goingToB;
        }
    }

    public void Activate()
    {
        moving = true;
    }

    public void Deactivate()
    {
        moving = false;
    }

    // Keep player/echo on platform
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player") || col.gameObject.CompareTag("Echo"))
        {
            col.transform.SetParent(transform);
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player") || col.gameObject.CompareTag("Echo"))
        {
            col.transform.SetParent(null);
        }
    }
}
