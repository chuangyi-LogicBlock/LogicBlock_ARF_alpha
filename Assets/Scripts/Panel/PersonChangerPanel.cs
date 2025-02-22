using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PersonChangerPanel : MonoBehaviour
{
    public GameObject parameterPanel;  // 参数调整面板
    public Dropdown themeDropdown;     // 主题下拉框
    public Dropdown modelDropdown;     // 模型下拉框

    private PersonChanger cubeChangePerson; // 改变小人的 Cube 脚本
    public ThemeManager themeManager;        // 主题管理脚本

    void Start()
    {
        // 隐藏参数调整面板
        parameterPanel.SetActive(false);

        // 初始化主题下拉框
        List<string> themeNames = new List<string>();
        foreach (ThemeData theme in themeManager.themes)
        {
            themeNames.Add(theme.themeName);
        }
        themeDropdown.AddOptions(themeNames);
        themeDropdown.onValueChanged.AddListener(OnThemeSelected);

        // 初始化模型下拉框
        UpdateModelDropdown(0);
        modelDropdown.onValueChanged.AddListener(OnModelSelected);
    }

    public void ShowParameterPanel(PersonChanger cube)
    {
        cubeChangePerson = cube;

        // 显示参数面板
        parameterPanel.SetActive(true);
    }

    // 隐藏参数调整面板
    public void HideParameterPanel()
    {
        parameterPanel.SetActive(false);
        cubeChangePerson = null;
    }

    private void UpdateModelDropdown(int themeIndex)
    {
        modelDropdown.ClearOptions();
        List<string> modelNames = new List<string>();
        ThemeData selectedTheme = themeManager.themes[themeIndex];
        foreach (GameObject model in selectedTheme.personModels)
        {
            modelNames.Add(model.name);
        }
        modelDropdown.AddOptions(modelNames);
    }

    private void OnThemeSelected(int themeIndex)
    {
        UpdateModelDropdown(themeIndex);
        OnModelSelected(0);
    }

    private void OnModelSelected(int modelIndex)
    {
        if (cubeChangePerson != null)
        {
            int themeIndex = themeDropdown.value;
            ThemeData selectedTheme = themeManager.themes[themeIndex];
            cubeChangePerson.newPersonPrefab = selectedTheme.personModels[modelIndex];
        }
    }
}