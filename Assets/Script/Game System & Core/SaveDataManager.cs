using UnityEngine;

public static class SaveDataManager
{
    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey("Level1Done");
        PlayerPrefs.DeleteKey("Level2Unlocked");
        PlayerPrefs.DeleteKey("Level2Done");
        PlayerPrefs.DeleteKey("Level3Unlocked");
        PlayerPrefs.Save();

        Debug.Log("Progress level di-reset. Level 2 & 3 terkunci kembali.");
    }
}