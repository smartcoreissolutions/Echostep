using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Activates when the player OR an echo lands on it.
/// Fires a UnityEvent (wire up doors, platforms, etc. in the Inspector).
/// </summary>
public class PressurePlate : MonoBehaviour
{
    public UnityEvent onActivate;
    public UnityEvent onDeactivate;
    public bool stayActive = true;          // if false, deactivates when object leaves
    public Color activeColor = Color.green;
    public Color inactiveColor = Color.gray;

    bool activated;
    SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr) sr.color = inactiveColor;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (activated && stayActive) return;

        if (other.CompareTag("Player") || other.CompareTag("Echo"))
        {
            Activate();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!stayActive && activated)
        {
            if (other.CompareTag("Player") || other.CompareTag("Echo"))
            {
                Deactivate();
            }
        }
    }

    void Activate()
    {
        activated = true;
        if (sr) sr.color = activeColor;
        onActivate?.Invoke();
    }

    void Deactivate()
    {
        activated = false;
        if (sr) sr.color = inactiveColor;
        onDeactivate?.Invoke();
    }
}
