using UnityEngine;

/// <summary>
/// Unified input helper — works with mouse, touch, keyboard, and gamepad.
/// One-button game: any press = action.
/// </summary>
public static class InputHelper
{
    private static float downTime;
    private static bool wasDown;
    private static bool isDown;
    private static bool justUp;
    private static bool isTap;

    public static bool IsDown => isDown;
    public static bool JustUp => justUp;
    public static bool IsTap => isTap;
    public static float HeldDuration => isDown ? Time.time - downTime : 0f;

    /// <summary>Call once per frame from a MonoBehaviour.</summary>
    public static void Tick()
    {
        wasDown = isDown;
        justUp = false;
        isTap = false;

        bool currentDown = Input.GetMouseButton(0)
            || Input.GetKey(KeyCode.Space)
            || Input.GetKey(KeyCode.JoystickButton0)
            || (Input.touchCount > 0);

        if (currentDown && !wasDown)
        {
            isDown = true;
            downTime = Time.time;
        }
        else if (!currentDown && wasDown)
        {
            isDown = false;
            justUp = true;
            float held = Time.time - downTime;
            if (held < GameConstants.TapThreshold)
                isTap = true;
        }
        else
        {
            isDown = currentDown;
        }
    }

    /// <summary>Any button pressed this frame (for menus).</summary>
    public static bool AnyPress()
    {
        return Input.GetMouseButtonDown(0)
            || Input.GetKeyDown(KeyCode.Space)
            || Input.GetKeyDown(KeyCode.Return)
            || Input.GetKeyDown(KeyCode.JoystickButton0)
            || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
    }

    /// <summary>Arrow key direction input (optional aiming).</summary>
    public static int ArrowDirection()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) return -1;
        if (Input.GetKeyDown(KeyCode.RightArrow)) return 1;

        // Gamepad
        float h = Input.GetAxis("Horizontal");
        if (h < -0.5f) return -1;
        if (h > 0.5f) return 1;

        return 0;
    }

    public static bool RestartPressed()
    {
        return Input.GetKeyDown(KeyCode.R);
    }

    public static void Clear()
    {
        isDown = false;
        justUp = false;
        isTap = false;
        wasDown = false;
    }
}
