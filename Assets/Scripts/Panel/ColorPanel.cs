using UnityEngine;
using UnityEngine.UI;

public class ColorPanel : MonoBehaviour
{
    public GameObject parameterPanel;  // 参数调整面板
    public Slider redSlider;           // 红色通道滑块
    public Slider greenSlider;         // 绿色通道滑块
    public Slider blueSlider;          // 蓝色通道滑块
    public Text redText;               // 显示红色值
    public Text greenText;             // 显示绿色值
    public Text blueText;              // 显示蓝色值
    public Image colorDisplayImage;    // 显示颜色的 Image 组件

    private ColorChanger currentColorChanger; // 当前选中的 ColorChanger 脚本

    void Start()
    {
        // 初始化隐藏参数调整面板
        parameterPanel.SetActive(false);

        // 设置滑块事件
        redSlider.onValueChanged.AddListener(OnRedSliderChanged);
        greenSlider.onValueChanged.AddListener(OnGreenSliderChanged);
        blueSlider.onValueChanged.AddListener(OnBlueSliderChanged);
    }

    // 显示面板并初始化滑块
    public void ShowParameterPanel(ColorChanger colorChanger)
    {
        currentColorChanger = colorChanger;

        // 初始化滑块值
        redSlider.value = currentColorChanger.newColor.r;
        greenSlider.value = currentColorChanger.newColor.g;
        blueSlider.value = currentColorChanger.newColor.b;

        // 更新显示文本
        UpdateText();

        // 更新颜色显示
        UpdateColorDisplay();

        // 使用 PanelManager 显示面板
        PanelManager.GetInstance().ShowPanel(parameterPanel);
    }

    // 隐藏参数调整面板
    public void HideParameterPanel()
    {
        parameterPanel.SetActive(false);
        currentColorChanger = null;
    }

    // 当调整红色通道时
    private void OnRedSliderChanged(float value)
    {
        if (currentColorChanger != null)
        {
            currentColorChanger.SetNewColor(new Color(value, currentColorChanger.newColor.g, currentColorChanger.newColor.b));
            UpdateText();
            UpdateColorDisplay();
        }
    }

    // 当调整绿色通道时
    private void OnGreenSliderChanged(float value)
    {
        if (currentColorChanger != null)
        {
            currentColorChanger.SetNewColor(new Color(currentColorChanger.newColor.r, value, currentColorChanger.newColor.b));
            UpdateText();
            UpdateColorDisplay();
        }
    }

    // 当调整蓝色通道时
    private void OnBlueSliderChanged(float value)
    {
        if (currentColorChanger != null)
        {
            currentColorChanger.SetNewColor(new Color(currentColorChanger.newColor.r, currentColorChanger.newColor.g, value));
            UpdateText();
            UpdateColorDisplay();
        }
    }

    // 更新显示文本
    private void UpdateText()
    {
        redText.text = $"R: {redSlider.value:F2}";
        greenText.text = $"G: {greenSlider.value:F2}";
        blueText.text = $"B: {blueSlider.value:F2}";
    }

    // 更新颜色显示
    private void UpdateColorDisplay()
    {
        if (colorDisplayImage != null)
        {
            colorDisplayImage.color = new Color(redSlider.value, greenSlider.value, blueSlider.value);
        }
    }
}