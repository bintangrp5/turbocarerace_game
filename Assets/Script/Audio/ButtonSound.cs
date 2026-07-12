using UnityEngine;
using UnityEngine.UI; // Tambahan wajib agar Unity mengenali komponen Slider

public class ButtonSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clickSound;
    
    // Tambahkan variabel ini untuk menghubungkan slider dari Inspector
    public Slider sliderSuara; 

    [Range(0f, 1f)]
    public float volume = 1f;

    void Start()
    {
        // 1. Cek apakah ada volume yang tersimpan di memori perangkat
        if (PlayerPrefs.HasKey("SimpanVolume"))
        {
            // Jika ada, timpa nilai default dengan nilai yang tersimpan
            volume = PlayerPrefs.GetFloat("SimpanVolume"); 
        }

        // 2. Terapkan nilai volume ke Audio Source
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }

        // 3. Sesuaikan posisi tuas Slider agar sinkron dengan volume saat ini
        if (sliderSuara != null)
        {
            sliderSuara.value = volume;
        }
    }

    public void PlaySound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.clip = clickSound;
            audioSource.Play();
        }
    }

    public void SetVolume(float v)
    {
        volume = v;

        if (audioSource != null)
        {
            audioSource.volume = v;
        }

        // 4. Simpan nilai volume ke memori setiap kali slider digeser
        PlayerPrefs.SetFloat("SimpanVolume", v);
        PlayerPrefs.Save();
    }
}