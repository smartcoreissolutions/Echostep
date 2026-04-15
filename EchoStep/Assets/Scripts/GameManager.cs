using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Handles level flow: restart, completion, rating display, scene transitions.
/// Persistent singleton across scenes.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Level Config")]
    public string[] levelScenes = { "Level1", "Level2", "Level3", "Level4", "Level5" };
    public int currentLevelIndex = 0;

    [Header("UI References (optional)")]
    public GameObject ratingPanel;
    public Text ratingText;
    public Text statsText;

    [HideInInspector]
    public LevelStats stats = new LevelStats();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        stats.Reset();
        if (ratingPanel != null) ratingPanel.SetActive(false);
    }

    public void RestartLevel()
    {
        if (EchoManager.Instance) EchoManager.Instance.ResetAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void CompleteLevel()
    {
        // Calculate rating
        EncoreRating.Grade grade = EncoreRating.Calculate(stats);

        Debug.Log($"Level Complete! Grade: {grade} | Echoes used: {stats.echoesUsed}/{stats.echoesSpawned} | Shockwaves: {stats.shockwavesTriggered}");

        // Show UI
        if (ratingPanel != null)
        {
            ratingPanel.SetActive(true);
            if (ratingText) ratingText.text = EncoreRating.GradeToString(grade);
            if (statsText) statsText.text =
                $"Echoes: {stats.echoesUsed}/{stats.echoesSpawned}\n" +
                $"Shockwaves: {stats.shockwavesTriggered}\n" +
                $"Landings: {stats.totalLandings}";
        }

        // Auto-advance after delay (or wait for button)
        Invoke(nameof(LoadNextLevel), 3f);
    }

    public void LoadNextLevel()
    {
        currentLevelIndex++;
        if (currentLevelIndex < levelScenes.Length)
        {
            SceneManager.LoadScene(levelScenes[currentLevelIndex]);
        }
        else
        {
            Debug.Log("All levels complete!");
            // Load credits or main menu
            // SceneManager.LoadScene("Credits");
        }
    }

    void Update()
    {
        // Quick restart with R
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartLevel();
        }
    }
}
