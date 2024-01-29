using RPG.Attributes;
using UnityEngine;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float projectileSpeed = 10f;
        [SerializeField] bool isHoming = false;
        [SerializeField] GameObject hitVFX = null;
        [SerializeField] float maxLifeTime = 10f;
        [SerializeField] float lifeAfterImpact = 0.2f;
        [SerializeField] GameObject[] destroyOnHit = null;
        Health target = null;
        GameObject instigator = null;
        float damage = 0f;

        void Start()
        {
            gameObject.transform.LookAt(GetAimLocation());
        }

        void Update()
        {
            if (target == null) { return; }

            if(isHoming && !target.IsDead()) 
            { 
                gameObject.transform.LookAt(GetAimLocation());
            }
            gameObject.transform.Translate(Vector3.forward * projectileSpeed * Time.deltaTime);
        }

        public void SetTarget(Health target, GameObject instigator, float damage)
        {
            this.target = target;
            this.damage = damage;
            this.instigator = instigator;

            Destroy(gameObject, maxLifeTime);
        }

        Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();

            if (targetCapsule == null) 
            { 
                return target.transform.position; 
            }

            return target.transform.position + Vector3.up * targetCapsule.height / 1.5f ;
        }

        void OnTriggerEnter(Collider collider)
        {
            if (collider.GetComponent<Health>() != target) { return; };
            if (target.IsDead()) { return; };

            target.TakeDamage(instigator, damage);

            projectileSpeed = 0;

            if (hitVFX != null)
            {
                Instantiate(hitVFX, GetAimLocation(), transform.rotation);
            }

            foreach(GameObject toDestroy in destroyOnHit)
            {
                Destroy(toDestroy);
            }

            Destroy(gameObject, lifeAfterImpact);
        }
    }
}
