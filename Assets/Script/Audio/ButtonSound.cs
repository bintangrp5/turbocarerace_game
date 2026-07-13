using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    public AudioSource audioSource;
    
    // Siapkan 2 slot untuk efek suara tombol yang berbeda
    public AudioClip suaraTombol1; 
    public AudioClip suaraTombol2; 
    
    public Slider sliderSuara; 

    [Range(0f, 1f)]
    public float volume = 1f;

    void Start()
    {
        // 1. Cek apakah ada volume yang tersimpan di memori perangkat
        if (PlayerPrefs.HasKey("SimpanVolume"))
        {
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

    // Fungsi untuk memutar efek suara tombol pertama
    public void PlaySound1()
    {
        if (audioSource != null && suaraTombol1 != null)
        {
            // PlayOneShot membuat suara tidak terpotong jika tombol dipencet cepat
            audioSource.PlayOneShot(suaraTombol1);
        }
    }

    // Fungsi untuk memutar efek suara tombol kedua
    public void PlaySound2()
    {
        if (audioSource != null && suaraTombol2 != null)
        {
            audioSource.PlayOneShot(suaraTombol2);
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