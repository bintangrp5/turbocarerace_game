using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelMenu : MonoBehaviour
{
    [Header("Tombol Level")]
    public Button level2Button;
    public Button level3Button;
    public Button detailLevel2Button; // 🔥 Tombol Detail Level 2

    [Header("Pengaturan Panel")]
    public GameObject panelTutorialManual;
    public GameObject panelTutorialOtomatis;
    public GameObject panelDetailInfo;

    private string levelTujuan = "";

    void Start()
    {
        // 1. Cek progress Level 1 (untuk membuka/mengunci Level 2)
        bool level1Selesai = PlayerPrefs.GetInt("Level1Done", 0) == 1;
        
        if (level2Button != null) 
            level2Button.interactable = level1Selesai;
            
        if (detailLevel2Button != null) 
            detailLevel2Button.interactable = level1Selesai; // Mengunci tombol Detail

        // 2. Cek progress Level 2 (untuk membuka/mengunci Level 3)
        bool level2Selesai = PlayerPrefs.GetInt("Level2Done", 0) == 1;
        
        if (level3Button != null) 
            level3Button.interactable = level2Selesai;
    }

    // Dipanggil saat tombol level (Level 1, 2, dst) diklik
    public void PilihLevel(string namaScene)
    {
        levelTujuan = namaScene;

        // Tutup panel detail jika level baru dipilih
        if (panelDetailInfo != null) panelDetailInfo.SetActive(false);

        // Membaca settingan kontrol (0 = Manual, 1 = Otomatis)
        int tipeKontrol = PlayerPrefs.GetInt("kontrol", 0); 

        Debug.Log("Mendeteksi Kontrol: " + tipeKontrol);

        if (tipeKontrol == 0)
        {
            panelTutorialManual.SetActive(true);
            panelTutorialOtomatis.SetActive(false);
        }
        else
        {
            panelTutorialManual.SetActive(false);
            panelTutorialOtomatis.SetActive(true);
        }
    }

    // Dipanggil saat tombol "Mulai Balapan" diklik
    public void GasMulaiBalapan()
    {
        if (levelTujuan != "")
        {
            // Pastikan semua panel tertutup saat balapan mulai
            if (panelDetailInfo != null) panelDetailInfo.SetActive(false);
            if (panelTutorialManual != null) panelTutorialManual.SetActive(false);
            if (panelTutorialOtomatis != null) panelTutorialOtomatis.SetActive(false);
            
            SceneManager.LoadScene(levelTujuan);
        }
        else
        {
            Debug.LogError("levelTujuan kosong! Pastikan PilihLevel terpanggil sebelum klik Mulai.");
        }
    }

    // Dipanggil oleh tombol "Kembali" di panel Detail Info
    public void TutupDetailInfo()
    {
        if (panelDetailInfo != null) panelDetailInfo.SetActive(false);
    }
}