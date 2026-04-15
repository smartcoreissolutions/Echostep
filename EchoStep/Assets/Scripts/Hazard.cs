using UnityEngine;

/// <summary>
/// Kill zone: restarts the level when the player touches it.
/// Can optionally move back and forth (Level 5 moving hazard).
/// </summary>
public class Hazard : MonoBehaviour
{
    [Header("Moving Hazard (optional)")]
    public bool moves = false;
    public Transform movePointA;
    public Transform movePointB;
    public float moveSpeed = 3f;

    bool goingToB = true;

    void Update()
    {
        if (!moves || movePointA == null || movePointB == null) return;

        Transform target = goingToB ? movePointB : movePointA;
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.05f)
            goingToB = !goingToB;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
                GameManager.Instance.RestartLevel();
        }
    }
}
