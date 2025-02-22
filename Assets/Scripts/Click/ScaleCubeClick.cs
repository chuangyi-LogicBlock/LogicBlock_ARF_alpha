using UnityEngine;
using UnityEngine.EventSystems;

public class ScaleCubeClick : MonoBehaviour, IPointerClickHandler
{
    public GameObject ScalePanelPrefab;
    private GameObject currentPanelInstance;
    private Canvas canvas;

    void Start()
    {
        if (ScalePanelPrefab == null)
        {
            Debug.LogWarning("ScalePanel Prefab is not assigned!");
        }
        canvas = FindObjectOfType<Canvas>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ScaleChanger scale = GetComponent<ScaleChanger>();
        if (scale != null && ScalePanelPrefab != null)
        {
            PanelManager panelManager = PanelManager.GetInstance();
            panelManager.HideAllPanels();

            if (currentPanelInstance == null)
            {
                currentPanelInstance = Instantiate(ScalePanelPrefab);
                if (canvas != null)
                {
                    currentPanelInstance.transform.SetParent(canvas.transform, false);
                }
                panelManager.AddPanel(currentPanelInstance);
            }

            ScalePanel parameterEditor = currentPanelInstance.GetComponent<ScalePanel>();
            if (parameterEditor != null)
            {
                parameterEditor.ShowParameterPanel(scale);
            }

            currentPanelInstance.SetActive(true);
        }
    }
}