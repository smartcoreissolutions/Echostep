using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages echo spawning, charge count, and cleanup.
/// Attach to a persistent GameObject in the scene (or use GameManager).
/// </summary>
public class EchoManager : MonoBehaviour
{
    public static EchoManager Instance { get; private set; }

    [Header("Config")]
    public GameObject echoPrefab;
    public int maxEchoes = 5;

    [Header("Runtime (read-only)")]
    public int currentCharges;
    public List<Echo> activeEchoes = new List<Echo>();

    PlayerController player;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        currentCharges = maxEchoes;
        player = FindFirstObjectByType<PlayerController>();
    }

    void Update()
    {
        if (player != null && player.justLanded)
        {
            TrySpawnEcho();
        }
    }

    void TrySpawnEcho()
    {
        if (currentCharges <= 0) return;
        if (player.lastJumpVelocity == Vector2.zero) return;

        // Spawn echo at player landing position
        GameObject go = Instantiate(echoPrefab, player.transform.position, Quaternion.identity);
        Echo echo = go.GetComponent<Echo>();
        echo.Init(player.lastJumpVelocity);

        activeEchoes.Add(echo);
        currentCharges--;

        // Track stats
        if (GameManager.Instance) GameManager.Instance.stats.echoesSpawned++;

        // Update UI
        if (EchoChargeUI.Instance) EchoChargeUI.Instance.UpdateDots(currentCharges, maxEchoes);
    }

    public void AddCharge(int amount = 1)
    {
        currentCharges = Mathf.Min(currentCharges + amount, maxEchoes);
        if (EchoChargeUI.Instance) EchoChargeUI.Instance.UpdateDots(currentCharges, maxEchoes);
    }

    public void RemoveEcho(Echo echo)
    {
        activeEchoes.Remove(echo);
    }

    public void ResetAll()
    {
        foreach (var e in activeEchoes)
        {
            if (e != null) Destroy(e.gameObject);
        }
        activeEchoes.Clear();
        currentCharges = maxEchoes;
        if (EchoChargeUI.Instance) EchoChargeUI.Instance.UpdateDots(currentCharges, maxEchoes);
    }
}
