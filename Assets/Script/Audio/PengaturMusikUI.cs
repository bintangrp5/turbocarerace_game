using UnityEngine;
using UnityEngine.UI;

public class PengaturMusikUI : MonoBehaviour
{
    public Slider sliderMusik;
    public AudioClip musikSceneIni; // Masukkan lagu .mp3 untuk scene ini di Inspector

    void Start()
    {
        // 1. Sesuaikan posisi tuas slider dengan memori
        if (PlayerPrefs.HasKey("SimpanVolumeMusik"))
        {
            if (sliderMusik != null)
                sliderMusik.value = PlayerPrefs.GetFloat("SimpanVolumeMusik");
        }
        else
        {
            if (sliderMusik != null)
                sliderMusik.value = 1f;
        }

        // 2. Putar musik khusus untuk scene ini
        if (MusicManager.instance != null && musikSceneIni != null)
        {
            MusicManager.instance.PlayMusic(musikSceneIni);
        }
    }

    // Sambungkan fungsi ini ke event On Value Changed di Slider
    public void UbahVolume(float nilai)
    {
        if (MusicManager.instance != null)
        {
            MusicManager.instance.SetVolume(nilai);
        }
    }
}