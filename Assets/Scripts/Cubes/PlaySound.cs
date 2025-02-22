using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public AudioClip collisionSound; // 碰撞声音（在 Unity 界面中赋值）
    private AudioSource audioSource; // 用于播放声音

    void Start()
    {
        // 获取或添加 AudioSource 组件
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 设置 AudioSource 的基本属性
        audioSource.playOnAwake = false;
        audioSource.clip = collisionSound;
    }

    void OnCollisionEnter(Collision collision)
    {
        // 检查碰撞物体是否是球体（根据 Tag 判断，球体的 Tag 需要设置为 "Ball"）
        if (collision.gameObject.CompareTag("Ball"))
        {
            // 播放声音
            if (collisionSound != null && audioSource != null)
            {
                audioSource.Play();
            }

            // 销毁球体
            Destroy(collision.gameObject);
        }
    }

    // 设置声音的方法
    public void SetCollisionSound(AudioClip newSound)
    {
        collisionSound = newSound;
        if (audioSource != null)
        {
            audioSource.clip = collisionSound;
        }
    }
}