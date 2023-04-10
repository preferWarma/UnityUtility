using System;

namespace Lyf.Utils.Singleton
{
    /// <summary>
    /// 通用单例, 全局存在
    /// </summary>
    public abstract class Singleton<T> : ISingleton, IDisposable where T : Singleton<T>, new()   // new()约束表示T必须有一个无参构造函数
    {
        private static readonly object Lock = typeof(T);
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)  // 双重检查锁
                {
                    lock (Lock)
                    {
                        _instance ??= new T();  // ??= 表示如果_instance为null, 则赋值为new T()
                    }
                }
                return _instance ??= new T();
            }
        }

        public void Dispose()
        {
            _instance = null;
        }
    } // class Singleton<T>
} // namespace Utils.Singleton