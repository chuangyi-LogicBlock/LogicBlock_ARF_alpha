using UnityEngine;
using UnityEngine.EventSystems;

public class SoundCubeClick : MonoBehaviour, IPointerClickHandler
{
    public GameObject SoundPanelPrefab;
    private GameObject currentPanelInstance;
    private Canvas canvas;

    void Start()
    {
        if (SoundPanelPrefab == null)
        {
            Debug.LogWarning("SoundPanel Prefab is not assigned!");
        }
        canvas = FindObjectOfType<Canvas>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlaySound sound = GetComponent<PlaySound>();
        if (sound != null && SoundPanelPrefab != null)
        {
            PanelManager panelManager = PanelManager.GetInstance();
            panelManager.HideAllPanels();

            if (currentPanelInstance == null)
            {
                currentPanelInstance = Instantiate(SoundPanelPrefab);
                if (canvas != null)
                {
                    currentPanelInstance.transform.SetParent(canvas.transform, false);
                }
                panelManager.AddPanel(currentPanelInstance);
            }

            SoundPanel parameterEditor = currentPanelInstance.GetComponent<SoundPanel>();
            if (parameterEditor != null)
            {
                parameterEditor.ShowParameterPanel(sound);
            }

            currentPanelInstance.SetActive(true);
        }
    }
}