using UnityEngine;

public class CarStatus : MonoBehaviour
{
    [Header("Health / Collision Settings")]
    [SerializeField] private int maxCollisions = 15; // Maksimal menabrak 15 kali
    private int currentHits = 0; // Jumlah tabrakan saat ini

    [SerializeField] private float minDamageSpeed = 4f; // Kecepatan minimum tabrakan untuk dihitung

    private bool isDead = false;

    public int MaxCollisions => maxCollisions;
    public int CurrentHits => currentHits;
    public int RemainingHits => Mathf.Max(0, maxCollisions - currentHits);
    public bool IsDead => isDead;

    private void Start()
    {
        currentHits = 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isDead) return;

        // Hitung kekuatan tabrakan berdasarkan kecepatan relatif
        float impactSpeed = collision.relativeVelocity.magnitude;
        if (impactSpeed >= minDamageSpeed)
        {
            currentHits++;
            Debug.Log($"[CarStatus] Menabrak! Total Tabrakan: {currentHits}/{maxCollisions}. Kecepatan tabrakan: {impactSpeed:F1} m/s.");

            if (currentHits >= maxCollisions)
            {
                Die();
            }
        }
    }

    public void Repair(int hitRecovery)
    {
        if (isDead) return;
        currentHits = Mathf.Max(0, currentHits - hitRecovery);
        Debug.Log($"[CarStatus] Memperbaiki mobil! Total Tabrakan sekarang: {currentHits}/{maxCollisions}");
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;
        currentHits = maxCollisions;
        Debug.Log("[CarStatus] Mobil mogok! (Sudah menabrak 15 kali)");

        CarController controller = GetComponent<CarController>();
        if (controller != null)
        {
            controller.StopCar(); // Memanggil fungsi stop yang kita buat tadi
        }
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TriggerGameOver("GAMEO OVER!");
        }
    }
}
