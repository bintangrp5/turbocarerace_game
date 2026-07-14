using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;
using System.Linq;
using System.Collections;

public enum TimerMode
{
    CountUp,
    CountDown
}


public enum GameMode
{
    Race,
    Survival
}

public class GameManager : MonoBehaviour
{
    public CarController car;
    public GameObject panelPause;
    [SerializeField] private CinemachineDistanceSetup cameraDistanceSetup;    

    [Header("Game Mode")]
    [SerializeField] private GameMode gameMode;
    public static GameManager Instance { get; private set; }

    private float timeElapsed = 0f;
    private float lastScoreTime = 0f;

    [Header("Game Flow")]
    // Hapus totalCheckpoints, kita hitung otomatis
    [SerializeField] private int scorePerCheckpoint = 100; 
    private int currentCheckpointIndex = -1;
    private int score = 0;
    private bool isGameActive = false;

    // Variabel baru untuk sistem Checkpoint dinamis
    private Checkpoint[] allCheckpoints;
    private int activeCheckpointsThisLap = 0;
    private int checkpointsPassedThisLap = 0;

    // Variabel baru untuk melacak checkpoint wajib
    private int activeMandatoryCheckpoints = 0;
    private int mandatoryCheckpointsPassedThisLap = 0;

    [Header("Lap Settings")]
    [SerializeField] private int totalLaps = 1;
    [SerializeField] private TMP_Text lapText;
    private int currentLap = 1;

    [Header("Timer Settings")]
    [SerializeField] private TimerMode timerMode = TimerMode.CountUp;
    [SerializeField] private float initialTimeLimit = 60f; 
    [SerializeField] private float timeBonusPerCheckpoint = 15f; 

    private float timeRemaining = 0f;
    private Color originalTimerTextColor = Color.white;

    [Header("UI Canvas Elements (Opsional)")]
    [SerializeField] private Slider healthSlider; 
    [SerializeField] private TMP_Text timerText; 
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text speedText; 
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text gameOverMessageText;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private TMP_Text victoryTimeText;

    [Header("Final Goal Settings")]
    [SerializeField] private Checkpoint finalGoalCheckpoint; 

    [Header("UI Text References")]
    [SerializeField] private TMP_Text hudScoreText;      
    [SerializeField] private TMP_Text gameOverScoreText;  
    [SerializeField] private TMP_Text victoryScoreText;  

    [SerializeField] public float BoostSpeedValue = 0f;   

    private CarStatus playerCarStatus;
    private Rigidbody playerRigidbody;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void ConfigureGameMode()
    {
        switch (gameMode)
        {
            case GameMode.Race:
                totalLaps = 3;
                timerMode = TimerMode.CountUp;
                break;

            case GameMode.Survival:
                totalLaps = 1;
                timerMode = TimerMode.CountDown;
                initialTimeLimit = 180f;
                break;
        }
    }

    private void Start()
    {
        ConfigureGameMode();

        Time.timeScale = 1f;

        if (panelPause != null)
            panelPause.SetActive(false);

        timeElapsed = 0f;
        timeRemaining = initialTimeLimit;
        isGameActive = false;
        currentLap = 1;

        if (timerText != null)
            originalTimerTextColor = timerText.color;

        playerCarStatus = FindObjectOfType<CarStatus>();

        if (playerCarStatus != null)
            playerRigidbody = playerCarStatus.GetComponent<Rigidbody>();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        SetupButtons();

        allCheckpoints = FindObjectsByType<Checkpoint>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);

        SetupLapCheckpoints();

        UpdateUIHUD();
    }
    
    private void SetupButtons()
    {
        GameObject pauseBtnObj = GameObject.Find("Pause") ?? GameObject.Find("Canvas/UI_HUD/Pause");
        if (pauseBtnObj != null)
        {
            Button pauseButton = pauseBtnObj.GetComponent<Button>();
            if (pauseButton != null)
            {
                pauseButton.onClick.RemoveAllListeners();
                pauseButton.onClick.AddListener(PauseGame);
            }
        }

        if (panelPause != null)
        {
            Button resumeButton = panelPause.transform.Find("btn_mainlagi")?.GetComponent<Button>();
            if (resumeButton != null) { resumeButton.onClick.RemoveAllListeners(); resumeButton.onClick.AddListener(ResumeGame); }

            Button restartButton = panelPause.transform.Find("btn_ulangi")?.GetComponent<Button>();
            if (restartButton != null) { restartButton.onClick.RemoveAllListeners(); restartButton.onClick.AddListener(RestartRace); }

            Button quitButton = panelPause.transform.Find("btn_berhentibalapan")?.GetComponent<Button>();
            if (quitButton != null) { quitButton.onClick.RemoveAllListeners(); quitButton.onClick.AddListener(QuitRace); }
        }
    }

    // Fungsi baru untuk mengatur Checkpoint mana yang aktif di lap tertentu
    private void SetupLapCheckpoints()
    {
        activeMandatoryCheckpoints = 0;
        mandatoryCheckpointsPassedThisLap = 0;
        currentCheckpointIndex = -1; 

        foreach (Checkpoint cp in allCheckpoints)
        {
            if (cp.TargetLap == 0 || cp.TargetLap == currentLap)
            {
                cp.gameObject.SetActive(true);
                cp.ResetCheckpoint();
                
                // Hitung berapa banyak checkpoint WAJIB di lap ini
                if (cp.IsMandatory)
                {
                    activeMandatoryCheckpoints++;
                }
            }
            else
            {
                cp.gameObject.SetActive(false); 
            }
        }

        Debug.Log($"[GameManager] Lap {currentLap} Mulai. Checkpoint Wajib: {activeMandatoryCheckpoints}");
    }

    public void StartRace()
    {
        isGameActive = true;
        if (car != null) car.StartAutoDrive();
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
        {
            if (panelPause != null && panelPause.activeSelf) ResumeGame();
            else PauseGame();
        }
        
        if (!isGameActive) return;
        
        timeElapsed += Time.deltaTime;

        if (timerMode == TimerMode.CountDown)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0f)
            {
                timeRemaining = 0f;
                UpdateUIHUD(); 
                TriggerGameOver("Waktu Habis!");
                return;
            }
        }

        UpdateUIHUD();
    }

    public void OnCheckpointPassed(Checkpoint cp)
    {
        if (!isGameActive) return;

        int index = cp.CheckpointIndex;

        // GARIS FINISH (LAP)
        if (cp.IsFinishLine)
        {
            if (mandatoryCheckpointsPassedThisLap >= activeMandatoryCheckpoints)
            {
                if (currentLap < totalLaps)
                {
                    currentLap++;

                    SetupLapCheckpoints();

                    Debug.Log($"[GameManager] Lap {currentLap} Dimulai!");
                }
                else
                {
                    Debug.Log("[GameManager] Semua Lap Selesai!");

                    StartCoroutine(FinishRace());
                }
            }
            else
            {
                Debug.Log("[GameManager] Checkpoint wajib belum lengkap!");

                cp.ResetCheckpoint();
            }

            return;
        }

        // // CHECKPOINT FINAL (VICTORY)
        // if (cp == finalGoalCheckpoint)
        // {
        //     Debug.Log("[GameManager] Final Goal Terlewati! Menang!");
        //     TriggerVictory();
        //     return;
        // }

        // CHECKPOINT BIASA & WAJIB
        if (index <= currentCheckpointIndex) return;
        currentCheckpointIndex = index; 

        // LOGIKA BARU:
        if (cp.IsMandatory)
        {
            // Jika Mandatory (Sector), HANYA tambah progres, TANPA skor dan TANPA boost
            mandatoryCheckpointsPassedThisLap++;
            Debug.Log($"[GameManager] Sector dilewati! Progres: {mandatoryCheckpointsPassedThisLap}");
        }
        else
        {
            // Jika BUKAN Mandatory, berikan skor dan bonus kecepatan
            AddScore(scorePerCheckpoint);
            
            if (timerMode == TimerMode.CountDown) 
                timeRemaining += timeBonusPerCheckpoint;
            
            // Boost speed hanya untuk checkpoint biasa
            CarController car = FindFirstObjectByType<CarController>();
            if (car != null) car.BoostSpeed(cp.boostSpeedValue); 
        }
    }

    public void AddScore(int amount)
    {
        if (!isGameActive) return;

        // Jika AddScore dipanggil lagi dalam waktu kurang dari 0.5 detik, abaikan
        if (Time.time - lastScoreTime < 0.5f) return; 

        lastScoreTime = Time.time; // Update waktu terakhir dipanggil

        Debug.Log($"AddScore dipanggil! Amount: {amount} | Current Score: {score}");

        score += amount;
        
        if (hudScoreText != null) hudScoreText.text = $"Skor: {score}";
        if (gameOverScoreText != null) gameOverScoreText.text = $"Skor Akhir: {score}";
        if (victoryScoreText != null) victoryScoreText.text = $"Skor Akhir: {score}";
    }

    public void TriggerGameOver(string message)
    {
        if (!isGameActive) return;

        isGameActive = false;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (gameOverMessageText != null)
            gameOverMessageText.text = message;

        if (gameOverScoreText != null)
            gameOverScoreText.text = $"Skor Akhir: {score}";

        Time.timeScale = 0f;
    }
    public void TriggerVictory()
    {
        if (!isGameActive) return;
        isGameActive = false;
        Time.timeScale = 0f; 
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "Level_1")
        {
            PlayerPrefs.SetInt("Level1Done", 1);
            PlayerPrefs.SetInt("Level2Unlocked", 1);
        }
        else if (sceneName == "Level_2")
        {
            PlayerPrefs.SetInt("Level2Done", 1);
            PlayerPrefs.SetInt("Level3Unlocked", 1);
        }
        
        PlayerPrefs.Save();

        if (victoryPanel != null) victoryPanel.SetActive(true);

        if (victoryTimeText != null)
        {
            int minutes = Mathf.FloorToInt(timeElapsed / 60F);
            int seconds = Mathf.FloorToInt(timeElapsed - minutes * 60);
            victoryTimeText.text = string.Format("Waktu: {0:0}:{1:00}", minutes, seconds);
        }

        if (victoryScoreText != null)
        {
            victoryScoreText.text = $"Skor Akhir: {score}";
        }
    }

    private IEnumerator FinishRace()
    {
        Debug.Log("[GameManager] Balapan Selesai!");

        CarController car = FindFirstObjectByType<CarController>();

        if (car != null)
            car.StopCar();

        yield return new WaitForSeconds(2f);

        TriggerVictory();
    }

    private void UpdateUIHUD()
    {
        if (playerCarStatus != null && healthSlider != null)
    {
        healthSlider.maxValue = playerCarStatus.MaxCollisions;
        healthSlider.value = playerCarStatus.RemainingHits;

        Image fill = healthSlider.fillRect.GetComponent<Image>();

        float t = (float)playerCarStatus.RemainingHits / playerCarStatus.MaxCollisions;
        fill.color = Color.Lerp(Color.red, Color.green, t);
    }

        if (timerText != null)
        {
            float displayTime = (timerMode == TimerMode.CountDown) ? timeRemaining : timeElapsed;
            int minutes = Mathf.FloorToInt(displayTime / 60F);
            int seconds = Mathf.FloorToInt(displayTime - minutes * 60);

            if (timerMode == TimerMode.CountDown && timeRemaining <= 10f)
            {
                timerText.color = Color.red;
                int milliseconds = Mathf.FloorToInt((displayTime - Mathf.Floor(displayTime)) * 100);
                timerText.text = string.Format("{0:0}:{1:00}.{2:02}", minutes, seconds, milliseconds);
            }
            else
            {
                timerText.color = originalTimerTextColor;
                timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
            }
        }

        if (hudScoreText != null) hudScoreText.text = $"Skor: {score}";
        if (gameOverScoreText != null) gameOverScoreText.text = $"Skor Akhir: {score}";
        if (victoryScoreText != null) victoryScoreText.text = $"Skor Akhir: {score}";

        if (speedText != null && playerRigidbody != null)
        {
            float speedKmh = playerRigidbody.linearVelocity.magnitude * 3.6f;
            speedText.text = $"{Mathf.RoundToInt(speedKmh)} km/h";
        }

        if (lapText != null)
        {
            if (totalLaps > 1)
            {
                lapText.gameObject.SetActive(true);
                lapText.text = $"Lap: {currentLap}/{totalLaps}";
            }
            else lapText.gameObject.SetActive(false);
        }
    }

    public void PauseGame()
    {   
        Time.timeScale = 0f;
        if(panelPause != null) panelPause.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        panelPause.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Debug.Log($"ResumeGame dipanggil. car={car}, cameraDistanceSetup={cameraDistanceSetup}");

        if (car != null) car.ApplySettingsFromPrefs();
        if (cameraDistanceSetup != null) cameraDistanceSetup.ApplySettingsFromPrefs();
    }
    public void RestartRace()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitRace()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1f; 
        
        string currentScene = SceneManager.GetActiveScene().name;
        
        if (currentScene == "Level_1") 
        {
            SceneManager.LoadScene("Level_2");
        }
        else if (currentScene == "Level_2") 
        {
            SceneManager.LoadScene("Level_3");
        }
        else 
        {
            Debug.Log("Ini adalah level terakhir!");
        }
    }
}