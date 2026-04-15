using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HUD: shows echo charges as dots (filled = available, empty = used).
/// </summary>
public class EchoChargeUI : MonoBehaviour
{
    public static EchoChargeUI Instance { get; private set; }

    public Image[] dots;                   // assign 5 Image components
    public Color filledColor = Color.cyan;
    public Color emptyColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);

    void Awake()
    {
        Instance = this;
    }

    public void UpdateDots(int charges, int max)
    {
        for (int i = 0; i < dots.Length; i++)
        {
            if (dots[i] == null) continue;
            dots[i].color = i < charges ? filledColor : emptyColor;
            dots[i].gameObject.SetActive(i < max);
        }
    }
}
