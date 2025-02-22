using UnityEngine;

public class PersonChanger : MonoBehaviour
{
    public GameObject newPersonPrefab; // 新的小人预制件

    private void OnTriggerEnter(Collider other)
    {
        // 检查进入触发器的物体是否带有 Rigidbody 组件
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // 获取原小人的速度
            Vector3 originalVelocity = rb.velocity;

            // 获取原小人的位置和旋转
            Vector3 position = other.transform.position;
            Quaternion rotation = other.transform.rotation;

            // 销毁原小人
            Destroy(other.gameObject);

            // 实例化新的小人预制件
            GameObject newPerson = Instantiate(newPersonPrefab, position, rotation);

            // 获取新小人的 Rigidbody 组件
            Rigidbody newRb = newPerson.GetComponent<Rigidbody>();
            if (newRb != null)
            {
                // 给新小人设置原来的速度
                newRb.velocity = originalVelocity;
            }

            // 获取新小人的动画组件
            Animator animator = newPerson.GetComponent<Animator>();
            if (animator != null)
            {
                // 播放 walk 动画
                animator.Play("walk");
            }
            Physics.IgnoreCollision(newPerson.GetComponent<Collider>(), GetComponent<Collider>());
        }
    }
}