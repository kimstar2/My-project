using System;
using _TevLib.ServiceLocatorSystem.PoolService;
using UnityEngine;

namespace _01.Scripts.ItemSystem
{
    public abstract class Collectable : MonoBehaviour , IPoolable
    {
        [SerializeField] protected ItemSO itemSo;
        [SerializeField] protected float dropDelay = 0.2f;
        public PoolItemSO Item => itemSo.poolItemSO;
        public GameObject GameObject => gameObject;
        
        protected bool AlreadyCollected;
        protected Collider2D collider;

        public bool CanCollectable => !AlreadyCollected;

        protected virtual void Awake()
        {
            collider = GetComponent<Collider2D>();
        }
        
        // ;


        public void ResetItem()
        {
            throw new System.NotImplementedException();
        }
    }
}