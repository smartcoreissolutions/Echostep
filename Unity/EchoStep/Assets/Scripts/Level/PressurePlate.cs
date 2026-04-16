using UnityEngine;

/// <summary>
/// Pressure plate — activates when player or echo lands on it, opens a linked door.
/// </summary>
public class PressurePlate : MonoBehaviour
{
    [Header("Link")]
    public DoorController linkedDoor;

    [Header("Visuals")]
    public SpriteRenderer plateRenderer;
    public Color inactiveColor = new Color(0.4f, 0.4f, 0.4f);
    public Color activeColor = new Color(0f, 1f, 0.4f);

    private bool activated;

    void Start()
    {
        if (plateRenderer != null) plateRenderer.color = inactiveColor;
    }

    public void Activate()
    {
        if (activated) return;
        activated = true;

        if (plateRenderer != null) plateRenderer.color = activeColor;
        if (linkedDoor != null) linkedDoor.Open();

        SFXManager.Instance?.Play(SFXManager.Clip.Door);

        // Glow effect
        var ps = GetComponentInChildren<ParticleSystem>();
        if (ps != null) ps.Play();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Echo"))
            Activate();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Echo"))
            Activate();
    }
}
