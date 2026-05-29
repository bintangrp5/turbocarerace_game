using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public enum TimerMode
{
    CountUp,
    CountDown
}

public class GameManager : MonoBehaviour
{
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
        timeElapsed = 0f;
        timeRemaining = initialTimeLimit;
        isGameActive = true;
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

        UpdateUIHUD();
    }

    private void Update()
    {
        if (!isGameActive) return;

        // Hitung waktu bertambah (stopwatch / count up)
        timeElapsed += Time.deltaTime;

        if (timerMode == TimerMode.CountDown)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0f)
            {
                timeRemaining = 0f;
                UpdateUIHUD(); // Update HUD to show 0:00
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
        Debug.Log($"[GameManager] VICTORY! Semua checkpoint terlewati! Total waktu: {timeElapsed:F1}s");

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

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
}
