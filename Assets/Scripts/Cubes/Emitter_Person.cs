using UnityEngine;
using System.Collections;

public class Emitter_Person : MonoBehaviour
{
    public GameObject personPrefab;
    public float shootForce = 10f; // 发射速度
    public float shootInterval = 1f; // 发射间隔

    private void Start()
    {
        // 启动协程
        StartCoroutine(ShootPersonsContinuously());
    }

    private IEnumerator ShootPersonsContinuously()
    {
        while (true)
        {
            ShootPerson();
            // 等待指定的发射间隔时间
            yield return new WaitForSeconds(shootInterval);
        }
    }

    private void ShootPerson()
    {
        // 直接使用Cube的transform.position作为发射位置（物体的中心）
        Vector3 shootPosition = transform.position;

        // 创建小人实例
        GameObject person = Instantiate(personPrefab, transform.position, Quaternion.identity);

        // 获取小人的动画组件
        Animator animator = person.GetComponent<Animator>();
        if (animator != null)
        {
            // 播放 walk 动画
            animator.Play("walk");
        }

        // 让小人朝向发射方向
        person.transform.forward = transform.forward;

        Rigidbody Rb = person.GetComponent<Rigidbody>();
        if (Rb != null)
        {
            Rb.AddForce(transform.forward * shootForce, ForceMode.Impulse);
        }
    }
}