using UnityEngine;

/// <summary>
/// End-of-level trigger. Calculates Encore rating and loads next level.
/// </summary>
public class LevelGoal : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
                GameManager.Instance.CompleteLevel();
        }
    }
}
