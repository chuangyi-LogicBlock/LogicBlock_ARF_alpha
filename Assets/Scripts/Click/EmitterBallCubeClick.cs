using UnityEngine;
using UnityEngine.EventSystems;

public class EmitterBallCubeClick : MonoBehaviour, IPointerClickHandler
{
    public GameObject emitterBallPanelPrefab;
    private GameObject currentPanelInstance;
    private Canvas canvas;

    void Start()
    {
        if (emitterBallPanelPrefab == null)
        {
            Debug.LogWarning("EmitterBallPanel Prefab is not assigned!");
        }
        canvas = FindObjectOfType<Canvas>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Emitter_Ball emitter = GetComponent<Emitter_Ball>();
        if (emitter != null && emitterBallPanelPrefab != null)
        {
            PanelManager panelManager = PanelManager.GetInstance();
            panelManager.HideAllPanels();

            if (currentPanelInstance == null)
            {
                currentPanelInstance = Instantiate(emitterBallPanelPrefab);
                if (canvas != null)
                {
                    currentPanelInstance.transform.SetParent(canvas.transform, false);
                }
                panelManager.AddPanel(currentPanelInstance);
            }

            EmitterBallPanel parameterEditor = currentPanelInstance.GetComponent<EmitterBallPanel>();
            if (parameterEditor != null)
            {
                parameterEditor.ShowParameterPanel(emitter);
            }

            currentPanelInstance.SetActive(true);
        }
    }
}