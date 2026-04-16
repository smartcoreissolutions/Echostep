using UnityEngine;

/// <summary>
/// Shockwave effect — expands, launches player, then despawns.
/// </summary>
public class ShockwaveController : MonoBehaviour
{
    public SpriteRenderer ringRenderer;
    public float duration = GameConstants.ShockwaveDuration;
    public float maxRadius = GameConstants.ShockwaveRadius;

    private float timer;
    private PlayerController player;

    void Start()
    {
        player = GameManager.Instance.player;
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = timer / duration;
        float radius = t * maxRadius;

        // Scale the ring visual
        transform.localScale = Vector3.one * radius * 2f;

        // Fade out
        if (ringRenderer != null)
        {
            Color c = ringRenderer.color;
            c.a = 1f - t;
            ringRenderer.color = c;
        }

        // Launch player if nearby (only early in the shockwave)
        if (timer < 0.15f && player != null && !player.isDead)
        {
            float dist = Vector2.Distance(transform.position, player.transform.position);
            if (dist < radius)
            {
                var rb = player.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, GameConstants.ShockwaveLaunch);
                    player.isGrounded = false;
                }
            }
        }

        if (timer >= duration)
            Destroy(gameObject);
    }
}
