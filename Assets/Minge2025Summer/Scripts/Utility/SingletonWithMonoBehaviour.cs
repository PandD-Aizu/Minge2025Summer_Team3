using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Utility
{
    public abstract class SingletonWithMonoBehaviour<TYpe> : MonoBehaviour, IDisposable where TYpe : MonoBehaviour
    {
        private static TYpe instance;

        public static TYpe Instance
        {
            get
            {
                Assert.IsNotNull(instance, "There is no object attached " + typeof(TYpe).Name);
                return instance;
            }
        }

        private void Awake()
        {
            if (instance != null && instance.gameObject != null)
            {
                Destroy(this.gameObject);
                return;
            }

            instance = this as TYpe;
            DontDestroyOnLoad(this.gameObject);
            OnAwakeProcess();
        }

        public static bool IsExist()
        {
            return instance != null ? true : false;
        }

        private void OnDestroy()
        {
            if (instance != (this as TYpe)) return;
            OnDestroyProcess();
            Dispose();
        }

        public virtual void Dispose()
        {
            if (IsExist() == true)
                instance = null;
        }

        protected virtual void OnDestroyProcess()
        {
        }

        protected virtual void OnAwakeProcess()
        {
        }
    }
}