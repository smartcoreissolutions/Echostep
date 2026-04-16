using UnityEngine;

/// <summary>
/// Player controller — charge-jump, ground detection, squash/stretch.
/// Uses a Rigidbody2D but overrides velocity for tight control.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Visuals")]
    public SpriteRenderer bodyRenderer;
    public SpriteRenderer eyeLeftRenderer;
    public SpriteRenderer eyeRightRenderer;
    public Transform arrowIndicator;
    public ParticleSystem jumpParticles;
    public ParticleSystem landParticles;
    public ParticleSystem chargeParticles;
    public ParticleSystem trailParticles;

    [Header("Colors")]
    public Color normalColor = new Color(0f, 0.87f, 1f);
    public Color chargingColor = new Color(1f, 0.67f, 0f);

    // State
    [HideInInspector] public int facing = 1;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool isCharging;
    [HideInInspector] public float chargeTime;
    [HideInInspector] public bool justLanded;
    [HideInInspector] public bool isDead;
    [HideInInspector] public Vector2 lastJumpVelocity;

    private Rigidbody2D rb;
    private BoxCollider2D col;
    private bool wasGrounded;
    private float squashY = 1f;
    private float stretchX = 1f;
    private Vector3 baseScale;
    private float blinkTimer;

    // Ground check
    [Header("Ground Check")]
    public LayerMask groundLayer;
    public float groundCheckDist = 0.05f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        rb.gravityScale = 0; // we handle gravity manually
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        baseScale = transform.localScale;
    }

    void Update()
    {
        if (isDead) return;

        InputHelper.Tick();

        // Arrow keys for direction
        int arrow = InputHelper.ArrowDirection();
        if (arrow != 0) facing = arrow;

        // Restart
        if (InputHelper.RestartPressed())
            GameManager.Instance.RestartLevel();

        // Update visuals
        UpdateSquashStretch();
        UpdateEyes();
        UpdateArrow();
        UpdateColors();
    }

    void FixedUpdate()
    {
        if (isDead) return;

        float dt = Time.fixedDeltaTime;
        Vector2 vel = rb.linearVelocity;

        // Gravity
        if (!isGrounded)
            vel.y -= GameConstants.Gravity * dt;

        // Friction
        if (isGrounded)
        {
            vel.x *= Mathf.Pow(GameConstants.GroundFrictionDecay, dt);
            if (Mathf.Abs(vel.x) < 0.01f) vel.x = 0f;
        }
        else
        {
            vel.x *= (1f - GameConstants.AirDrag * dt);
        }

        rb.linearVelocity = vel;

        // Ground check
        CheckGround();

        // Landing detection
        if (!wasGrounded && isGrounded)
        {
            justLanded = true;
            GameManager.Instance.Stats.landings++;
            OnLand();
        }
        else
        {
            justLanded = false;
        }
        wasGrounded = isGrounded;

        // Input
        if (GameManager.Instance.IsInputBlocked())
        {
            InputHelper.Clear();
            return;
        }

        HandleInput(dt);

        // Fall death
        if (transform.position.y < -5f)
            Die();
    }

    private void HandleInput(float dt)
    {
        // Tap to flip
        if (InputHelper.IsTap && isGrounded)
        {
            facing *= -1;
            SFXManager.Instance?.Play(SFXManager.Clip.Flip);
            SpawnFlipParticles();
        }

        // Start charging
        if (InputHelper.IsDown && isGrounded && !isCharging)
        {
            isCharging = true;
            chargeTime = 0f;
            chargeParticles?.Play();
        }

        // Continue charging
        if (isCharging)
        {
            chargeTime += dt;
            if (chargeTime > GameConstants.MaxChargeTime)
                chargeTime = GameConstants.MaxChargeTime;

            // Squash during charge
            float t = chargeTime / GameConstants.MaxChargeTime;
            squashY = 1f + t * 0.3f;
            stretchX = 1f - t * 0.15f;
        }

        // Release = jump
        if (InputHelper.JustUp && isCharging)
        {
            Jump();
        }
    }

    private void Jump()
    {
        float t = chargeTime / GameConstants.MaxChargeTime;
        float force = GameConstants.MinJump + (GameConstants.MaxJump - GameConstants.MinJump) * t;
        float angleRad = GameConstants.JumpAngleDeg * Mathf.Deg2Rad;

        Vector2 jumpVel = new Vector2(
            Mathf.Cos(angleRad) * force * facing,
            Mathf.Sin(angleRad) * force
        );

        rb.linearVelocity = jumpVel;
        isGrounded = false;
        isCharging = false;
        chargeTime = 0f;
        lastJumpVelocity = jumpVel;

        // Stretch on jump
        squashY = 0.6f;
        stretchX = 1.4f;

        SFXManager.Instance?.Play(SFXManager.Clip.Jump);
        GameManager.Instance.cameraController?.Shake(3f);
        jumpParticles?.Play();
        chargeParticles?.Stop();
        trailParticles?.Play();
    }

    private void OnLand()
    {
        squashY = 1.4f;
        stretchX = 0.6f;
        SFXManager.Instance?.Play(SFXManager.Clip.Land);
        GameManager.Instance.cameraController?.Shake(4f);
        landParticles?.Play();
        trailParticles?.Stop();
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        SFXManager.Instance?.Play(SFXManager.Clip.Die);
        GameManager.Instance.cameraController?.Shake(12f);
        // Death particles
        if (jumpParticles != null)
        {
            var main = jumpParticles.main;
            main.startColor = Color.red;
            jumpParticles.Emit(25);
        }
        bodyRenderer.enabled = false;
        eyeLeftRenderer.enabled = false;
        eyeRightRenderer.enabled = false;
        Invoke(nameof(RestartAfterDeath), 1f);
    }

    private void RestartAfterDeath()
    {
        GameManager.Instance.RestartLevel();
    }

    public void Spawn(Vector2 position)
    {
        transform.position = position;
        rb.linearVelocity = Vector2.zero;
        facing = 1;
        isGrounded = false;
        isCharging = false;
        chargeTime = 0f;
        isDead = false;
        justLanded = false;
        wasGrounded = true; // prevent false landing on first frame
        lastJumpVelocity = Vector2.zero;
        squashY = 1f;
        stretchX = 1f;
        bodyRenderer.enabled = true;
        eyeLeftRenderer.enabled = true;
        eyeRightRenderer.enabled = true;
        chargeParticles?.Stop();
        trailParticles?.Stop();
    }

    private void CheckGround()
    {
        isGrounded = false;
        Vector2 origin = (Vector2)transform.position + Vector2.down * (col.size.y * 0.5f);
        float halfWidth = col.size.x * 0.4f;

        // Two raycasts from feet corners
        RaycastHit2D hitL = Physics2D.Raycast(origin + Vector2.left * halfWidth, Vector2.down, groundCheckDist, groundLayer);
        RaycastHit2D hitR = Physics2D.Raycast(origin + Vector2.right * halfWidth, Vector2.down, groundCheckDist, groundLayer);

        if ((hitL.collider != null || hitR.collider != null) && rb.linearVelocity.y <= 0.01f)
        {
            isGrounded = true;
            // Snap to surface
            float surfaceY = Mathf.Max(
                hitL.collider != null ? hitL.point.y : -999f,
                hitR.collider != null ? hitR.point.y : -999f
            );
            Vector2 pos = transform.position;
            pos.y = surfaceY + col.size.y * 0.5f;
            transform.position = pos;

            Vector2 vel = rb.linearVelocity;
            vel.y = 0f;
            rb.linearVelocity = vel;
        }
    }

    private void UpdateSquashStretch()
    {
        squashY = Mathf.Lerp(squashY, 1f, 8f * Time.deltaTime);
        stretchX = Mathf.Lerp(stretchX, 1f, 8f * Time.deltaTime);
        transform.localScale = new Vector3(
            baseScale.x * facing * stretchX,
            baseScale.y * squashY,
            baseScale.z
        );
    }

    private void UpdateEyes()
    {
        blinkTimer += Time.deltaTime;
        // Blink every 3 seconds for 0.1s
        bool blinking = (blinkTimer % 3f) < 0.1f;
        if (eyeLeftRenderer != null)
        {
            eyeLeftRenderer.transform.localScale = blinking
                ? new Vector3(1f, 0.2f, 1f)
                : Vector3.one;
        }
        if (eyeRightRenderer != null)
        {
            eyeRightRenderer.transform.localScale = blinking
                ? new Vector3(1f, 0.2f, 1f)
                : Vector3.one;
        }
    }

    private void UpdateArrow()
    {
        if (arrowIndicator == null) return;
        arrowIndicator.gameObject.SetActive(isGrounded && !isCharging);
    }

    private void UpdateColors()
    {
        if (bodyRenderer == null) return;
        bodyRenderer.color = Color.Lerp(
            bodyRenderer.color,
            isCharging ? chargingColor : normalColor,
            10f * Time.deltaTime
        );
    }

    private void SpawnFlipParticles()
    {
        if (jumpParticles != null)
        {
            var main = jumpParticles.main;
            main.startColor = new Color(0.53f, 0.87f, 1f);
            jumpParticles.Emit(3);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Goal zone
        if (collision.gameObject.CompareTag("Goal"))
        {
            GameManager.Instance.CompleteLevel();
        }
        // Hazard
        if (collision.gameObject.CompareTag("Hazard"))
        {
            Die();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Goal zone (if trigger)
        if (other.CompareTag("Goal"))
        {
            GameManager.Instance.CompleteLevel();
        }
        // Hazard
        if (other.CompareTag("Hazard"))
        {
            Die();
        }
        // Crystal
        if (other.CompareTag("Crystal"))
        {
            GameManager.Instance.EchoCharges = Mathf.Min(
                GameManager.Instance.EchoCharges + 1,
                GameManager.Instance.MaxEchoes
            );
            SFXManager.Instance?.Play(SFXManager.Clip.Crystal);
            Destroy(other.gameObject);
        }
    }
}
