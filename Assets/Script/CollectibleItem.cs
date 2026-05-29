using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public enum CollectibleType { Score, Repair }

    [Header("Collectible Settings")]
    [SerializeField] private CollectibleType type = CollectibleType.Score;
    [SerializeField] private float value = 1f; // Jumlah penambahan skor atau pemulihan HP tabrakan

    [Header("Animation Settings")]
    [SerializeField] private float rotationSpeed = 100f; // Kecepatan berputar
    [SerializeField] private float floatSpeed = 2f;      // Kecepatan melayang naik-turun
    [SerializeField] private float floatHeight = 0.2f;    // Ketinggian naik-turun

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        // Efek visual 1: Berputar otomatis (Wow factor!)
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        // Efek visual 2: Melayang naik-turun secara halus menggunakan rumus Sinus
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Deteksi apakah mobil player yang menyentuh
        CarStatus carStatus = other.GetComponent<CarStatus>();
        if (carStatus == null)
        {
            carStatus = other.GetComponentInParent<CarStatus>();
        }

        if (carStatus != null && !carStatus.IsDead)
        {
            if (type == CollectibleType.Score)
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddScore((int)value);
                }
            }
            else if (type == CollectibleType.Repair)
            {
                carStatus.Repair((int)value); // Memulihkan kesempatan tabrakan (currentHits berkurang)
            }

            // Sembunyikan objek setelah diambil agar tidak bisa diambil lagi
            gameObject.SetActive(false);
            
            Debug.Log($"[Collectible] Berhasil mengambil {type} bernilai {value}!");
        }
    }
}
