using System;
using System.Collections.Generic;
using _01.Scripts.GameSystem.Event;
using _01.Scripts.GameSystem.GameServices;
using _TevLib.CoreLib.EventSystem;
using _TevLib.ModuleSystem;
using _TevLib.ServiceLocatorSystem;
using UnityEngine;

namespace _01.Scripts.ItemSystem
{
    public class ItemCollector : MonoModule
    {
        [field: SerializeField] public float MagneticPower { get; private set; } = 20f;
        [field: SerializeField] public float Radius { get; private set; } = 2f;
        [field: SerializeField] public ContactFilter2D contactFilter;
        [field: SerializeField] public int MaxDetectCount {get; private set;} = 10;
        [SerializeField] private EventChannelSO eventChannel;
        private Collider2D[] _resultArray;
        private HashSet<Collider2D> _hashSet;

        public override void Init(ModuleOwner owner)
        {
            base.Init(owner);
            _resultArray = new Collider2D[MaxDetectCount];
            _hashSet = new HashSet<Collider2D>();
        }

        public void OnEnable()
            => eventChannel.AddListener<ExpCollectEvent>(HandleExpCollected);


        private void OnDisable()
            => eventChannel.RemoveListener<ExpCollectEvent>(HandleExpCollected);

        private void HandleExpCollected(ExpCollectEvent evt)
        {
            _hashSet.Remove(evt.Collider);
        }

        private void FixedUpdate()
        {
            int count = Physics2D.OverlapCircle(transform.position,Radius, contactFilter, _resultArray);
            for (int i = 0; i < count; i ++)
            {
                if (_hashSet.Contains(_resultArray[i])) continue;
                if (!_resultArray[i].TryGetComponent(out Collectable collectable)) continue;
                if (!collectable.CanCollectable) continue;
                collectable.Collect(transform,MagneticPower);
                _hashSet.Add(_resultArray[i]);
            }
        }
        
        #if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, Radius);
        }
        #endif
    }
}