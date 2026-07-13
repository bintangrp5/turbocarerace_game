using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    public AudioSource audioSource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Muat volume yang tersimpan saat game baru dibuka
        if (PlayerPrefs.HasKey("SimpanVolumeMusik"))
        {
            audioSource.volume = PlayerPrefs.GetFloat("SimpanVolumeMusik");
        }
        else
        {
            audioSource.volume = 1f; // default volume
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        // Cek jika lagu yang mau diputar sama dengan yang sedang menyala,
        // abaikan agar lagunya tidak tersendat/mengulang dari awal.
        if (audioSource.clip == clip) return;

        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void SetVolume(float value)
    {
        audioSource.volume = value;
        
        // Simpan nilai volume setiap kali diubah
        PlayerPrefs.SetFloat("SimpanVolumeMusik", value);
        PlayerPrefs.Save();
    }
}