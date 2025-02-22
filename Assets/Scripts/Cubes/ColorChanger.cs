using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    // 用于在 Inspector 中更改球体的颜色
    public Color newColor = Color.white;

    private void OnTriggerEnter(Collider other)
    {
        // 检查触发器进入的物体是否是球体
        if (other.CompareTag("Ball"))
        {
            // 获取进入触发器的球体的 Renderer 组件
            Renderer ballRenderer = other.GetComponent<Renderer>();

            if (ballRenderer != null)
            {
                // 改变球体的颜色
                ballRenderer.material.color = newColor;
            }
        }
    }

    // 设置颜色的方法
    public void SetNewColor(Color color)
    {
        newColor = color;
    }
}