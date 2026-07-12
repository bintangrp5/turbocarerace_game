using UnityEngine;

public class UIKontrolSelector : MonoBehaviour
{
    public GameObject statusPanel1;
    public GameObject statusPanel2;

    public void PilihManual()
    {
        statusPanel1.SetActive(true);
        statusPanel2.SetActive(false);

        PlayerPrefs.SetInt("kontrol", 0);
        PlayerPrefs.Save();
    }

    public void PilihOtomatis()
    {
        statusPanel1.SetActive(false);
        statusPanel2.SetActive(true);

        PlayerPrefs.SetInt("kontrol", 1);
        PlayerPrefs.Save();
    }

    void Start()
    {
        int kontrol = PlayerPrefs.GetInt("kontrol", 0);

        if (kontrol == 0)
            PilihManual();
        else
            PilihOtomatis();
    }
}