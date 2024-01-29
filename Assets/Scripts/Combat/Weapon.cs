using RPG.Attributes;
using UnityEngine;

namespace RPG.Combat
{

    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject 
    {
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] GameObject weaponPrefab = null;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float weaponDamage = 20f;
        [SerializeField] float percentageBonus = 0;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] Projectile projectile = null;

        const string weaponName = "Weapon";

        public void Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);

            if(weaponPrefab != null)
            {
                Transform handTransform = isRightHanded ? rightHand : leftHand;
                GameObject weapon = Instantiate(weaponPrefab, handTransform);
                weapon.name = weaponName;
            }

            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if(animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
            else if(overrideController != null)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }
        }

        void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(weaponName);

            if(oldWeapon == null)
            {
                oldWeapon = leftHand.Find(weaponName);
            }
            if(oldWeapon == null) { return; }

            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calculatedDamage)
        {
            Transform handTransform = isRightHanded ? rightHand : leftHand;
            Projectile projectileInstance = Instantiate(projectile, handTransform.position, Quaternion.identity);
            projectileInstance.SetTarget(target, instigator, calculatedDamage);
        }

        public float GetRange()
        {
            return weaponRange;
        }

        public float GetDamage()
        {
            return weaponDamage;
        }

        public float GetPercentageBonus()
        {
            return percentageBonus;
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }
    }
}