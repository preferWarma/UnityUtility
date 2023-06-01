using System;
using UnityEngine;

namespace Lyf.Utils.Singleton
{
    /// <summary>
    /// 全局单例, 一直存在, 切换场景时不会被销毁, 且保留第一个创建的实例
    /// </summary>
    public class GlobalSingleton<T> : MonoBehaviour, ISingleton, IDisposable where T : GlobalSingleton<T>
    {
        private static readonly object Lock = new();
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (Lock)
                    {
                        if (_instance == null)
                        {
                            _instance = FindObjectOfType<T>();
                            if (_instance == null)
                            {
                                Debug.LogWarning($"Cannot find {typeof(T)} in scene, creating a new one.");
                                GameObject obj = new GameObject(typeof(T).Name);
                                _instance = obj.AddComponent<T>();
                            }
                            DontDestroyOnLoad(_instance.gameObject);
                        }
                    }
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        public virtual void Dispose()
        {
            if (_instance == this)
            {
                _instance = null;
                Destroy(gameObject);
            }
        }
    }

}