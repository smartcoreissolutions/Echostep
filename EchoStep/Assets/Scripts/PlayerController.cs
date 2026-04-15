using UnityEngine;

/// <summary>
/// Single-button player: hold to charge, release to jump.
/// Facing direction follows last horizontal input or jump direction.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Jump")]
    public float minJumpForce = 4f;
    public float maxJumpForce = 14f;
    public float maxChargeTime = 1.2f;
    public float jumpAngle = 65f;          // degrees from horizontal

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayers;

    [Header("References")]
    public ChargeBarUI chargeBar;

    // --- Runtime ---
    Rigidbody2D rb;
    bool isGrounded;
    bool isCharging;
    float chargeTimer;
    int facingDir = 1;                     // 1 = right, -1 = left

    // Exposed for EchoManager
    [HideInInspector] public Vector2 lastJumpVelocity;
    [HideInInspector] public bool justLanded;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        CheckGround();
        HandleInput();
    }

    void CheckGround()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(
            groundCheck ? groundCheck.position : (Vector3)((Vector2)transform.position + Vector2.down * 0.5f),
            groundCheckRadius, groundLayers);

        // Detect landing frame
        if (!wasGrounded && isGrounded && rb.linearVelocity.y <= 0.1f)
        {
            justLanded = true;
        }
    }

    void HandleInput()
    {
        bool pressed = Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0);
        bool released = Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(0);

        if (pressed && isGrounded && !isCharging)
        {
            isCharging = true;
            chargeTimer = 0f;
        }

        if (isCharging)
        {
            chargeTimer += Time.deltaTime;
            chargeTimer = Mathf.Min(chargeTimer, maxChargeTime);
            if (chargeBar) chargeBar.SetFill(chargeTimer / maxChargeTime);
        }

        if (released && isCharging)
        {
            Jump();
            isCharging = false;
            if (chargeBar) chargeBar.SetFill(0f);
        }
    }

    void Jump()
    {
        float t = chargeTimer / maxChargeTime;
        float force = Mathf.Lerp(minJumpForce, maxJumpForce, t);

        float angle = jumpAngle * Mathf.Deg2Rad;
        Vector2 dir = new Vector2(Mathf.Cos(angle) * facingDir, Mathf.Sin(angle)).normalized;

        rb.linearVelocity = Vector2.zero;
        Vector2 jumpVel = dir * force;
        rb.AddForce(jumpVel, ForceMode2D.Impulse);

        lastJumpVelocity = jumpVel;

        // Track stats
        if (GameManager.Instance) GameManager.Instance.stats.totalLandings++;
    }

    /// <summary>Called externally (e.g. by echo boost or shockwave).</summary>
    public void ApplyExternalForce(Vector2 force)
    {
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    /// <summary>Flip facing direction (tap or auto-detect).</summary>
    public void SetFacing(int dir)
    {
        facingDir = dir;
        Vector3 s = transform.localScale;
        s.x = Mathf.Abs(s.x) * dir;
        transform.localScale = s;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // Auto-face away from walls
        if (col.contacts.Length > 0)
        {
            float nx = col.contacts[0].normal.x;
            if (Mathf.Abs(nx) > 0.7f)
                SetFacing(nx > 0 ? 1 : -1);
        }
    }

    void LateUpdate()
    {
        // Reset per-frame flags at end of frame
        justLanded = false;
    }

    void OnDrawGizmosSelected()
    {
        Vector3 pos = groundCheck ? groundCheck.position : transform.position + Vector3.down * 0.5f;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(pos, groundCheckRadius);
    }
}
