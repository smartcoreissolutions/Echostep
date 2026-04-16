using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages all UI panels — title, HUD, rating, game over.
/// Requires a Canvas with child panels.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject titlePanel;
    public GameObject hudPanel;
    public GameObject ratingPanel;
    public GameObject gameOverPanel;

    [Header("HUD Elements")]
    public Image[] echoDots;
    public Image chargeBar;
    public Image chargeBarBg;
    public TextMeshProUGUI levelLabel;
    public TextMeshProUGUI levelMessage;
    public TextMeshProUGUI controlsHint;

    [Header("Rating Elements")]
    public TextMeshProUGUI gradeText;
    public TextMeshProUGUI statsText;
    public TextMeshProUGUI continueText;

    [Header("Title Elements")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI subtitleText;
    public TextMeshProUGUI startPrompt;

    [Header("Colors")]
    public Color echoDotActive = new Color(0f, 0.87f, 1f);
    public Color echoDotInactive = new Color(0.13f, 0.13f, 0.2f);

    private float levelMsgTimer;
    private float titlePulsePhase;
    private float ratingTimer;

    void Update()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        switch (gm.CurrentState)
        {
            case GameManager.State.Playing:
                UpdateHUD(gm);
                break;
            case GameManager.State.Title:
                UpdateTitle();
                break;
            case GameManager.State.Rating:
                ratingTimer += Time.deltaTime;
                UpdateRating(gm);
                break;
        }
    }

    public void OnStateChanged(GameManager.State state)
    {
        titlePanel?.SetActive(state == GameManager.State.Title);
        hudPanel?.SetActive(state == GameManager.State.Playing);
        ratingPanel?.SetActive(state == GameManager.State.Rating);
        gameOverPanel?.SetActive(state == GameManager.State.GameOver);

        if (state == GameManager.State.Rating) ratingTimer = 0f;
        if (state == GameManager.State.Playing)
        {
            levelMsgTimer = 3f;
            var ld = FindObjectOfType<LevelData>();
            if (ld != null && levelMessage != null)
                levelMessage.text = ld.levelMessage;
        }
    }

    private void UpdateHUD(GameManager gm)
    {
        // Echo dots
        if (echoDots != null)
        {
            for (int i = 0; i < echoDots.Length; i++)
            {
                echoDots[i].gameObject.SetActive(i < gm.MaxEchoes);
                echoDots[i].color = i < gm.EchoCharges ? echoDotActive : echoDotInactive;
            }
        }

        // Charge bar
        if (chargeBar != null && chargeBarBg != null)
        {
            bool show = gm.player.isCharging;
            chargeBarBg.gameObject.SetActive(show);
            chargeBar.gameObject.SetActive(show);
            if (show)
            {
                float t = gm.player.chargeTime / GameConstants.MaxChargeTime;
                chargeBar.fillAmount = t;
                chargeBar.color = Color.Lerp(new Color(0f, 0.67f, 1f), new Color(1f, 0.27f, 0f), t);
            }
        }

        // Level label
        if (levelLabel != null)
            levelLabel.text = $"LEVEL {gm.CurrentLevel + 1}/5";

        // Level message fade
        if (levelMsgTimer > 0f)
        {
            levelMsgTimer -= Time.deltaTime;
            if (levelMessage != null)
            {
                Color c = levelMessage.color;
                c.a = Mathf.Min(levelMsgTimer, 1f);
                levelMessage.color = c;
            }
        }
        else if (levelMessage != null)
        {
            levelMessage.gameObject.SetActive(false);
        }

        // Controls hint (level 1 only)
        if (controlsHint != null)
            controlsHint.gameObject.SetActive(gm.CurrentLevel == 0 && levelMsgTimer > 0f);
    }

    private void UpdateTitle()
    {
        titlePulsePhase += Time.deltaTime;

        // Pulsing start prompt
        if (startPrompt != null)
        {
            float blink = Mathf.Sin(titlePulsePhase * 3f) > 0f ? 1f : 0f;
            Color c = startPrompt.color;
            c.a = blink * 0.9f;
            startPrompt.color = c;
        }

        // Glowing title
        if (titleText != null)
        {
            float glow = Mathf.Sin(titlePulsePhase * 1.5f) * 0.3f + 0.7f;
            Color c = titleText.color;
            c.a = glow;
            titleText.color = c;
        }
    }

    private void UpdateRating(GameManager gm)
    {
        // Grade appears at 0.3s
        if (gradeText != null)
        {
            gradeText.gameObject.SetActive(ratingTimer > 0.3f);
            if (ratingTimer > 0.3f)
            {
                string grade = gm.CalculateGrade();
                gradeText.text = grade;
                float scale = Mathf.Min((ratingTimer - 0.3f) * 4f, 1f);
                gradeText.transform.localScale = Vector3.one * scale;

                // Grade color
                gradeText.color = grade switch
                {
                    "S" => new Color(1f, 0.84f, 0f),
                    "A" => new Color(0f, 1f, 0.53f),
                    "B" => new Color(0f, 0.8f, 1f),
                    "C" => new Color(0.67f, 0.67f, 0.67f),
                    _   => new Color(1f, 0.27f, 0.27f)
                };
            }
        }

        // Stats at 0.8s
        if (statsText != null)
        {
            statsText.gameObject.SetActive(ratingTimer > 0.8f);
            if (ratingTimer > 0.8f)
            {
                statsText.text = $"Echoes Used: {gm.Stats.echoesUsed}/{gm.Stats.echoesSpawned}\n" +
                                 $"Shockwaves: {gm.Stats.shockwaves}\n" +
                                 $"Landings: {gm.Stats.landings}";
            }
        }

        // Continue prompt at 2s
        if (continueText != null)
        {
            bool show = ratingTimer > 2f && Mathf.Sin(Time.time * 3f) > 0f;
            continueText.gameObject.SetActive(show);
        }
    }
}
