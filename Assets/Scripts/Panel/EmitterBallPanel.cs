using UnityEngine;
using UnityEngine.UI;

public class EmitterBallPanel : MonoBehaviour
{
    public GameObject parameterPanel;  // 参数调整面板
    public Slider shootIntervalSlider; // 调整 shootInterval 的滑块
    public Slider shootForceSlider;    // 调整 shootForce 的滑块
    public Text shootIntervalText;     // 显示 shootInterval 的值
    public Text shootForceText;        // 显示 shootForce 的值

    private Emitter_Ball currentEmitter;  // 当前选中的 Emitter 脚本

    void Start()
    {
        // 隐藏参数调整面板
        parameterPanel.SetActive(false);

        // 设置滑块事件
        shootIntervalSlider.onValueChanged.AddListener(OnShootIntervalChanged);
        shootForceSlider.onValueChanged.AddListener(OnShootForceChanged);
    }

    public void ShowParameterPanel(Emitter_Ball emitter)
    {
        currentEmitter = emitter;

        // 初始化滑块值
        shootIntervalSlider.value = currentEmitter.shootInterval;
        shootForceSlider.value = currentEmitter.shootForce;

        // 更新显示文本
        UpdateText();

        // 显示参数面板
        parameterPanel.SetActive(true);
    }

    // 隐藏参数调整面板
    public void HideParameterPanel()
    {
        parameterPanel.SetActive(false);
        currentEmitter = null;
    }

    // 当调整 shootInterval 滑块时
    private void OnShootIntervalChanged(float value)
    {
        if (currentEmitter != null)
        {
            currentEmitter.shootInterval = value;
            UpdateText();
        }
    }

    // 当调整 shootForce 滑块时
    private void OnShootForceChanged(float value)
    {
        if (currentEmitter != null)
        {
            currentEmitter.shootForce = value;
            UpdateText();
        }
    }

    // 更新显示文本
    private void UpdateText()
    {
        shootIntervalText.text = $"发射间隔: {shootIntervalSlider.value:F2}s";
        shootForceText.text = $"发射速度: {shootForceSlider.value:F2}";
    }
}