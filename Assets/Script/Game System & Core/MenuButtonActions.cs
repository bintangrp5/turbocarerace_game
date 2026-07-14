using UnityEngine;

public class MenuButtonActions : MonoBehaviour
{
    public void OnResetProgressClicked()
    {
        SaveDataManager.ResetProgress();
    }
}