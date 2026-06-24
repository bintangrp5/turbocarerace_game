using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public enum TimerMode
{
    CountUp,
    CountDown
}

public class GameManager : MonoBehaviour
{
    public GameObject panelPause;
    public static GameManager Instance { get; private set; }

    private float timeElapsed = 0f;

    [Header("Game Flow")]
    [SerializeField] private int totalCheckpoints = 5;
    [SerializeField] private int scorePerCheckpoint = 100; // Skor yang didapat setiap melewati checkpoint
    private int currentCheckpointIndex = -1;
    private int score = 0;
    private bool isGameActive = false;

    [Header("Lap Settings")]
    [SerializeField] private int totalLaps = 1;
    [SerializeField] private TMP_Text lapText;
    private int currentLap = 1;

    [Header("Timer Settings")]
    [SerializeField] private TimerMode timerMode = TimerMode.CountUp;
    [SerializeField] private float initialTimeLimit = 60f; // Waktu awal dalam detik untuk CountDown
    [SerializeField] private float timeBonusPerCheckpoint = 15f; // Bonus waktu setiap melewati checkpoint

    private float timeRemaining = 0f;
    private Color originalTimerTextColor = Color.white;

    [Header("UI Canvas Elements (Opsional)")]
    [SerializeField] private Slider healthSlider; // Slider untuk jumlah sisa kesempatan menabrak
    [SerializeField] private TMP_Text timerText; // Teks waktu berjalan naik dari 0
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text speedText; // Teks untuk menampilkan kecepatan mobil (KM/H)
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text gameOverMessageText;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private TMP_Text victoryTimeText; // Menampilkan total waktu kemenangan

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

    private void Start()
    {
        Time.timeScale = 1f;

        if (panelPause != null)
        {
            panelPause.SetActive(false);
        }
        timeElapsed = 0f;
        timeRemaining = initialTimeLimit;
        isGameActive = false;
        currentLap = 1;

        if (timerText != null)
        {
            originalTimerTextColor = timerText.color;
        }

        playerCarStatus = FindObjectOfType<CarStatus>();
        if (playerCarStatus != null)
        {
            playerRigidbody = playerCarStatus.GetComponent<Rigidbody>();
        }

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);

        // Bind Pause Button
        GameObject pauseBtnObj = GameObject.Find("Pause");
        if (pauseBtnObj == null)
        {
            pauseBtnObj = GameObject.Find("Canvas/UI_HUD/Pause");
        }
        if (pauseBtnObj != null)
        {
            Button pauseButton = pauseBtnObj.GetComponent<Button>();
            if (pauseButton != null)
            {
                pauseButton.onClick.RemoveAllListeners();
                pauseButton.onClick.AddListener(PauseGame);
                Debug.Log("[GameManager] Bound Pause button click event programmatically.");
            }
        }

        // Bind panelPause buttons
        if (panelPause != null)
        {
            Button resumeButton = panelPause.transform.Find("btn_mainlagi")?.GetComponent<Button>();
            if (resumeButton != null)
            {
                resumeButton.onClick.RemoveAllListeners();
                resumeButton.onClick.AddListener(ResumeGame);
                Debug.Log("[GameManager] Bound Resume button (btn_mainlagi) programmatically.");
            }

            Button restartButton = panelPause.transform.Find("btn_ulangi")?.GetComponent<Button>();
            if (restartButton != null)
            {
                restartButton.onClick.RemoveAllListeners();
                restartButton.onClick.AddListener(RestartRace);
                Debug.Log("[GameManager] Bound Restart button (btn_ulangi) programmatically.");
            }

            Button quitButton = panelPause.transform.Find("btn_berhentibalapan")?.GetComponent<Button>();
            if (quitButton != null)
            {
                quitButton.onClick.RemoveAllListeners();
                quitButton.onClick.AddListener(QuitRace);
                Debug.Log("[GameManager] Bound Quit button (btn_berhentibalapan) programmatically.");
            }
        }

        UpdateUIHUD();
    }

    public void StartRace()
    {
         Debug.Log("START RACE DIPANGGIL");
         isGameActive = true;
    }
    

    private void Update()
    {
        // Menggunakan Input System Baru untuk mendeteksi tombol P di Keyboard
        if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
        {
            if (panelPause != null && panelPause.activeSelf)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
        
        if (!isGameActive) return;
        
        // Hitung waktu bertambah (stopwatch / count up)
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

    public void OnCheckpointPassed(int index)
    {
        if (!isGameActive) return;

        // Memastikan checkpoint dilewati sesuai urutan agar tidak bisa curang
        if (index > currentCheckpointIndex)
        {
            currentCheckpointIndex = index;
            Debug.Log($"[GameManager] Checkpoint {index} Terlewati! Waktu saat ini: {timeElapsed:F1} detik.");

            // Tambahkan skor setelah melewati checkpoint
            AddScore(scorePerCheckpoint);

            // Jika dalam mode CountDown, berikan bonus waktu
            if (timerMode == TimerMode.CountDown)
            {
                timeRemaining += timeBonusPerCheckpoint;
                Debug.Log($"[GameManager] Bonus waktu +{timeBonusPerCheckpoint}s! Sisa waktu sekarang: {timeRemaining:F1}s.");
            }

            // Jika player berhasil melewati semua checkpoint
            if (currentCheckpointIndex == totalCheckpoints - 1)
            {
                if (currentLap < totalLaps)
                {
                    currentLap++;
                    currentCheckpointIndex = -1; // Reset checkpoint index untuk lap berikutnya
                    
                    // Reset semua checkpoint di scene agar bisa dipicu kembali
                    Checkpoint[] checkpoints = FindObjectsOfType<Checkpoint>();
                    foreach (Checkpoint cp in checkpoints)
                    {
                        cp.ResetCheckpoint();
                    }
                    
                    Debug.Log($"[GameManager] Lap {currentLap - 1} Selesai! Mulai Lap {currentLap}/{totalLaps}.");
                }
                else
                {
                    TriggerVictory();
                }
            }
        }
    }

    public void AddScore(int amount)
    {
        if (!isGameActive) return;
        score += amount;
        Debug.Log($"[GameManager] Skor bertambah! Total Skor: {score}");
    }

    public void TriggerGameOver(string message)
    {
        if (!isGameActive) return;
        isGameActive = false;
        Debug.Log($"[GameManager] GAME OVER: {message}");

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        if (gameOverMessageText != null)
        {
            gameOverMessageText.text = message;
        }
    }


    public void TriggerVictory()
    {
        if (!isGameActive) return;
        isGameActive = false;

        string sceneName = SceneManager.GetActiveScene().name;

        // 🔓 UNLOCK SYSTEM BERDASARKAN LEVEL
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
        // nanti tinggal lanjut kalau ada level berikutnya

        PlayerPrefs.Save();

        Debug.Log($"[GameManager] VICTORY di {sceneName}! Total waktu: {timeElapsed:F1}s");

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }

        if (victoryTimeText != null)
        {
            int minutes = Mathf.FloorToInt(timeElapsed / 60F);
            int seconds = Mathf.FloorToInt(timeElapsed - minutes * 60);
            victoryTimeText.text = string.Format("Waktu: {0:0}:{1:00}", minutes, seconds);
        }
    }


    private void UpdateUIHUD()
    {
        // Update Slider HP (Kesempatan Menabrak tersisa)
        if (playerCarStatus != null && healthSlider != null)
        {
            healthSlider.maxValue = playerCarStatus.MaxCollisions;
            healthSlider.value = playerCarStatus.RemainingHits;
        }

        // Update Teks Timer
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

        // Update Teks Skor
        if (scoreText != null)
        {
            scoreText.text = $"Skor: {score}";
        }

        // Update Teks Kecepatan Mobil
        if (speedText != null && playerRigidbody != null)
        {
            float speedKmh = playerRigidbody.linearVelocity.magnitude * 3.6f;
            speedText.text = $"{Mathf.RoundToInt(speedKmh)} km/h";
        }

        // Update Teks Lap
        if (lapText != null)
        {
            if (totalLaps > 1)
            {
                lapText.gameObject.SetActive(true);
                lapText.text = $"Lap: {currentLap}/{totalLaps}";
            }
            else
            {
                lapText.gameObject.SetActive(false);
            }
        }
    }

   public void PauseGame()
    {   
        Debug.Log("PAUSE DIPANGGIL");
        Time.timeScale = 0f;

        if(panelPause != null)
        {
            panelPause.SetActive(true);
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        panelPause.SetActive(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void RestartRace()
    {
        Debug.Log("RESTART RACE");

        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitRace()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("Main Menu");
    }
}
