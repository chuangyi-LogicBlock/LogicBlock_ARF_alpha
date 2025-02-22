using UnityEngine;
using System.Collections.Generic;

public class PanelManager : MonoBehaviour
{
    private static PanelManager instance;
    private List<GameObject> activePanels = new List<GameObject>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static PanelManager GetInstance()
    {
        return instance;
    }

    public void AddPanel(GameObject panel)
    {
        if (!activePanels.Contains(panel))
        {
            activePanels.Add(panel);
        }
    }

    public void HideAllPanels()
    {
        foreach (GameObject panel in activePanels)
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }
    }

    public void ShowPanel(GameObject panel)
    {
        HideAllPanels();
        if (panel != null)
        {
            panel.SetActive(true);
        }
    }
}