using UnityEngine;
using UnityEngine.EventSystems;

public class PersonChangerCubeClick : MonoBehaviour, IPointerClickHandler
{
    public GameObject PersonChangerPanelPrefab;
    private GameObject currentPanelInstance;
    private Canvas canvas;

    void Start()
    {
        if (PersonChangerPanelPrefab == null)
        {
            Debug.LogWarning("PersonChangerPanel Prefab is not assigned!");
        }
        canvas = FindObjectOfType<Canvas>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PersonChanger pc = GetComponent<PersonChanger>();
        if (pc != null && PersonChangerPanelPrefab != null)
        {
            PanelManager panelManager = PanelManager.GetInstance();
            panelManager.HideAllPanels();

            if (currentPanelInstance == null)
            {
                currentPanelInstance = Instantiate(PersonChangerPanelPrefab);
                if (canvas != null)
                {
                    currentPanelInstance.transform.SetParent(canvas.transform, false);
                }
                panelManager.AddPanel(currentPanelInstance);
            }

            PersonChangerPanel parameterEditor = currentPanelInstance.GetComponent<PersonChangerPanel>();
            if (parameterEditor != null)
            {
                parameterEditor.ShowParameterPanel(pc);
            }

            currentPanelInstance.SetActive(true);
        }
    }
}