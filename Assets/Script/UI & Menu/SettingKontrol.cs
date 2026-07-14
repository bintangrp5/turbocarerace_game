using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SettingKontrol : MonoBehaviour
{
    public TMP_Text previewSpeedText;

    [Header("Referensi Live Update")]
    public CarController car; // drag GameObject Car2 di Inspector
    public CinemachineDistanceSetup cameraDistanceSetup; // drag GameObject FreeLook Camera di Inspector

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

    private Color tombolManualColorAsli;
    private Color tombolOtomatisColorAsli;
    private Color tombolDekatColorAsli;
    private Color tombolSedangColorAsli;
    private Color tombolJauhColorAsli;
    private Color tombolKMHColorAsli;
    private Color tombolMPHColorAsli;

    private Color32 warnaPilih = new Color32(106, 141, 173, 255);

    void Start()
    {
        // Simpan warna asli tiap tombol sebelum diubah script
        if (tombolManual != null) tombolManualColorAsli = tombolManual.color;
        if (tombolOtomatis != null) tombolOtomatisColorAsli = tombolOtomatis.color;
        if (tombolDekat != null) tombolDekatColorAsli = tombolDekat.color;
        if (tombolSedang != null) tombolSedangColorAsli = tombolSedang.color;
        if (tombolJauh != null) tombolJauhColorAsli = tombolJauh.color;
        if (tombolKMH != null) tombolKMHColorAsli = tombolKMH.color;
        if (tombolMPH != null) tombolMPHColorAsli = tombolMPH.color;

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

        if (car != null) car.ApplySettingsFromPrefs();
        Debug.Log("Kontrol Manual dipilih");
    }

    public void SetOtomatis()
    {
        PlayerPrefs.SetInt("kontrol", 1);
        PlayerPrefs.Save();
        UpdateWarnaTombolKontrol();

        if (car != null) car.ApplySettingsFromPrefs();
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

        if (cameraDistanceSetup != null) cameraDistanceSetup.ApplySettingsFromPrefs();
    }

    public void SetCameraMedium()
    {
        PlayerPrefs.SetInt("camera_mode", 1);
        PlayerPrefs.Save();
        UpdateWarnaTombolKamera();

        if (cameraDistanceSetup != null) cameraDistanceSetup.ApplySettingsFromPrefs();
    }

    public void SetCameraFar()
    {
        PlayerPrefs.SetInt("camera_mode", 2);
        PlayerPrefs.Save();
        UpdateWarnaTombolKamera();

        if (cameraDistanceSetup != null) cameraDistanceSetup.ApplySettingsFromPrefs();
    }

    // =====================
    // 🔥 FUNGSI UPDATE WARNA
    // =====================
    void UpdateWarnaTombolKontrol()
    {
        if (tombolManual == null || tombolOtomatis == null) return;

        int kontrol = PlayerPrefs.GetInt("kontrol", 0);

        tombolManual.color = (kontrol == 0) ? warnaPilih : tombolManualColorAsli;
        tombolOtomatis.color = (kontrol == 1) ? warnaPilih : tombolOtomatisColorAsli;
    }

    void UpdateWarnaTombolKamera()
    {
        if (tombolDekat == null || tombolSedang == null || tombolJauh == null) return;

        int mode = PlayerPrefs.GetInt("camera_mode", 1);

        tombolDekat.color = (mode == 0) ? warnaPilih : tombolDekatColorAsli;
        tombolSedang.color = (mode == 1) ? warnaPilih : tombolSedangColorAsli;
        tombolJauh.color = (mode == 2) ? warnaPilih : tombolJauhColorAsli;
    }

    void UpdateWarnaTombolSpeedUnit()
    {
        if (tombolKMH == null || tombolMPH == null) return;

        string unit = PlayerPrefs.GetString("speed_unit", "KMH");

        tombolKMH.color = (unit == "KMH") ? warnaPilih : tombolKMHColorAsli;
        tombolMPH.color = (unit == "MPH") ? warnaPilih : tombolMPHColorAsli;
    }
}