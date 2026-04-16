using UnityEngine;

/// <summary>
/// Collectible crystal — gives one echo charge.
/// Bobs, rotates, and sparkles.
/// </summary>
public class Crystal : MonoBehaviour
{
    public ParticleSystem sparkles;
    public float bobSpeed = 2f;
    public float bobHeight = 0.1f;
    public float rotateSpeed = 60f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
        gameObject.tag = "Crystal";
    }

    void Update()
    {
        // Bob up and down
        Vector3 pos = startPos;
        pos.y += Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = pos;

        // Rotate
        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.EchoCharges = Mathf.Min(
                GameManager.Instance.EchoCharges + 1,
                GameManager.Instance.MaxEchoes
            );
            SFXManager.Instance?.Play(SFXManager.Clip.Crystal);

            // Burst particles
            if (sparkles != null)
            {
                sparkles.transform.parent = null;
                sparkles.Emit(15);
                Destroy(sparkles.gameObject, 2f);
            }

            Destroy(gameObject);
        }
    }
}
