using System;
using UnityEngine;

namespace Lyf.Utils.Singleton
{
    /// <summary>
    /// 全局单例, 一直存在, 切换场景时不会被销毁
    /// </summary>
    public class GlobalSingleton<T> : MonoBehaviour, ISingleton, IDisposable where T : GlobalSingleton<T>
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
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);    //如果已经被访问过了 代表已经有一个对应的单例对象存在了 那么就会在Awake中销毁自己
            }
        }
        protected void OnEnable()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
        }
        
        protected void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
        
        public void Dispose()
        {
            if (_instance == this)
            {
                _instance = null;
                Destroy(gameObject);
            }
        }
    }
}