using UnityEngine;
using UnityEngine.UI;

public class PengaturMusikUI : MonoBehaviour
{
    public Slider sliderMusik;
    public AudioClip musikSceneIni; // Lagu khusus scene ini

    void Start()
    {
        // 1. Sinkronkan slider dari MusicManager (sumber kebenaran tunggal)
        if (MusicManager.instance != null && sliderMusik != null)
        {
            // SetValueWithoutNotify agar tidak memicu OnValueChanged saat inisialisasi
            sliderMusik.SetValueWithoutNotify(MusicManager.instance.audioSource.volume);
        }

        // 2. Putar musik khusus scene ini
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