using UnityEngine;

public class Destroy : MonoBehaviour
{
    void Start()
    {
        // 在Start方法中调用Destroy函数，传入对象和延迟时间
        Destroy(gameObject, 20f);
    }
}