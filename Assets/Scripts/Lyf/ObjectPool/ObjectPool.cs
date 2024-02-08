using System;
using System.Collections.Generic;
using Lyf.Utils.Singleton;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Lyf.ObjectPool
{
    public class ObjectPool : Singleton<ObjectPool>
    {
        private static int _initialPoolCount = 10; // 初始对象池大小
        private static int _addCount = 5; // 每次增加的对象数量

        private readonly Dictionary<string, ObjectPoolData> _objectPoolDataDic = new();
        private readonly GameObject _parentPool = new("ParentObjectPool");   // 所有对象池的父物体

        public void SetInitialPoolCount(int count) => _initialPoolCount = count; // 设置初始对象池大小
        public void SetAddCount(int count) => _addCount = count; // 设置每次增加的对象数量

        /// <summary>
        /// 从对象池取出一个对象
        /// </summary>
        public GameObject Allocate(GameObject prefab, Action<GameObject> callback = null)
        {
            var prefabName = prefab.name;

            // 如果对象池中没有该对象对应的根节点, 则初始化一个
            if (!_objectPoolDataDic.TryGetValue(prefabName, out var poolData))
            {
                poolData = InitializePool(prefab);
            }
            
            // 如果对象池中没有足够的对象, 则扩容
            if (poolData.AvailableObjects.Count == 0)
            {
                ExpandPool(prefab);
            }

            var obj = poolData.AvailableObjects.Dequeue();
            obj.SetActive(true);

            callback?.Invoke(obj);   // 回调函数
            return obj;
        }

        /// <summary>
        /// 将对象回收到对象池
        /// </summary>
        public void Recycle(GameObject prefab)
        {
            var prefabName = prefab.name;

            if (!_objectPoolDataDic.TryGetValue(prefabName, out var poolData))
            {
                Debug.LogError($"未找到 {prefabName} 的对象池数据");
                return;
            }

            poolData.AvailableObjects.Enqueue(prefab);
            prefab.SetActive(false);
        }
        
        public void ClearPool(string prefabName, bool containActive = false)    // 清空对象池
        {
            if (_objectPoolDataDic.TryGetValue(prefabName, out var poolData))
            {
                for (var i = poolData.AllObjects.Count - 1; i >= 0; i--)
                {
                    var obj = poolData.AllObjects[i];
                    if (obj.activeSelf && !containActive) continue; // 如果不包含激活的对象, 则跳过
                    Object.Destroy(obj);
                    poolData.AllObjects.RemoveAt(i);
                }

                // 清空对象池数据, 避免空引用异常
                poolData.AvailableObjects.Clear();

                Debug.Log(containActive ? $"已清空对象池 {prefabName}Pool 中的全部对象" : $"已清空对象池 {prefabName}Pool 中未激活的全部对象");
            }
        }
        
        public void ClearPool(GameObject prefab, bool containActive = false) => ClearPool(prefab.name, containActive);    // 重载方法

        public void ClearAllPool(bool containActive = false)
        {
            var prefabNames = new List<string>(_objectPoolDataDic.Keys);
            foreach (var prefabName in prefabNames)
            {
                ClearPool(prefabName, containActive);
            }
        }
        
        public void ExpandPool(GameObject prefab)  // 扩容对象池
        {
            var prefabName = prefab.name;
            if (!_objectPoolDataDic.TryGetValue(prefabName, out var poolData))
            {
                Debug.LogError($"未找到 {prefabName} 的对象池数据");
                return;
            }

            var parent = _parentPool.transform.Find(prefabName + "Pool");

            for (var i = 0; i < _addCount; i++)
            {
                var obj = Object.Instantiate(prefab, parent, true);
                obj.name = obj.name.Replace("(Clone)", string.Empty);
                poolData.AddObject(obj);
                obj.SetActive(false);
            }

            Debug.LogFormat("已扩容 {0} 对象池，当前容量为 {1}", prefabName, poolData.AllObjects.Count);
        }
        
        // 其他方法...(以后想到了再说)

        private ObjectPoolData InitializePool(GameObject prefab)  // 创建并初始化一个不存在的对象池
        {
            var prefabName = prefab.name.Replace("(Clone)", string.Empty);
            var rootObj = new GameObject(prefabName + "Pool");  // 创建对象池的父物体
            rootObj.transform.SetParent(_parentPool.transform);
            
            var poolData = new ObjectPoolData();
            _objectPoolDataDic[prefabName] = poolData;

            for (var i = 0; i < _initialPoolCount; i++)
            {
                var obj = Object.Instantiate(prefab, rootObj.transform, true);
                obj.name = obj.name.Replace("(Clone)", string.Empty);
                poolData.AddObject(obj);
                obj.SetActive(false);
            }

            Debug.Log($"已初始化 {prefabName} 对象池，初始容量为 {_initialPoolCount}");
            return poolData;
        }
        
        private class ObjectPoolData
        {
            public Queue<GameObject> AvailableObjects { get; } = new();
            public List<GameObject> AllObjects { get; } = new();
            
            public void AddObject(GameObject obj)
            {
                AllObjects.Add(obj);
                AvailableObjects.Enqueue(obj);
            }
        }
    }
}
