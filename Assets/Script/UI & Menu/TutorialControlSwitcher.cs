using UnityEngine;

public class TutorialControlSwitcher : MonoBehaviour
{
    [Header("Target Container di Panel_Kontrol")]
    public Transform kontrolContentArea; // area kosong di dalam Panel_Kontrol

    [Header("Sumber Panel dari Panel_Play")]
    public GameObject panelTutorialManual;
    public GameObject panelTutorialOtomatis;

    public void ShowManual()
    {
        panelTutorialManual.transform.SetParent(kontrolContentArea, false);
        panelTutorialOtomatis.transform.SetParent(kontrolContentArea, false);

        panelTutorialManual.SetActive(true);
        panelTutorialOtomatis.SetActive(false);
    }

    public void ShowOtomatis()
    {
        panelTutorialManual.transform.SetParent(kontrolContentArea, false);
        panelTutorialOtomatis.transform.SetParent(kontrolContentArea, false);

        panelTutorialManual.SetActive(false);
        panelTutorialOtomatis.SetActive(true);
    }
}