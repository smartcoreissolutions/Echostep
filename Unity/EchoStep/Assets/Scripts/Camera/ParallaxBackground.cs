using UnityEngine;

/// <summary>
/// Parallax star field background.
/// Attach to a quad/plane behind the game world.
/// </summary>
public class ParallaxBackground : MonoBehaviour
{
    [Header("Stars")]
    public int starCount = 200;
    public Material starMaterial;

    private struct Star
    {
        public Vector2 basePos;
        public float size;
        public float parallax;
        public float brightness;
        public float twinkleOffset;
    }

    private Star[] stars;
    private ParticleSystem ps;
    private ParticleSystem.Particle[] particleBuffer;

    void Start()
    {
        // Create particle system for stars
        ps = gameObject.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.maxParticles = starCount;
        main.startLifetime = Mathf.Infinity;
        main.startSpeed = 0f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.playOnAwake = false;
        main.loop = false;

        var emission = ps.emission;
        emission.enabled = false;

        var shape = ps.shape;
        shape.enabled = false;

        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        if (starMaterial != null) renderer.material = starMaterial;
        renderer.sortingOrder = -100;

        // Initialize stars
        stars = new Star[starCount];
        particleBuffer = new ParticleSystem.Particle[starCount];

        for (int i = 0; i < starCount; i++)
        {
            stars[i] = new Star
            {
                basePos = new Vector2(Random.Range(-30f, 30f), Random.Range(-15f, 15f)),
                size = Random.Range(0.02f, 0.08f),
                parallax = Random.Range(0.05f, 0.3f),
                brightness = Random.Range(0.2f, 0.7f),
                twinkleOffset = Random.Range(0f, Mathf.PI * 2f)
            };
        }

        // Emit all particles
        ps.Emit(starCount);
    }

    void LateUpdate()
    {
        if (ps == null) return;

        int count = ps.GetParticles(particleBuffer);
        Camera cam = Camera.main;
        if (cam == null) return;

        Vector2 camPos = cam.transform.position;
        float time = Time.time;

        for (int i = 0; i < Mathf.Min(count, starCount); i++)
        {
            var star = stars[i];
            float twinkle = Mathf.Sin(time * 2f + star.twinkleOffset) * 0.3f + 0.7f;
            float alpha = star.brightness * twinkle;

            // Parallax offset
            Vector2 pos = star.basePos - camPos * star.parallax;
            // Wrap around viewport
            float hw = 20f, hh = 12f;
            pos.x = ((pos.x % hw) + hw * 1.5f) % hw - hw * 0.5f;
            pos.y = ((pos.y % hh) + hh * 1.5f) % hh - hh * 0.5f;

            particleBuffer[i].position = new Vector3(camPos.x + pos.x, camPos.y + pos.y, 10f);
            particleBuffer[i].startSize = star.size;
            particleBuffer[i].startColor = new Color(0.7f, 0.8f, 1f, alpha);
        }

        ps.SetParticles(particleBuffer, count);
    }
}
