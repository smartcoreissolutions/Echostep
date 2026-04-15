using UnityEngine;

/// <summary>
/// Echo behavior:
/// - Waits 2 seconds, then replays the stored jump velocity.
/// - Acts as a platform while stationary.
/// - Gives speed boost when touched by player from the side.
/// - Triggers shockwave when touching another echo.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Echo : MonoBehaviour
{
    [Header("Config")]
    public float replayDelay = 2f;
    public float platformSinkSpeed = 0.3f;
    public float boostMultiplier = 3f;
    public float lifetime = 8f;
    public GameObject shockwavePrefab;

    // --- Runtime ---
    Vector2 storedVelocity;
    Rigidbody2D rb;
    float timer;
    bool hasReplayed;
    bool isDestroyed;
    float sinkOffset;

    // Track whether player is standing on this echo
    bool playerOnTop;

    public void Init(Vector2 jumpVelocity)
    {
        storedVelocity = jumpVelocity;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        // Semi-transparent appearance
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color c = sr.color;
            c.a = 0.45f;
            sr.color = c;
        }
    }

    void Update()
    {
        if (isDestroyed) return;

        timer += Time.deltaTime;

        // Platform sink when player stands on it
        if (playerOnTop)
        {
            sinkOffset += platformSinkSpeed * Time.deltaTime;
            transform.position += Vector3.down * platformSinkSpeed * Time.deltaTime;
        }

        // Replay after delay
        if (!hasReplayed && timer >= replayDelay)
        {
            Replay();
        }

        // Lifetime
        if (timer >= lifetime)
        {
            DestroySelf();
        }
    }

    void Replay()
    {
        hasReplayed = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 3f;
        rb.linearVelocity = storedVelocity;

        // After replay lands, go kinematic again
        Invoke(nameof(FreezeAfterReplay), 2f);
    }

    void FreezeAfterReplay()
    {
        if (isDestroyed) return;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // Player stands on echo (contact from above)
        if (col.gameObject.CompareTag("Player"))
        {
            foreach (var contact in col.contacts)
            {
                if (contact.normal.y < -0.5f) // player is above
                {
                    playerOnTop = true;
                    break;
                }
                else
                {
                    // Side touch → speed boost
                    GiveBoost(col.gameObject);
                }
            }
        }

        // Echo-to-echo collision → shockwave
        if (col.gameObject.CompareTag("Echo"))
        {
            TriggerShockwave(col.gameObject);
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            playerOnTop = false;
        }
    }

    void GiveBoost(GameObject playerObj)
    {
        PlayerController pc = playerObj.GetComponent<PlayerController>();
        if (pc != null)
        {
            Vector2 boostDir = (playerObj.transform.position - transform.position).normalized;
            boostDir.y = 0; // horizontal only
            pc.ApplyExternalForce(boostDir.normalized * boostMultiplier);

            if (GameManager.Instance) GameManager.Instance.stats.echoesUsed++;
        }
    }

    void TriggerShockwave(GameObject otherEcho)
    {
        Vector2 midpoint = (transform.position + otherEcho.transform.position) / 2f;

        // Spawn shockwave
        if (shockwavePrefab != null)
        {
            Instantiate(shockwavePrefab, midpoint, Quaternion.identity);
        }
        else
        {
            // Fallback: create a simple shockwave
            GameObject sw = new GameObject("Shockwave");
            sw.transform.position = midpoint;
            sw.AddComponent<ShockwaveEffect>();
            CircleCollider2D col = sw.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 2f;
            Destroy(sw, 0.5f);
        }

        if (GameManager.Instance)
        {
            GameManager.Instance.stats.shockwavesTriggered++;
            GameManager.Instance.stats.echoesUsed += 2;
        }

        // Destroy both echoes
        Echo otherEchoScript = otherEcho.GetComponent<Echo>();
        if (otherEchoScript != null) otherEchoScript.DestroySelf();
        DestroySelf();
    }

    void DestroySelf()
    {
        if (isDestroyed) return;
        isDestroyed = true;

        if (EchoManager.Instance) EchoManager.Instance.RemoveEcho(this);
        Destroy(gameObject);
    }
}
