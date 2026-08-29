using System;
using UnityEngine;

namespace _TevLib.CoreLib
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindFirstObjectByType<T>();
                if (_instance is null)
                {
                    string objectName = typeof(T).ToString();
                    GameObject instanceGo = new GameObject(objectName);
                    _instance = instanceGo.AddComponent<T>();
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            T[] manager = FindObjectsByType<T>(FindObjectsSortMode.None);
            if (manager.Length > 1)
                Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }
    }
}