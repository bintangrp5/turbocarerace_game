using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public GameObject[] panels;

    public void ShowPanel(GameObject panelToShow)
    {
        foreach (GameObject p in panels)
        {
            p.SetActive(false);
        }

        panelToShow.SetActive(true);
    }
}