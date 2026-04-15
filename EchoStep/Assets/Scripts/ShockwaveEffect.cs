using UnityEngine;

/// <summary>
/// Shockwave: launches the player upward when they enter the trigger.
/// Auto-destroys after a short duration.
/// </summary>
public class ShockwaveEffect : MonoBehaviour
{
    public float launchForce = 16f;
    public float duration = 0.5f;

    void Start()
    {
        Destroy(gameObject, duration);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.ApplyExternalForce(Vector2.up * launchForce);
            }
        }
    }
}
