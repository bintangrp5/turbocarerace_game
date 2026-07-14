using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private int checkpointIndex;
    public int CheckpointIndex => checkpointIndex;

    [Tooltip("0 = Muncul di semua lap. 1 = Lap 1 saja, 2 = Lap 2 saja, dst.")]
    [SerializeField] private int targetLap = 0; 
    public int TargetLap => targetLap;

    [SerializeField] private Color activeColor = Color.green;
    [SerializeField] public float boostSpeedValue = 15f;

    [Header("Behavior Settings")]
    [SerializeField] private bool hideOnPassed = true; 
    
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

        if (!other.CompareTag("Player")) return; 

        CarController car = other.GetComponentInParent<CarController>();
        if (car != null)
        {
            isTriggered = true;
            
            if (hideOnPassed)
            {
                gameObject.SetActive(false); 
            }
            
            car.BoostSpeed(boostSpeedValue);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnCheckpointPassed(this); // Mengirim 'this' adalah benar
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