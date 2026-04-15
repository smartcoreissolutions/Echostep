using UnityEngine;

/// <summary>
/// Pickup: adds +1 echo charge when the player touches it.
/// </summary>
public class ResonanceCrystal : MonoBehaviour
{
    public int chargeAmount = 1;
    public GameObject pickupEffect; // optional particle burst

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (EchoManager.Instance != null)
            {
                EchoManager.Instance.AddCharge(chargeAmount);
            }

            if (pickupEffect != null)
            {
                Instantiate(pickupEffect, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}
