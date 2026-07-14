using UnityEngine;
using Unity.Cinemachine;

public class CinemachineDistanceSetup : MonoBehaviour
{
    private CinemachineCamera freeLookCam;

    [Header("Pengaturan Zoom (FOV)")]
    public float fovDekat = 40f;
    public float fovSedang = 60f;
    public float fovJauh = 80f;

    void Start()
    {
        freeLookCam = GetComponent<CinemachineCamera>();
        ApplySettingsFromPrefs();
    }

    // Bisa dipanggil kapan saja untuk re-sync jarak kamera dari PlayerPrefs
    public void ApplySettingsFromPrefs()
    {
        if (freeLookCam == null) freeLookCam = GetComponent<CinemachineCamera>();

        int mode = PlayerPrefs.GetInt("camera_mode", 1);

        switch (mode)
        {
            case 0:
                freeLookCam.Lens.FieldOfView = fovDekat;
                Debug.Log("Menerapkan Kamera Dekat");
                break;
            case 1:
                freeLookCam.Lens.FieldOfView = fovSedang;
                Debug.Log("Menerapkan Kamera Sedang");
                break;
            case 2:
                freeLookCam.Lens.FieldOfView = fovJauh;
                Debug.Log("Menerapkan Kamera Jauh");
                break;
        }
    }
}