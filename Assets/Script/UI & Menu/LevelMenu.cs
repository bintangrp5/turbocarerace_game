using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelMenu : MonoBehaviour
{
    [Header("Tombol Level")]
    public Button level2Button;
    public Button level3Button;
    public Button detailLevel2Button;

    [Header("Pengaturan Panel")]
    public GameObject panelTutorialManual;
    public GameObject panelTutorialOtomatis;
    public GameObject panelDetailInfo;

    private string levelTujuan = "";

    void Start()
    {
        bool level1Selesai = PlayerPrefs.GetInt("Level1Done", 0) == 1;

        if (level2Button != null)
            level2Button.interactable = level1Selesai;

        if (detailLevel2Button != null)
            detailLevel2Button.interactable = level1Selesai;

        bool level2Selesai = PlayerPrefs.GetInt("Level2Done", 0) == 1;

        if (level3Button != null)
            level3Button.interactable = level2Selesai;
    }

    // Dipanggil saat tombol level (Level 1, 2, dst) diklik
    // Sekarang: buka panel info/rules dulu, BELUM buka panel kontrol
    public void PilihLevel(string namaScene)
    {
        levelTujuan = namaScene;

        // Pastikan panel kontrol belum muncul dulu
        if (panelTutorialManual != null) panelTutorialManual.SetActive(false);
        if (panelTutorialOtomatis != null) panelTutorialOtomatis.SetActive(false);

        // Tampilkan panel info/rules
        if (panelDetailInfo != null) panelDetailInfo.SetActive(true);

        Debug.Log("Level dipilih: " + namaScene + ". Menampilkan info/rules.");
    }

    // BARU: Dipanggil dari tombol "Lanjut"/"Mengerti" di dalam panelDetailInfo
    public void LanjutKeKontrol()
    {
        if (panelDetailInfo != null) panelDetailInfo.SetActive(false);

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

    // Dipanggil saat tombol "Mulai Balapan" diklik — TIDAK BERUBAH
    public void GasMulaiBalapan()
    {
        if (levelTujuan != "")
        {
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

    // Dipanggil oleh tombol "Kembali" di panel Detail Info — TIDAK BERUBAH
    public void TutupDetailInfo()
    {
        if (panelDetailInfo != null) panelDetailInfo.SetActive(false);
    }
}