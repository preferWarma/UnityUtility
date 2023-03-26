using UnityEngine;

namespace Utils
{
    // 泛型单例
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        public static T Instance { get; private set; }
        public static bool IsInitialized => Instance != null;  // 属性：是否被生成
        protected virtual void Awake()
        {
            if (Instance != null)
                Destroy(gameObject);
            else
                Instance = (T)this;
        }

        protected virtual void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }// class Singleton<T>
    
}// namespace Utils