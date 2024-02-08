using UnityEngine;

namespace Lyf.ObjectPool
{
    public class Example : MonoBehaviour
    {
        public GameObject[] prefabs;
        private void Update()
        {
            // 按A生成一个Cube
            if (Input.GetKeyDown(KeyCode.A))
            {
                var cube = ObjectPool.Instance.Allocate(prefabs[0], obj =>
                {
                    // Cube设置为左上角
                    obj.transform.position = new Vector3(-7, 3, 0);
                    obj.transform.SetParent(transform);
                });
            }
            // 按B生成一个Sphere
            if (Input.GetKeyDown(KeyCode.B))
            {
                var sphere = ObjectPool.Instance.Allocate(prefabs[1], obj =>
                {
                    // Sphere设置为右上角
                    obj.transform.position = new Vector3(7, 3, 0);
                });
            }
            // 按C生成一个Capsule
            if (Input.GetKeyDown(KeyCode.C))
            {
                var capsule = ObjectPool.Instance.Allocate(prefabs[2], obj =>
                {
                    // Capsule设置为中心
                    obj.transform.position = new Vector3(0, 0, 0);
                });
            }
            
            // 按D回收全部Cube(不包括已经激活)
            if (Input.GetKeyDown(KeyCode.D))
            {
                ObjectPool.Instance.ClearPool(prefabs[0]);
            }
            
            // 按E收回所有Sphere(包括已经激活)
            if (Input.GetKeyDown(KeyCode.E))
            {
                ObjectPool.Instance.ClearPool(prefabs[1], true);
            }
            
            // 按F回收全部对象(包括已经激活)
            if (Input.GetKeyDown(KeyCode.F))
            {
                ObjectPool.Instance.ClearAllPool(true);
            }
        }
    }
}