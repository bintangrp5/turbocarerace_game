using UnityEngine;
using TMPro;
using UnityEngine.UI; // 🔥 Wajib ditambahkan agar bisa mengubah warna Image Button

public class SettingKontrol : MonoBehaviour
{
    public TMP_Text previewSpeedText; // TMP di menu setting

    [Header("UI Tombol Kamera")]
    public Image tombolDekat;
    public Image tombolSedang;
    public Image tombolJauh;

    [Header("UI Tombol Unit Kecepatan")]
    public Image tombolKMH;
    public Image tombolMPH;

    // Warna untuk tombol (Biru = Terpilih, Putih = Normal)
    private Color32 warnaPilih = new Color32(106, 141, 173, 255);
    private Color32 warnaNormal = new Color32(255, 255, 255, 255);

    void Start()
    {
        // ... (Jika kamu punya kode Start lain, biarkan di sini) ...

        // 🔥 Panggil fungsi ini agar saat menu dibuka, warnanya langsung menyesuaikan
        UpdateWarnaTombolKamera();
        UpdateWarnaTombolSpeedUnit();
    }

    // =====================
    // 🚗 KONTROL
    // =====================
    public void SetManual()
    {
        PlayerPrefs.SetInt("kontrol", 0);
        PlayerPrefs.Save();
        Debug.Log("Kontrol Manual dipilih");
    }

    public void SetOtomatis()
    {
        PlayerPrefs.SetInt("kontrol", 1);
        PlayerPrefs.Save();
        Debug.Log("Kontrol Otomatis dipilih");
    }

    // =====================
    // ⚡ SPEED UNIT
    // =====================
    public void SetKMH()
    {
        PlayerPrefs.SetString("speed_unit", "KMH");
        
        // Baris di bawah ini dihapus atau diberi garis miring ganda agar tidak aktif
        // previewSpeedText.text = "KM/JAM"; 
        
        UpdateWarnaTombolSpeedUnit();
    }

    public void SetMPH()
    {
        PlayerPrefs.SetString("speed_unit", "MPH");
        
        // Baris di bawah ini dihapus atau diberi garis miring ganda agar tidak aktif
        // previewSpeedText.text = "MPH"; 
        
        UpdateWarnaTombolSpeedUnit();
    }

    // =====================
    // 📷 KAMERA
    // =====================
    public void SetCameraNear()
    {
        PlayerPrefs.SetInt("camera_mode", 0);
        PlayerPrefs.Save();
        UpdateWarnaTombolKamera(); // Update warna setelah dipencet
    }

    public void SetCameraMedium()
    {
        PlayerPrefs.SetInt("camera_mode", 1);
        PlayerPrefs.Save();
        UpdateWarnaTombolKamera(); // Update warna setelah dipencet
    }

    public void SetCameraFar()
    {
        PlayerPrefs.SetInt("camera_mode", 2);
        PlayerPrefs.Save();
        UpdateWarnaTombolKamera(); // Update warna setelah dipencet
    }

    // 🔥 FUNGSI BARU: Mengubah warna tombol berdasarkan data yang tersimpan
    void UpdateWarnaTombolKamera()
    {
        // Pastikan kolom Image di Inspector tidak kosong agar tidak error
        if (tombolDekat == null || tombolSedang == null || tombolJauh == null) return;

        int mode = PlayerPrefs.GetInt("camera_mode", 1); // 1 = default sedang

        // 1. Kembalikan semua tombol ke warna putih dulu
        tombolDekat.color = warnaNormal;
        tombolSedang.color = warnaNormal;
        tombolJauh.color = warnaNormal;

        // 2. Warnai biru pada tombol yang sesuai dengan angka di PlayerPrefs
        if (mode == 0)
        {
            tombolDekat.color = warnaPilih;
        }
        else if (mode == 1)
        {
            tombolSedang.color = warnaPilih;
        }
        else if (mode == 2)
        {
            tombolJauh.color = warnaPilih;
        }
    }

    void UpdateWarnaTombolSpeedUnit()
    {
        // Pastikan kolom Image di Inspector tidak kosong agar tidak error
        if (tombolKMH == null || tombolMPH == null) return;

        string unit = PlayerPrefs.GetString("speed_unit", "KMH"); // Default KMH

        // 1. Kembalikan semua tombol ke warna putih dulu
        tombolKMH.color = warnaNormal;
        tombolMPH.color = warnaNormal;

        // 2. Warnai biru pada tombol yang sesuai dengan string di PlayerPrefs
        if (unit == "KMH")
        {
            tombolKMH.color = warnaPilih;
        }
        else if (unit == "MPH")
        {
            tombolMPH.color = warnaPilih;
        }
    }
}