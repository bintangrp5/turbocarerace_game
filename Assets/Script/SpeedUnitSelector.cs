using UnityEngine;
using UnityEngine.UI;

public class SpeedUnitSelector : MonoBehaviour
{
    public Image kmhImage;
    public Image mphImage;

    public Color selectedColor = new Color32(106, 141, 173, 255);
    public Color normalColor = Color.white;

    void Start()
    {
        LoadSelection();
    }

    public void SelectKMH()
    {
        PlayerPrefs.SetInt("SpeedUnit", 0);
        PlayerPrefs.Save();
        UpdateVisual(0);
    }

    public void SelectMPH()
    {
        PlayerPrefs.SetInt("SpeedUnit", 1);
        PlayerPrefs.Save();
        UpdateVisual(1);
    }

    void LoadSelection()
    {
        int unit = PlayerPrefs.GetInt("SpeedUnit", 0);
        UpdateVisual(unit);
    }

    void UpdateVisual(int unit)
    {
        if (unit == 0)
        {
            kmhImage.color = selectedColor;
            mphImage.color = normalColor;
        }
        else
        {
            mphImage.color = selectedColor;
            kmhImage.color = normalColor;
        }
    }
}