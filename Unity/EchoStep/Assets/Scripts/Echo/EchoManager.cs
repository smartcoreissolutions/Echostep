using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages echo spawning, replay, one-way platform behavior, and shockwave detection.
/// </summary>
public class EchoManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject echoPrefab;
    public GameObject shockwavePrefab;

    private List<EchoController> echoes = new List<EchoController>();
    private PlayerController player;

    void Start()
    {
        player = GameManager.Instance.player;
    }

    void Update()
    {
        // Spawn echo on landing
        if (player.justLanded
            && GameManager.Instance.EchoCharges > 0
            && player.lastJumpVelocity.sqrMagnitude > 0.1f)
        {
            SpawnEcho(player.transform.position, player.lastJumpVelocity);
            GameManager.Instance.EchoCharges--;
            GameManager.Instance.Stats.echoesSpawned++;
            SFXManager.Instance?.Play(SFXManager.Clip.Echo);
        }

        // Check for echo-echo collisions → shockwave
        for (int i = echoes.Count - 1; i >= 0; i--)
        {
            if (echoes[i] == null || echoes[i].isDead) { echoes.RemoveAt(i); continue; }

            for (int j = i - 1; j >= 0; j--)
            {
                if (echoes[j] == null || echoes[j].isDead) continue;

                var a = echoes[i];
                var b = echoes[j];
                float dist = Vector2.Distance(a.transform.position, b.transform.position);

                if (dist < GameConstants.EchoW && (a.isReplaying || b.isReplaying))
                {
                    Vector2 midpoint = (a.transform.position + b.transform.position) * 0.5f;
                    SpawnShockwave(midpoint);
                    GameManager.Instance.Stats.shockwaves++;
                    GameManager.Instance.Stats.echoesUsed += 2;
                    a.Kill();
                    b.Kill();
                }
            }
        }

        // Cleanup dead echoes
        echoes.RemoveAll(e => e == null || e.isDead);
    }

    public void SpawnEcho(Vector2 position, Vector2 storedVelocity)
    {
        if (echoPrefab == null) return;

        GameObject obj = Instantiate(echoPrefab, position, Quaternion.identity);
        var echo = obj.GetComponent<EchoController>();
        echo.Initialize(storedVelocity);
        echoes.Add(echo);
    }

    private void SpawnShockwave(Vector2 position)
    {
        if (shockwavePrefab == null) return;

        Instantiate(shockwavePrefab, position, Quaternion.identity);
        SFXManager.Instance?.Play(SFXManager.Clip.Shockwave);
        GameManager.Instance.cameraController?.Shake(10f);
    }

    public void ClearAll()
    {
        foreach (var e in echoes)
        {
            if (e != null) Destroy(e.gameObject);
        }
        echoes.Clear();
    }
}
