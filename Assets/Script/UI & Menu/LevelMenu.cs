using UnityEngine;
using UnityEngine.UI;

public class LevelMenu : MonoBehaviour
{
    public Button level2Button;
    public Button level3Button;

    void Start()
    {
        // Cek apakah Level 1 sudah selesai
        level2Button.interactable = PlayerPrefs.GetInt("Level1Done", 0) == 1;
        
        // Cek apakah Level 2 sudah selesai (jika ada level 3)
        if (level3Button != null)
            level3Button.interactable = PlayerPrefs.GetInt("Level2Done", 0) == 1;

        Debug.Log("Level1Done = " + PlayerPrefs.GetInt("Level1Done", 0));
        Debug.Log("Level2Unlocked = " + PlayerPrefs.GetInt("Level2Unlocked", 0));
    }
}