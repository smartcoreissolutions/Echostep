using UnityEngine;

/// <summary>
/// Smooth camera follow with screen shake.
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Follow")]
    public Transform target;
    public float smoothSpeed = 4f;
    public Vector3 offset = new Vector3(0f, 0.5f, -10f);

    [Header("Shake")]
    private float shakeIntensity;
    private float shakeDecay = 0.9f;

    void LateUpdate()
    {
        if (target == null) return;

        // Smooth follow
        Vector3 desired = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);

        // Screen shake
        if (shakeIntensity > 0.005f)
        {
            Vector3 shakeOffset = new Vector3(
                Random.Range(-1f, 1f) * shakeIntensity,
                Random.Range(-1f, 1f) * shakeIntensity,
                0f
            );
            transform.position += shakeOffset;
            shakeIntensity *= shakeDecay;
        }
        else
        {
            shakeIntensity = 0f;
        }
    }

    public void Shake(float intensity)
    {
        // Convert from HTML pixel shake to Unity units
        float unityIntensity = intensity * 0.02f;
        shakeIntensity = Mathf.Max(shakeIntensity, unityIntensity);
    }
}
