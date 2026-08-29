using System;
using System.Collections.Generic;
using UnityEngine;

namespace _TevLib.ServiceLocatorSystem
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type,object> Services = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void InitializeServiceLocator()
            => Services.Clear();

        public static void RegisterService<T>(T service)
        {
            Services[typeof(T)] = service;
            Debug.Log($"[ServiceLocator] Register - {typeof(T).Name} 등록됨 : {service.GetType().Name}");
        }

        public static void UnregisterService<T>()
        {
            Services.Remove(typeof(T));
            Debug.Log($"[ServiceLocator] Unregister - {typeof(T).Name}]");
        }

        public static T GetService<T>()
        {
            if (Services.TryGetValue(typeof(T), out object service))
                return (T)service;
            
            Debug.LogWarning($"[Service Locator] {typeof(T).Name} 이 등록되지 않음");
            return default;
        }
    }
}