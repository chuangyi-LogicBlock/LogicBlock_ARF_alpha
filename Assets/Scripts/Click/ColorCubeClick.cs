using UnityEngine;
using UnityEngine.EventSystems;

public class ColorCubeClick : MonoBehaviour, IPointerClickHandler
{
    public GameObject ColorPanelPrefab;
    private GameObject currentPanelInstance;
    private Canvas canvas;

    void Start()
    {
        if (ColorPanelPrefab == null)
        {
            Debug.LogWarning("ColorPanel Prefab is not assigned!");
        }
        canvas = FindObjectOfType<Canvas>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ColorChanger color = GetComponent<ColorChanger>();
        if (color != null && ColorPanelPrefab != null)
        {
            PanelManager panelManager = PanelManager.GetInstance();
            panelManager.HideAllPanels();

            if (currentPanelInstance == null)
            {
                currentPanelInstance = Instantiate(ColorPanelPrefab);
                if (canvas != null)
                {
                    currentPanelInstance.transform.SetParent(canvas.transform, false);
                }
                panelManager.AddPanel(currentPanelInstance);
            }

            ColorPanel parameterEditor = currentPanelInstance.GetComponent<ColorPanel>();
            if (parameterEditor != null)
            {
                parameterEditor.ShowParameterPanel(color);
            }

            currentPanelInstance.SetActive(true);
        }
    }
}