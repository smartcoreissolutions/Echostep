using UnityEngine;

/// <summary>
/// Calculates the Encore rating (D → S) at end of level.
/// </summary>
[System.Serializable]
public class LevelStats
{
    public int echoesSpawned;
    public int echoesUsed;            // echoes that were interacted with
    public int shockwavesTriggered;
    public int totalLandings;
    public int wastedLandings;        // landings that didn't spawn an echo or weren't useful

    public void Reset()
    {
        echoesSpawned = 0;
        echoesUsed = 0;
        shockwavesTriggered = 0;
        totalLandings = 0;
        wastedLandings = 0;
    }
}

public static class EncoreRating
{
    public enum Grade { D, C, B, A, S }

    public static Grade Calculate(LevelStats stats)
    {
        float utilization = stats.echoesSpawned > 0
            ? (float)stats.echoesUsed / stats.echoesSpawned
            : 0f;

        int shockwaves = stats.shockwavesTriggered;
        int wasted = stats.wastedLandings;

        // S: 80%+ echo use, 2+ shockwaves, 0 wasted
        if (utilization >= 0.8f && shockwaves >= 2 && wasted == 0)
            return Grade.S;

        // A: 60%+ echo use, 1+ shockwave
        if (utilization >= 0.6f && shockwaves >= 1)
            return Grade.A;

        // B: 40%+ echo use
        if (utilization >= 0.4f)
            return Grade.B;

        // C: completed (caller already knows they finished)
        if (stats.totalLandings > 0)
            return Grade.C;

        return Grade.D;
    }

    public static string GradeToString(Grade grade)
    {
        return grade.ToString();
    }
}
