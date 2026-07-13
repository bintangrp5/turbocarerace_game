using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SettingKontrol : MonoBehaviour
{
    public TMP_Text previewSpeedText;

    [Header("UI Tombol Kamera")]
    public Image tombolDekat;
    public Image tombolSedang;
    public Image tombolJauh;

    [Header("UI Tombol Unit Kecepatan")]
    public Image tombolKMH;
    public Image tombolMPH;

    [Header("UI Tombol Kontrol")]
    public Image tombolManual;
    public Image tombolOtomatis;

    private Color32 warnaPilih = new Color32(106, 141, 173, 255);
    private Color32 warnaNormal = new Color32(255, 255, 255, 255);

    void Start()
    {
        // Panggil fungsi update saat menu dibuka agar tombol sesuai dengan data tersimpan
        UpdateWarnaTombolKamera();
        UpdateWarnaTombolSpeedUnit();
        UpdateWarnaTombolKontrol();
    }

    // =====================
    // 🚗 KONTROL
    // =====================
    public void SetManual()
    {
        PlayerPrefs.SetInt("kontrol", 0);
        PlayerPrefs.Save();
        UpdateWarnaTombolKontrol();
        Debug.Log("Kontrol Manual dipilih");
    }

    public void SetOtomatis()
    {
        PlayerPrefs.SetInt("kontrol", 1);
        PlayerPrefs.Save();
        UpdateWarnaTombolKontrol();
        Debug.Log("Kontrol Otomatis dipilih");
    }

    // =====================
    // ⚡ SPEED UNIT
    // =====================
    public void SetKMH()
    {
        PlayerPrefs.SetString("speed_unit", "KMH");
        UpdateWarnaTombolSpeedUnit();
    }

    public void SetMPH()
    {
        PlayerPrefs.SetString("speed_unit", "MPH");
        UpdateWarnaTombolSpeedUnit();
    }

    // =====================
    // 📷 KAMERA
    // =====================
    public void SetCameraNear()
    {
        PlayerPrefs.SetInt("camera_mode", 0);
        PlayerPrefs.Save();
        UpdateWarnaTombolKamera();
    }

    public void SetCameraMedium()
    {
        PlayerPrefs.SetInt("camera_mode", 1);
        PlayerPrefs.Save();
        UpdateWarnaTombolKamera();
    }

    public void SetCameraFar()
    {
        PlayerPrefs.SetInt("camera_mode", 2);
        PlayerPrefs.Save();
        UpdateWarnaTombolKamera();
    }

    // =====================
    // 🔥 FUNGSI UPDATE WARNA
    // =====================
    void UpdateWarnaTombolKontrol()
    {
        if (tombolManual == null || tombolOtomatis == null) return;
        
        int kontrol = PlayerPrefs.GetInt("kontrol", 0);
        
        tombolManual.color = (kontrol == 0) ? warnaPilih : warnaNormal;
        tombolOtomatis.color = (kontrol == 1) ? warnaPilih : warnaNormal;
    }

    void UpdateWarnaTombolKamera()
    {
        if (tombolDekat == null || tombolSedang == null || tombolJauh == null) return;

        int mode = PlayerPrefs.GetInt("camera_mode", 1);

        tombolDekat.color = (mode == 0) ? warnaPilih : warnaNormal;
        tombolSedang.color = (mode == 1) ? warnaPilih : warnaNormal;
        tombolJauh.color = (mode == 2) ? warnaPilih : warnaNormal;
    }

    void UpdateWarnaTombolSpeedUnit()
    {
        if (tombolKMH == null || tombolMPH == null) return;

        string unit = PlayerPrefs.GetString("speed_unit", "KMH");

        tombolKMH.color = (unit == "KMH") ? warnaPilih : warnaNormal;
        tombolMPH.color = (unit == "MPH") ? warnaPilih : warnaNormal;
    }
}