using UnityEngine;
using TMPro;

public class SettingKontrol : MonoBehaviour
{
    public TMP_Text previewSpeedText; // TMP di menu setting

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
        previewSpeedText.text = "KM/JAM";
        Debug.Log("KMH dipilih");

        ApplyColor(); // biar warna ikut update
    }

    public void SetMPH()
    {
        PlayerPrefs.SetString("speed_unit", "MPH");
        previewSpeedText.text = "MPH";
        Debug.Log("MPH dipilih");

        ApplyColor();
    }

    // =====================
    // 🎨 WARNA
    // =====================
    public void SetRed()
    {
        PlayerPrefs.SetString("speed_color", "red");
        ApplyColor();
    }

    public void SetBlue()
    {
        PlayerPrefs.SetString("speed_color", "blue");
        ApplyColor();
    }

    void ApplyColor()
    {
        string color = PlayerPrefs.GetString("speed_color", "white");

        if (color == "red")
            previewSpeedText.color = Color.red;
        else if (color == "blue")
            previewSpeedText.color = new Color32(106, 141, 173, 255);
        else
            previewSpeedText.color = Color.white;
    }
}