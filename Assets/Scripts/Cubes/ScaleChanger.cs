using UnityEngine;

public class ScaleChanger : MonoBehaviour
{
    // 用于在 Inspector 中设置球体的尺寸变化倍数，默认设置为2
    public float sizeMultiplier = 2f;

    private void OnTriggerEnter(Collider other)
    {
        // 检查触发器进入的物体是否是球体
        if (other.CompareTag("Ball"))
        {
            // 获取进入触发器的球体的 Transform 组件
            Transform ballTransform = other.transform;

            if (ballTransform != null)
            {
                // 改变球体的尺寸，默认是原来尺寸的倍数
                ballTransform.localScale = ballTransform.localScale * sizeMultiplier;
            }
        }
    }
}