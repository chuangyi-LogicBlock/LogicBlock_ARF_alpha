using UnityEngine;

public class Split_3 : MonoBehaviour
{
    public float shootForce = 10f;  // 发射力
    public float offsetDistance = 0.02f; // 新小球生成的偏移距离

    private void OnCollisionEnter(Collision collision)
    {
        // 确保只有球体与 Cube2 发生碰撞时才会分裂
        if (collision.gameObject.CompareTag("Ball"))
        {
            // 调用分裂功能
            SplitBall(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("Person"))
        {
            // 调用分裂功能
            SplitPerson(collision.gameObject);
        }
    }

    // 分裂球体的逻辑
    void SplitBall(GameObject originalBall)
    {
        // 获取原球体的属性
        Vector3 originalScale = originalBall.transform.localScale;
        Material originalMaterial = originalBall.GetComponent<Renderer>().material;

        // 获取 Cube2 的局部方向
        Vector3 cube2ZDirection = transform.forward;      // Cube2 的 Z 方向
        Vector3 cube2XDirection = transform.right;        // Cube2 的 X 方向
        Vector3 cube2NegXDirection = -transform.right;    // Cube2 的 X 负方向

        // 生成3个新的小球
        SpawnBall(originalBall, cube2ZDirection, originalScale, originalMaterial);
        SpawnBall(originalBall, cube2XDirection, originalScale, originalMaterial);
        SpawnBall(originalBall, cube2NegXDirection, originalScale, originalMaterial);

        // 销毁原本的球体
        Destroy(originalBall);
    }

    void SpawnBall(GameObject originalBall, Vector3 direction, Vector3 originalScale, Material originalMaterial)
    {
        // 计算新小球的生成位置，稍微偏移一点
        Vector3 spawnPosition = transform.position + direction * offsetDistance;
        GameObject newBall = Instantiate(originalBall, spawnPosition, Quaternion.identity);
        Rigidbody rb = newBall.GetComponent<Rigidbody>();
        rb.linearVelocity = direction * shootForce;

        // 保持新生成小球的属性一致
        newBall.transform.localScale = originalScale;
        newBall.GetComponent<Renderer>().material = originalMaterial;

        // 忽略新生成的小球与 Cube2 的碰撞
        Physics.IgnoreCollision(newBall.GetComponent<Collider>(), GetComponent<Collider>());
    }
    void SplitPerson(GameObject originalPerson)
    {
        // 获取原球体的属性
        Vector3 originalScale = originalPerson.transform.localScale;

        // 获取 Cube2 的局部方向
        Vector3 cube2ZDirection = transform.forward;      // Cube2 的 Z 方向
        Vector3 cube2XDirection = transform.right;        // Cube2 的 X 方向
        Vector3 cube2NegXDirection = -transform.right;    // Cube2 的 X 负方向

        // 生成3个新的小球
        SpawnPerson(originalPerson, cube2ZDirection, originalScale);
        SpawnPerson(originalPerson, cube2XDirection, originalScale);
        SpawnPerson(originalPerson, cube2NegXDirection, originalScale);

        // 销毁原本的球体
        Destroy(originalPerson);
    }

    void SpawnPerson(GameObject originalPerson, Vector3 direction, Vector3 originalScale)
    {
        // 计算新小球的生成位置，稍微偏移一点
        Vector3 spawnPosition = transform.position + direction * offsetDistance;
        GameObject newPerson = Instantiate(originalPerson, spawnPosition, Quaternion.identity);
        Rigidbody rb = newPerson.GetComponent<Rigidbody>();
        rb.linearVelocity = direction * shootForce;

        // 保持新生成小球的属性一致
        newPerson.transform.localScale = originalScale;
        newPerson.transform.forward = direction;

        // 忽略新生成的小球与 Cube2 的碰撞
        Physics.IgnoreCollision(newPerson.GetComponent<Collider>(), GetComponent<Collider>());
    }
}