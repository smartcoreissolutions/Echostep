using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Visual charge bar that fills while holding the jump button.
/// </summary>
public class ChargeBarUI : MonoBehaviour
{
    public Image fillImage;       // Image with Fill type set to Horizontal or Vertical
    public Gradient colorGradient; // optional: changes color as charge increases

    public void SetFill(float t)
    {
        t = Mathf.Clamp01(t);
        if (fillImage != null)
        {
            fillImage.fillAmount = t;
            if (colorGradient != null)
                fillImage.color = colorGradient.Evaluate(t);
        }
    }
}
