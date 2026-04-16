using UnityEngine;

/// <summary>
/// Moving hazard — patrols between point A and point B.
/// </summary>
public class MovingHazard : MonoBehaviour
{
    [Header("Patrol")]
    public Vector2 pointA;
    public Vector2 pointB;
    public float speed = 1.5f;

    [Header("Visuals")]
    public SpriteRenderer hazardRenderer;
    public ParticleSystem dangerParticles;

    private bool goingB = true;

    void Start()
    {
        if (pointA == Vector2.zero) pointA = transform.position;
        gameObject.tag = "Hazard";
    }

    void Update()
    {
        Vector2 target = goingB ? pointB : pointA;
        Vector2 pos = transform.position;
        float dist = Vector2.Distance(pos, target);

        if (dist < 0.02f)
        {
            goingB = !goingB;
        }
        else
        {
            Vector2 dir = (target - pos).normalized;
            transform.position = pos + dir * speed * Time.deltaTime;
        }

        // Pulsing glow
        if (hazardRenderer != null)
        {
            float pulse = Mathf.Sin(Time.time * 6f) * 0.3f + 0.7f;
            hazardRenderer.color = new Color(1f, 0.15f, 0.15f, pulse);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.player.Die();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.player.Die();
        }
    }
}
