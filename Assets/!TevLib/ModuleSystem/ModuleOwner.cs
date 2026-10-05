using System;
using System.Collections.Generic;
using UnityEngine;
using ZLinq;

namespace _TevLib.ModuleSystem
{
    public abstract class ModuleOwner : MonoBehaviour
    {
        private Dictionary<Type,IModule> _moduleDict;
        public IReadOnlyDictionary<Type,IModule> ModuleDict => _moduleDict;

        public virtual void Awake()
        {
            _moduleDict = GetComponentsInChildren<IModule>().AsValueEnumerable().ToDictionary(m => m.GetType());
            
            InitializeModules();
            AfterInitializeModules();
        }

        protected virtual void InitializeModules()
        {
            foreach (IInitModule initModule in _moduleDict.Values.AsValueEnumerable().OfType<IInitModule>())
                initModule.Init(this);
        }

        protected virtual void AfterInitializeModules()
        {
            foreach (IAfterInitModule afterInitModule in _moduleDict.Values.AsValueEnumerable().OfType<IAfterInitModule>())
                afterInitModule.AfterInit();
        }

        public T GetModule<T>()
        {
            if (_moduleDict.TryGetValue(typeof(T), out IModule module))
                return (T) module;
            
            IModule findModule = _moduleDict.Values.AsValueEnumerable().FirstOrDefault(m => m is T);
            
            if (findModule is T casted)
                return casted;
            
            return default;
        }
    }
}