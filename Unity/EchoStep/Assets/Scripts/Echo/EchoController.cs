using UnityEngine;

/// <summary>
/// Individual echo behavior — waits, replays stored jump, becomes platform.
/// Uses a PlatformEffector2D for one-way collision.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class EchoController : MonoBehaviour
{
    [Header("Visuals")]
    public SpriteRenderer bodyRenderer;
    public SpriteRenderer eyeLeftRenderer;
    public SpriteRenderer eyeRightRenderer;
    public ParticleSystem replayParticles;

    [Header("One-way platform")]
    public PlatformEffector2D platformEffector;

    [HideInInspector] public bool isReplaying;
    [HideInInspector] public bool isFrozen;
    [HideInInspector] public bool isDead;
    [HideInInspector] public bool playerOnTop;

    private Rigidbody2D rb;
    private BoxCollider2D col;
    private Vector2 storedVelocity;
    private float timer;
    private float alpha;
    private bool hasReplayed;
    private float gracePeriod;

    public void Initialize(Vector2 velocity)
    {
        storedVelocity = velocity;
        timer = 0f;
        alpha = 0f;
        hasReplayed = false;
        isReplaying = false;
        isFrozen = false;
        isDead = false;
        playerOnTop = false;
        gracePeriod = GameConstants.EchoGracePeriod;

        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.freezeRotation = true;

        // Start with collider disabled during grace period
        col.enabled = false;

        // Platform effector for one-way collision
        if (platformEffector != null)
        {
            platformEffector.useOneWay = true;
            platformEffector.surfaceArc = 170f;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        alpha = Mathf.Min(alpha + Time.deltaTime * 3f, 1f);

        // Grace period
        if (gracePeriod > 0f)
        {
            gracePeriod -= Time.deltaTime;
            if (gracePeriod <= 0f)
                col.enabled = true;
        }

        // Sink when player is standing on top
        if (playerOnTop && !isReplaying)
        {
            transform.position += Vector3.down * GameConstants.EchoSinkSpeed * Time.deltaTime;
        }
        playerOnTop = false;

        // Start replay
        if (!hasReplayed && timer >= GameConstants.EchoReplayDelay)
        {
            hasReplayed = true;
            isReplaying = true;
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = GameConstants.Gravity / Physics2D.gravity.magnitude;
            rb.linearVelocity = storedVelocity;
            replayParticles?.Play();

            // During replay, disable one-way (echo moves freely)
            col.usedByEffector = false;
        }

        // Freeze after 2s of replay
        if (isReplaying && !isFrozen && timer >= GameConstants.EchoReplayDelay + 2f)
        {
            Freeze();
        }

        // Lifetime
        if (timer >= GameConstants.EchoLifetime || transform.position.y < -5f)
        {
            Kill();
        }

        // Visuals
        UpdateVisuals();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // When replaying, land on platforms and freeze
        if (isReplaying && !isFrozen)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                // Check if landing from above
                foreach (var contact in collision.contacts)
                {
                    if (contact.normal.y > 0.5f)
                    {
                        Freeze();
                        break;
                    }
                }
            }
        }

        // Check pressure plates
        var plate = collision.gameObject.GetComponent<PressurePlate>();
        if (plate != null)
        {
            plate.Activate();
            GameManager.Instance.Stats.echoesUsed++;
        }

        var timedPlate = collision.gameObject.GetComponent<TimedPlate>();
        if (timedPlate != null)
        {
            timedPlate.TryComplete();
            GameManager.Instance.Stats.echoesUsed++;
        }
    }

    private void Freeze()
    {
        isReplaying = false;
        isFrozen = true;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;

        // Re-enable one-way platform
        col.usedByEffector = true;
    }

    public void Kill()
    {
        isDead = true;
        // Death particles
        if (replayParticles != null)
        {
            replayParticles.transform.parent = null;
            var main = replayParticles.main;
            main.startColor = new Color(0f, 0.8f, 1f, 0.5f);
            replayParticles.Emit(10);
            Destroy(replayParticles.gameObject, 2f);
        }
        Destroy(gameObject);
    }

    private void UpdateVisuals()
    {
        float displayAlpha = isReplaying ? 0.7f : 0.35f * alpha;
        Color c = isReplaying
            ? new Color(0f, 0.93f, 1f, displayAlpha)
            : new Color(0f, 0.6f, 0.8f, displayAlpha);
        bodyRenderer.color = c;

        Color eyeColor = new Color(1f, 1f, 1f, displayAlpha * 0.8f);
        if (eyeLeftRenderer) eyeLeftRenderer.color = eyeColor;
        if (eyeRightRenderer) eyeRightRenderer.color = eyeColor;
    }
}
