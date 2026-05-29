using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private int checkpointIndex;
    [SerializeField] private Color activeColor = Color.green;
    
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

        // Mendeteksi apakah mobil player yang menabrak
        if (other.CompareTag("Player") || other.GetComponentInParent<CarController>() != null)
        {
            isTriggered = true;
            
            // Ubah warna visual checkpoint agar pemain tahu sudah terlewati
            if (meshRenderer != null)
            {
                meshRenderer.material.color = activeColor;
            }

            // Laporkan ke GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnCheckpointPassed(checkpointIndex);
            }
            else
            {
                Debug.LogWarning($"[Checkpoint] GameManager belum ada di scene! Indeks: {checkpointIndex}");
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
    }
}
