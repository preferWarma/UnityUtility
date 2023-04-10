using System.Collections.Generic;
using Lyf.Utils.Singleton;
using UnityEngine;

namespace Lyf.ObjectPool
{
    public class ObjectPool : Singleton<ObjectPool>
    {
        private readonly Dictionary<string, Queue<GameObject>> _objectPools = new(); // key: prefab 名字, value: prefabs对应的对象池
        private readonly Dictionary<string, int> _poolMaxCountDic = new(); // key: prefab 名字, value: prefabs对应的对象池的当前最大数量
        private GameObject _parentPool;   // 所有对象池的父物体

        /// <summary>
        /// 从对象池拿出对象
        /// </summary>
        /// <returns></returns>
        public GameObject Allocate(GameObject prefab)
        {
            // 如果该对象池不存在或者该对象池中没有可以分配的对象了
            if (!_objectPools.ContainsKey(prefab.name) || _objectPools[prefab.name].Count == 0)
            {
                if (!_parentPool)
                {
                    _parentPool = new GameObject("ParentPool");   // 创建一个对象池的父物体
                }
                var childPool = _parentPool.transform.Find(prefab.name + "Pool");   // 查找父对象池中是否有对应的子物体对象池
                if (!childPool)
                {
                    childPool = new GameObject(prefab.name + "Pool").transform;   // 创建该对象池的子物体对象池
                    childPool.SetParent(_parentPool.transform);   // 将该对象池的子物体对象池设置为对象池的父物体
                }
                FillPool(prefab);   // 填充该对象对应的对象池
            }
            var res = _objectPools[prefab.name].Dequeue();   // 从对象池中取出一个对象
            res.SetActive(true);
            return res;
        }

        /// <summary>
        /// 将对象回收到对象池
        /// </summary>
        public void Recycle(GameObject prefab)
        {
            var name = prefab.name.Replace("(Clone)", "");   // 去掉对象名字中的(Clone)
            if (!_objectPools.ContainsKey(name))
            {
                _objectPools.Add(name, new Queue<GameObject>());   // 如果该对象池不存在则创建一个新的对象池
            }
            _objectPools[name].Enqueue(prefab);   // 将对象放入对象池
            prefab.SetActive(false);
        }
        
        private void FillPool(GameObject prefab) // 填充该对象对应的对象池, 采用二倍填充
        {
            var parent = _parentPool.transform.Find(prefab.name + "Pool");
            // 如果该对象对应的对象池不存在则创建一个新的对象池与之对应
            if (!_objectPools.ContainsKey(prefab.name))
            {
                _objectPools.Add(prefab.name, new Queue<GameObject>());
                // 初始容量设置为 10
                for (var i = 0; i < 10; i++)
                {
                    var obj = Object.Instantiate(prefab, parent, true);
                    Recycle(obj);
                }
                _poolMaxCountDic.Add(prefab.name, 10);
                Debug.Log($"已创建{prefab.name}Pool对象池, 初始容量设置为 10");
            }

            else // 存在对象池则二倍扩容
            {
                if (_poolMaxCountDic[prefab.name] == 0) // 如果之前清空了该对象池,则首次填充的时候,容量设置为 10
                {
                    _poolMaxCountDic[prefab.name] = 10;
                    for (var i = 0; i < _poolMaxCountDic[prefab.name]; i++)
                    {
                        var obj = Object.Instantiate(prefab, parent, true);
                        Recycle(obj);
                    }
                    Debug.Log($"已重新初始化{prefab.name}Pool对象池, 当前容量为 10");
                }
                else
                {
                    for (var i = 0; i < _poolMaxCountDic[prefab.name]; i++)
                    {
                        var obj = Object.Instantiate(prefab, parent, true);
                        Recycle(obj);
                    }
                    _poolMaxCountDic[prefab.name] *= 2;
                    Debug.Log($"已扩容{prefab.name}Pool对象池, 当前容量为 {_poolMaxCountDic[prefab.name]}");
                }
            }

        }
        
        /// <summary>
        /// 清空指定对象池中的对象
        /// </summary>
        /// <param name="prefabName"> 对象名 </param>
        /// <param name="containActive"> 是否清空当前已经激活的对象 </param>
        public void ClearPool(string prefabName, bool containActive = false)
        {
                if (_objectPools.ContainsKey(prefabName))
                {
                    var childPool = _parentPool.transform.Find(prefabName + "Pool");
                    for (var i = 0; i < childPool.childCount; i++)
                    {
                        var child = childPool.GetChild(i);
                        if (!child.gameObject.activeSelf)
                        {
                            Object.Destroy(child.gameObject);
                            _poolMaxCountDic[prefabName]--;
                        }
                        else if (containActive)
                        {
                            Object.Destroy(child.gameObject);
                            _poolMaxCountDic[prefabName]--;
                        }
                    }

                    _objectPools[prefabName].Clear();
                    Debug.Log(containActive ? $"已清空对象池 {prefabName}Pool中的全部对象" : $"已清空对象池 {prefabName}Pool中未激活的全部对象");
                }
            
        }
        
        /// <summary>
        /// 清空所有对象池中的对象
        /// </summary>
        /// <param name="containActive"> 是否清空当前已经激活的对象 </param>
        public void ClearAllPool(bool containActive = false)
        {
            foreach (var pool in _objectPools)
            {
                ClearPool(pool.Key, containActive);
            }

            Debug.Log(containActive ? "已清空所有对象池中的全部对象" : "已清空所有对象池中未激活的全部对象");
        }
    }
}