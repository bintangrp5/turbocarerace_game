using UnityEngine;
using TMPro;

public class SpeedDisplay : MonoBehaviour
{
    public TextMeshProUGUI speedText;
    public Rigidbody rb;

    void Update()
    {
        // Menghitung kecepatan
        float speed = rb.linearVelocity.magnitude * 3.6f;

        // 🔥 PERBAIKAN: Gunakan GetString dan "speed_unit" agar SAMA dengan SettingKontrol
        string unit = PlayerPrefs.GetString("speed_unit", "KMH"); // "KMH" sebagai default

        // Cek teksnya dan sesuaikan rumus
        if (unit == "KMH")
        {
            speedText.text = speed.ToString("0") + " km/h";
        }
        else if (unit == "MPH")
        {
            float mph = speed * 0.621371f;
            speedText.text = mph.ToString("0") + " mph";
        }
    }
}