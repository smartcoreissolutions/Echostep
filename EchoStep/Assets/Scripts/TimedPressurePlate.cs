using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Timed pressure plate: must be activated within a time window.
/// Used in Level 4 — echo must arrive within 1.5s of being "armed".
/// Arms when the level starts (or when manually triggered).
/// </summary>
public class TimedPressurePlate : MonoBehaviour
{
    public float timeWindow = 1.5f;
    public bool armOnStart = true;
    public UnityEvent onActivateSuccess;
    public UnityEvent onTimeout;
    public Color armedColor = Color.yellow;
    public Color successColor = Color.green;
    public Color failColor = Color.red;
    public Color inactiveColor = Color.gray;

    bool armed;
    bool completed;
    float armTimer;
    SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr) sr.color = inactiveColor;
    }

    void Start()
    {
        if (armOnStart) Arm();
    }

    public void Arm()
    {
        if (completed) return;
        armed = true;
        armTimer = 0f;
        if (sr) sr.color = armedColor;
    }

    void Update()
    {
        if (!armed || completed) return;

        armTimer += Time.deltaTime;
        if (armTimer >= timeWindow)
        {
            // Timed out
            armed = false;
            if (sr) sr.color = failColor;
            onTimeout?.Invoke();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!armed || completed) return;

        if (other.CompareTag("Player") || other.CompareTag("Echo"))
        {
            completed = true;
            armed = false;
            if (sr) sr.color = successColor;
            onActivateSuccess?.Invoke();
        }
    }
}
