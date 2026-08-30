using System;
using System.Threading;
using _TevLib.ServiceLocatorSystem;
using _TevLib.ServiceLocatorSystem.PoolService;
using _TevLib.ServiceLocatorSystem.TimeService;
using DG.Tweening;
using UnityEngine;

namespace _01.Scripts.ItemSystem
{
    public abstract class Collectable : MonoBehaviour, IPoolable
    {
        [SerializeField] protected ItemSO itemData;
        [SerializeField] protected float dropDelay = 0.2f;
        [SerializeField] protected AnimationCurve dropCurve;
        public PoolItemSO Item => itemData.poolItemSO;
        public GameObject GameObject => gameObject;

        protected bool AlreadyCollected;
        protected Collider2D Collider;
        private CancellationTokenSource _cts;

        private bool _canCollectable;
        public bool CanCollectable => _canCollectable && !AlreadyCollected;

        protected virtual void Awake()
        {
            Collider = GetComponent<Collider2D>();
        }

        public void DropIt(Vector3 position)
        {
            KillTask();
            _cts = new CancellationTokenSource();
            transform.position = position;

            ServiceLocator.GetService<ITimeService>().ActionTimer(
                dropDelay,
                _cts.Token,
                null,
                () => _canCollectable = true
            );
        }

        public void SetItemData(ItemSO newItem)
        {
            itemData = newItem;
        }

        private void KillTask()
        {
            if (_cts == null) return;
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }

        public abstract void Collect(Transform collector, float magneticPower);

        public void ResetItem()
        {
            AlreadyCollected = false;
        }
    }
}