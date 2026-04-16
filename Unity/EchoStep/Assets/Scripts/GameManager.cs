using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages game state, level loading, and transitions.
/// Singleton — lives on a root GameObject "GameManager".
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum State { Title, Playing, Rating, GameOver }
    public State CurrentState { get; private set; } = State.Title;
    public int CurrentLevel { get; private set; } = 0;
    public int MaxEchoes { get; set; } = 5;
    public int EchoCharges { get; set; } = 5;
    public LevelStats Stats;

    public UnityEvent OnLevelLoaded;
    public UnityEvent OnStateChanged;

    [Header("Level Prefabs")]
    public GameObject[] levelPrefabs;  // assign in inspector

    [Header("References")]
    public PlayerController player;
    public EchoManager echoManager;
    public UIManager uiManager;
    public CameraController cameraController;

    private GameObject currentLevelInstance;
    private float inputBlackoutEnd;
    private float ratingTimer;

    [System.Serializable]
    public struct LevelStats
    {
        public int echoesSpawned;
        public int echoesUsed;
        public int shockwaves;
        public int landings;
        public int wastedLandings;

        public void Reset()
        {
            echoesSpawned = 0;
            echoesUsed = 0;
            shockwaves = 0;
            landings = 0;
            wastedLandings = 0;
        }
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        SetState(State.Title);
    }

    void Update()
    {
        switch (CurrentState)
        {
            case State.Title:
                if (InputHelper.AnyPress())
                {
                    LoadLevel(0);
                }
                break;

            case State.Rating:
                ratingTimer += Time.deltaTime;
                if (ratingTimer > 2f && InputHelper.AnyPress())
                {
                    LoadLevel(CurrentLevel + 1);
                }
                break;

            case State.GameOver:
                if (InputHelper.AnyPress())
                {
                    SetState(State.Title);
                }
                break;
        }
    }

    public void LoadLevel(int index)
    {
        CurrentLevel = index;

        // Cleanup
        if (currentLevelInstance != null) Destroy(currentLevelInstance);
        echoManager?.ClearAll();

        // Reset stats
        Stats.Reset();
        EchoCharges = 5;
        MaxEchoes = 5;

        if (index < levelPrefabs.Length)
        {
            currentLevelInstance = Instantiate(levelPrefabs[index]);
            var levelData = currentLevelInstance.GetComponent<LevelData>();
            if (levelData != null)
            {
                MaxEchoes = levelData.maxEchoes;
                EchoCharges = levelData.echoCharges;
                player.Spawn(levelData.playerSpawn);
            }

            inputBlackoutEnd = Time.time + GameConstants.InputBlackout;
            SetState(State.Playing);
            OnLevelLoaded?.Invoke();
        }
        else
        {
            SetState(State.GameOver);
        }
    }

    public void RestartLevel()
    {
        LoadLevel(CurrentLevel);
    }

    public void CompleteLevel()
    {
        ratingTimer = 0f;
        SetState(State.Rating);
        SFXManager.Instance?.Play(SFXManager.Clip.Goal);
        cameraController?.Shake(6f);
    }

    public bool IsInputBlocked()
    {
        return Time.time < inputBlackoutEnd;
    }

    public string CalculateGrade()
    {
        float utilization = Stats.echoesSpawned > 0
            ? (float)Stats.echoesUsed / Stats.echoesSpawned
            : 0f;
        if (utilization >= 0.8f && Stats.shockwaves >= 2 && Stats.wastedLandings == 0) return "S";
        if (utilization >= 0.6f && Stats.shockwaves >= 1) return "A";
        if (utilization >= 0.4f) return "B";
        if (Stats.landings > 0) return "C";
        return "D";
    }

    private void SetState(State newState)
    {
        CurrentState = newState;
        uiManager?.OnStateChanged(newState);
        OnStateChanged?.Invoke();
    }
}
