using UnityEngine;
using System.Collections;

public class Emitter_Ball : MonoBehaviour
{
    public GameObject ballPrefab;
    public float shootForce = 10f; // 发射速度
    public float shootInterval = 1f; // 发射间隔

    private void Start()
    {
        // 启动协程
        StartCoroutine(ShootBallsContinuously());
    }

    private IEnumerator ShootBallsContinuously()
    {
        while (true)
        {
            ShootBall();
            // 等待指定的发射间隔时间
            yield return new WaitForSeconds(shootInterval);
        }
    }

    private void ShootBall()
    {
        // 直接使用Cube的transform.position作为发射位置（物体的中心）
        Vector3 shootPosition = transform.position;

        // 创建小球并设置发射点
        GameObject ball = Instantiate(ballPrefab, shootPosition, Quaternion.identity);
        Rigidbody ballRb = ball.GetComponent<Rigidbody>();
        if (ballRb != null)
        {
            // 通过物理力发射小球
            ballRb.AddForce(transform.forward * shootForce, ForceMode.Impulse);
        }
    }
}