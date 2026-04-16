using UnityEngine;

/// <summary>
/// Door that blocks the path until opened by a plate.
/// Slides up or fades when opened.
/// </summary>
public class DoorController : MonoBehaviour
{
    [Header("Animation")]
    public float slideDistance = 1f;
    public float slideSpeed = 3f;

    [Header("Visuals")]
    public SpriteRenderer doorRenderer;
    public ParticleSystem openParticles;

    private bool isOpen;
    private Vector3 closedPos;
    private Vector3 openPos;
    private BoxCollider2D col;

    void Awake()
    {
        closedPos = transform.position;
        openPos = closedPos + Vector3.up * slideDistance;
        col = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (isOpen)
        {
            transform.position = Vector3.MoveTowards(transform.position, openPos, slideSpeed * Time.deltaTime);

            // Fade out
            if (doorRenderer != null)
            {
                Color c = doorRenderer.color;
                c.a = Mathf.Max(0f, c.a - Time.deltaTime * 2f);
                doorRenderer.color = c;
                if (c.a <= 0f)
                {
                    if (col != null) col.enabled = false;
                    gameObject.SetActive(false);
                }
            }
        }
    }

    public void Open()
    {
        if (isOpen) return;
        isOpen = true;
        openParticles?.Play();
    }
}
