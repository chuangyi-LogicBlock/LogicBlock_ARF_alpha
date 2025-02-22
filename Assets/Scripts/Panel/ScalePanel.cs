using UnityEngine;
using UnityEngine.UI;

public class ScalePanel : MonoBehaviour
{
    public GameObject parameterPanel;  // 参数调整面板
    public Slider sizeSlider;          // 控制尺寸的滑块
    public Text sizeText;              // 显示尺寸倍数的文本

    private ScaleChanger currentScaleChanger; // 当前选中的 ScaleChanger 脚本

    void Start()
    {
        // 设置滑块事件
        sizeSlider.onValueChanged.AddListener(OnSizeSliderChanged);
    }

    // 显示面板并初始化滑块
    public void ShowParameterPanel(ScaleChanger scaleChanger)
    {
        currentScaleChanger = scaleChanger;

        // 初始化滑块值
        sizeSlider.value = currentScaleChanger.sizeMultiplier;

        // 更新显示文本
        UpdateText();

        // 显示面板
        PanelManager.GetInstance().ShowPanel(parameterPanel);
    }

    // 隐藏参数调整面板
    public void HideParameterPanel()
    {
        parameterPanel.SetActive(false);
        currentScaleChanger = null;
    }

    // 当调整尺寸倍数时
    private void OnSizeSliderChanged(float value)
    {
        if (currentScaleChanger != null)
        {
            currentScaleChanger.sizeMultiplier = value;
            UpdateText();
        }
    }

    // 更新显示文本
    private void UpdateText()
    {
        sizeText.text = $"Size: {sizeSlider.value:F2}";
    }
}