using UnityEngine;

/// <summary>
/// Level data component — attached to each level prefab root.
/// Defines spawn point, echo limits, and optional overrides.
/// </summary>
public class LevelData : MonoBehaviour
{
    [Header("Player")]
    public Vector2 playerSpawn = new Vector2(1f, 5.2f);

    [Header("Echoes")]
    public int maxEchoes = 5;
    public int echoCharges = 5;

    [Header("Level Info")]
    public string levelMessage = "Level";
}
