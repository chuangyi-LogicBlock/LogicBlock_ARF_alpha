using UnityEngine;
using UnityEngine.UI;

public class SoundPanel : MonoBehaviour
{
    public GameObject parameterPanel; // 参数调整面板
    public Dropdown soundDropdown; // 下拉框用于选择声音
    public AudioClip[] soundOptions; // 存储所有可选声音
    private PlaySound currentPlaySound; // 当前选中的 PlaySound 脚本

    void Start()
    {
        // 初始化隐藏参数调整面板
        parameterPanel.SetActive(false);

        // 填充下拉框选项
        PopulateDropdown();

        // 监听下拉框选择变化
        soundDropdown.onValueChanged.AddListener(OnSoundSelectionChanged);
    }

    // 显示面板并初始化
    public void ShowParameterPanel(PlaySound playSound)
    {
        currentPlaySound = playSound;

        // 设置当前选中的声音在下拉框中的值
        int currentIndex = GetCurrentSoundIndex(playSound.collisionSound);
        soundDropdown.value = currentIndex;

        // 使用 PanelManager 显示面板
        PanelManager.GetInstance().ShowPanel(parameterPanel);
    }

    // 隐藏参数调整面板
    public void HideParameterPanel()
    {
        parameterPanel.SetActive(false);
        currentPlaySound = null;
    }

    // 当选择声音时的回调
    private void OnSoundSelectionChanged(int index)
    {
        if (currentPlaySound != null && index >= 0 && index < soundOptions.Length)
        {
            currentPlaySound.SetCollisionSound(soundOptions[index]);
        }
    }

    // 填充下拉框选项
    private void PopulateDropdown()
    {
        soundDropdown.ClearOptions();
        foreach (var sound in soundOptions)
        {
            soundDropdown.options.Add(new Dropdown.OptionData(sound.name));
        }
    }

    // 获取当前声音在数组中的索引
    private int GetCurrentSoundIndex(AudioClip sound)
    {
        for (int i = 0; i < soundOptions.Length; i++)
        {
            if (soundOptions[i] == sound)
            {
                return i;
            }
        }
        return 0; // 默认选择第一个声音
    }
}