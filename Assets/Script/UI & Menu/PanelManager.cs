using UnityEngine;
using UnityEngine.SceneManagement;

public class PanelManager : MonoBehaviour
{
    public GameObject[] panels;

    public void ShowPanel(GameObject panelToShow)
    {
        Debug.Log("ShowPanel dipanggil untuk: " + panelToShow.name);

        foreach (GameObject p in panels)
        {
            p.SetActive(false);
        }

        panelToShow.SetActive(true);

        Debug.Log(panelToShow.name + " sekarang aktif? " + panelToShow.activeSelf);
    }
    public void PlayGame()
    {
        SceneManager.LoadScene("Level_1");
    }

    public void QuitApplication()
    {
        Debug.Log("[PanelManager] Keluar dari game...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}