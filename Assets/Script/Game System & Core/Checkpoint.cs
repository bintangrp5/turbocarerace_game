using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private int checkpointIndex;
    public int CheckpointIndex => checkpointIndex;

    [Tooltip("0 = Muncul di semua lap. 1 = Lap 1 saja, 2 = Lap 2 saja, dst.")]
    [SerializeField] private int targetLap = 0; 
    public int TargetLap => targetLap;

    [SerializeField] private Color activeColor = Color.green;
    [SerializeField] private float boostSpeedValue = 15f; 

    [Header("Behavior Settings")]
    [SerializeField] private bool hideOnPassed = true; 
    
    // ✅ FITUR BARU: Checkpoint yang wajib dilewati agar lap sah
    [Tooltip("Centang jika ini checkpoint wajib (seperti di pertengahan sirkuit)")]
    [SerializeField] private bool isMandatory = false;
    public bool IsMandatory => isMandatory;

    [Tooltip("Centang HANYA pada checkpoint terakhir (Garis Finish) penentu ganti lap")]
    [SerializeField] private bool isFinishLine = false; 
    public bool IsFinishLine => isFinishLine;

    private bool isTriggered = false;
    private Renderer meshRenderer;
    private Color originalColor;

    private void Start()
    {
        meshRenderer = GetComponent<Renderer>();
        if (meshRenderer != null)
        {
            originalColor = meshRenderer.material.color;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;

        CarController car = other.GetComponentInParent<CarController>();
        
        if (other.CompareTag("Player") || car != null)
        {
            isTriggered = true;
            
            // 1. Sembunyikan DULU sebelum melapor
            if (hideOnPassed)
            {
                gameObject.SetActive(false); 
            }
            else
            {
                if (meshRenderer != null)
                {
                    meshRenderer.material.color = activeColor; 
                }
            }

            if (car != null)
            {
                car.BoostSpeed(boostSpeedValue);
            }

            // 2. BARU laporkan ke GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnCheckpointPassed(this); 
            }
        }
    }

    public void ResetCheckpoint()
    {
        isTriggered = false;
        if (meshRenderer != null)
        {
            meshRenderer.material.color = originalColor;
        }
        
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }
}