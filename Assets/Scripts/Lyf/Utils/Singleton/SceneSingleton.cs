using System;
using UnityEngine;

namespace Lyf.Utils.Singleton
{
    /// <summary>
    /// 场景内单例, 切换场景时会被销毁
    /// </summary>
    public class SceneSingleton<T> : MonoBehaviour, ISingleton, IDisposable where T : SceneSingleton<T>
    {
        private static readonly object Lock = typeof(T);
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    // 双重检查锁
                    lock (Lock)
                    {
                        if (_instance == null)
                        {
                            _instance = FindObjectOfType<T>();  // 从场景中寻找一个T类型的组件
                            if (_instance == null)
                            {
                                throw new Exception($"Can not find {typeof(T)} in scene");
                            }
                        }
                    }// lock
                    _instance.Awake();  // 如果在Awake前被访问 则 Awake将在此处调用一次
                }
                return _instance;
            }
        }

        protected void Awake()
        {
            // 如果Awake前没有被访问 那么就会在Awake中初始化
            if (_instance == null)
            {
                _instance = this as T;
            }
            else if (_instance != this)
            {
                // 如果已经被访问过了 代表已经有一个对应的单例对象存在了 那么就会在Awake中销毁自己
                Destroy(gameObject);
            }
        }
        
        protected void OnEnable()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
        }

        protected void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        public void Dispose()   // Dispose()方法用于释放资源
        {
            if (_instance == this)
            {
                _instance = null;
                Destroy(gameObject);
            }
        }
    } // class SceneSingleton
} // namespace Utils.Singleton