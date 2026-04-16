using UnityEngine;

/// <summary>
/// Central game constants matching the HTML5 version.
/// Attach to any always-alive GameObject or access statically.
/// </summary>
public static class GameConstants
{
    // Physics
    public const float Gravity       = 18f;    // Unity units (HTML was 1800px → /100)
    public const float MinJump       = 4.2f;
    public const float MaxJump       = 10.5f;
    public const float MaxChargeTime = 1.0f;
    public const float JumpAngleDeg  = 62f;
    public const float GroundSkin    = 0.05f;

    // Echo
    public const float EchoReplayDelay = 2.0f;
    public const float EchoLifetime    = 10.0f;
    public const float EchoSinkSpeed   = 0.18f;
    public const float EchoBoost       = 4.5f;
    public const float EchoGracePeriod = 0.3f;

    // Shockwave
    public const float ShockwaveLaunch   = 11f;
    public const float ShockwaveRadius   = 0.8f;
    public const float ShockwaveDuration = 0.4f;

    // Player size (world units)
    public const float PlayerW = 0.28f;
    public const float PlayerH = 0.36f;
    public const float EchoW   = 0.28f;
    public const float EchoH   = 0.36f;

    // Input
    public const float TapThreshold = 0.15f;
    public const float InputBlackout = 0.3f;

    // Ground friction
    public const float GroundFrictionDecay = 0.0001f;
    public const float AirDrag = 0.5f;
}
