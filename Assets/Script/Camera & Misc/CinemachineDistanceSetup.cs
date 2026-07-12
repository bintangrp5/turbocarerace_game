using UnityEngine;
using Unity.Cinemachine; // Wajib untuk Cinemachine 3

public class CinemachineDistanceSetup : MonoBehaviour
{
    private CinemachineCamera freeLookCam;

    [Header("Pengaturan Zoom (FOV)")]
    public float fovDekat = 40f;
    public float fovSedang = 60f;
    public float fovJauh = 80f;

    void Start()
    {
        // Ambil komponen kamera Cinemachine yang ada di objek ini
        freeLookCam = GetComponent<CinemachineCamera>();

        // Baca data yang disimpan oleh SettingKontrol di Main Menu
        // Kuncinya ("camera_mode") HARUS SAMA PERSIS dengan yang ada di SettingKontrol
        int mode = PlayerPrefs.GetInt("camera_mode", 1); 

        // Terapkan pengaturan kamera sesuai angka yang disimpan
        switch (mode)
        {
            case 0:
                freeLookCam.Lens.FieldOfView = fovDekat;
                Debug.Log("Game dimulai: Menerapkan Kamera Dekat");
                break;
            case 1:
                freeLookCam.Lens.FieldOfView = fovSedang;
                Debug.Log("Game dimulai: Menerapkan Kamera Sedang");
                break;
            case 2:
                freeLookCam.Lens.FieldOfView = fovJauh;
                Debug.Log("Game dimulai: Menerapkan Kamera Jauh");
                break;
        }
    }
}