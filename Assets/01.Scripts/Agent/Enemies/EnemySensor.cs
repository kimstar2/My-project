using System.Linq;
using _TevLib.ModuleSystem;
using UnityEngine;

namespace _01.Scripts.Agent.Enemies
{
    public class EnemySensor : MonoModule
    {
        [SerializeField] private ContactFilter2D filter;
        [SerializeField] private int maxResults;
        [SerializeField] private LayerMask obstacleMask;
        
        public Collider2D[] ColliderResults {get; private set;}

        public override void Init(ModuleOwner owner)
        {
            base.Init(owner);
            ColliderResults = new Collider2D[maxResults];
        }

        public GameObject IsTargetInRadius(float radius)
        {
            int count = GetAllTargetsInRadius(radius);
            return count > 0 ? ColliderResults.First().gameObject : null;
        }

        public GameObject GetClosestTarget(float radius)
        {
            int count = GetAllTargetsInRadius(radius);
            
            if (count == 0) return null;
            
            GameObject closest = null;
            float minSqrDistance = float.MaxValue;
            Vector2 origin = transform.position;

            for (int i = 0; i < count; i++)
            {
                float sqrDistance = ((Vector2)ColliderResults[i].transform.position - origin).sqrMagnitude;
                if (sqrDistance < minSqrDistance)
                {
                    minSqrDistance = sqrDistance;
                    closest = ColliderResults[i].gameObject;
                }
            }
            
            return closest;
        }

        public int GetAllTargetsInRadius(float radius)
        {
            return Physics2D.OverlapCircle(transform.position,radius,filter,ColliderResults);
        }

        public bool TryDetectTarget(float radius, out GameObject target)
        {
            int count = GetAllTargetsInRadius(radius);
            float minSqrDistance = float.MaxValue;
            target = null;
            
            for (int i = 0; i < count; i++)
            {
                Debug.Log(ColliderResults[i].gameObject.name);
                GameObject candidate = ColliderResults[i].gameObject;
                
                float distance = Vector2.Distance(candidate.transform.position, transform.position);

                if (minSqrDistance > distance * distance)
                {
                    minSqrDistance = distance;
                    target = candidate;
                }
            }
            if (target != null)
                return true;
            
            return false;
        }
    }
}