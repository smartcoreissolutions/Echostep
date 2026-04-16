using UnityEngine;

/// <summary>
/// Timed pressure plate — arms on player landing, must be triggered within a time window.
/// </summary>
public class TimedPlate : MonoBehaviour
{
    [Header("Config")]
    public float timeWindow = 2.5f;
    public DoorController linkedDoor;

    [Header("Visuals")]
    public SpriteRenderer plateRenderer;
    public SpriteRenderer timerBar;

    private bool armed;
    private bool completed;
    private float timer;

    void Update()
    {
        if (completed) return;

        if (armed)
        {
            timer += Time.deltaTime;
            float frac = 1f - (timer / timeWindow);

            // Update timer bar
            if (timerBar != null)
            {
                timerBar.transform.localScale = new Vector3(frac, 1f, 1f);
                timerBar.color = Color.Lerp(Color.red, Color.green, frac);
            }

            // Update plate color
            if (plateRenderer != null)
            {
                plateRenderer.color = Color.Lerp(Color.red, Color.green, frac);
            }

            if (timer >= timeWindow)
            {
                armed = false;
                if (plateRenderer != null) plateRenderer.color = new Color(0.4f, 0.4f, 0.4f);
                if (timerBar != null) timerBar.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>Called when player lands anywhere in the level.</summary>
    public void ArmOnLanding()
    {
        if (completed) return;
        armed = true;
        timer = 0f;
        if (timerBar != null) timerBar.gameObject.SetActive(true);
    }

    public void TryComplete()
    {
        if (!armed || completed) return;
        completed = true;
        armed = false;
        if (linkedDoor != null) linkedDoor.Open();
        if (plateRenderer != null) plateRenderer.color = new Color(0f, 1f, 0.4f);
        if (timerBar != null) timerBar.gameObject.SetActive(false);
        SFXManager.Instance?.Play(SFXManager.Clip.Door);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Echo"))
            TryComplete();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Echo"))
            TryComplete();
    }
}
