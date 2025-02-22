using UnityEngine;

public class Split_6 : MonoBehaviour
{
    public float shootForce = 10f;  // 发射力
    public float offsetDistance = 0.02f; // 新小球生成的偏移距离

    private void OnCollisionEnter(Collision collision)
    {
        // 确保只有球体与Cube2发生碰撞时才会分裂
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

        // 获取Cube2的局部方向
        Vector3 cubeZDirection = transform.forward;  // Cube2的z方向

        // 计算绕z轴旋转的方向向量
        Quaternion rotate60Clockwise = Quaternion.Euler(0, 60, 0);
        Quaternion rotate120Clockwise = Quaternion.Euler(0, 120, 0);
        Quaternion rotate60CounterClockwise = Quaternion.Euler(0, -60, 0);
        Quaternion rotate120CounterClockwise = Quaternion.Euler(0, -120, 0);

        Vector3 directionRotate60Clockwise = rotate60Clockwise * cubeZDirection;
        Vector3 directionRotate120Clockwise = rotate120Clockwise * cubeZDirection;
        Vector3 directionRotate60CounterClockwise = rotate60CounterClockwise * cubeZDirection;
        Vector3 directionRotate120CounterClockwise = rotate120CounterClockwise * cubeZDirection;

        // 生成5个小球，分别按不同的方向发射
        Vector3[] directions = new Vector3[]
        {
            cubeZDirection,
            directionRotate60Clockwise,
            directionRotate120Clockwise,
            directionRotate60CounterClockwise,
            directionRotate120CounterClockwise
        };

        foreach (Vector3 direction in directions)
        {
            SpawnBall(originalBall, direction, originalScale, originalMaterial);
        }

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

        // 忽略新生成的小球与Cube2的碰撞
        Physics.IgnoreCollision(newBall.GetComponent<Collider>(), GetComponent<Collider>());
    }
    void SplitPerson(GameObject originalPerson)
    {
        // 获取原球体的属性
        Vector3 originalScale = originalPerson.transform.localScale;

        // 获取Cube2的局部方向
        Vector3 cubeZDirection = transform.forward;  // Cube2的z方向

        // 计算绕z轴旋转的方向向量
        Quaternion rotate60Clockwise = Quaternion.Euler(0, 60, 0);
        Quaternion rotate120Clockwise = Quaternion.Euler(0, 120, 0);
        Quaternion rotate60CounterClockwise = Quaternion.Euler(0, -60, 0);
        Quaternion rotate120CounterClockwise = Quaternion.Euler(0, -120, 0);

        Vector3 directionRotate60Clockwise = rotate60Clockwise * cubeZDirection;
        Vector3 directionRotate120Clockwise = rotate120Clockwise * cubeZDirection;
        Vector3 directionRotate60CounterClockwise = rotate60CounterClockwise * cubeZDirection;
        Vector3 directionRotate120CounterClockwise = rotate120CounterClockwise * cubeZDirection;

        // 生成5个小球，分别按不同的方向发射
        Vector3[] directions = new Vector3[]
        {
            cubeZDirection,
            directionRotate60Clockwise,
            directionRotate120Clockwise,
            directionRotate60CounterClockwise,
            directionRotate120CounterClockwise
        };

        foreach (Vector3 direction in directions)
        {
            SpawnPerson(originalPerson, direction, originalScale);
        }

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