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
                var cube = ObjectPool.Instance.Allocate(prefabs[0]);
                cube.transform.position = Vector3.zero;
            }
            // 按B生成一个Sphere
            if (Input.GetKeyDown(KeyCode.B))
            {
                var sphere = ObjectPool.Instance.Allocate(prefabs[1]);
                sphere.transform.position = Vector3.zero;
            }
            // 按C生成一个Capsule
            if (Input.GetKeyDown(KeyCode.C))
            {
                var capsule = ObjectPool.Instance.Allocate(prefabs[2]);
                capsule.transform.position = Vector3.zero;
            }
            
            // 按D回收全部Cube
            if (Input.GetKeyDown(KeyCode.D))
            {
                ObjectPool.Instance.ClearPool(prefabs[0].name);
            }
            // 按E回收全部对象
            if (Input.GetKeyDown(KeyCode.E))
            {
                ObjectPool.Instance.ClearAllPool(true);
            }
        }
    }
}