using System.Collections;
using System.Collections.Generic;
using GameDevTV.Utils;
using Newtonsoft.Json.Linq;
using RPG.Attributes;
using RPG.Core;
using RPG.Movement;
using RPG.Saving;
using RPG.Stats;
using Unity.VisualScripting;
using UnityEngine;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        [SerializeField] float timeBetweenAttacks = 1.5f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] Weapon defaultWeapon = null;
        Health target;
        Mover mover;
        ActionScheduler actionScheduler;
        Animator animator;
        float timeSinceLastAttack = Mathf.Infinity;
        LazyValue<Weapon> currentWeapon;

        void Awake()
        {
            mover = GetComponent<Mover>();
            actionScheduler = GetComponent<ActionScheduler>();
            animator = GetComponent<Animator>();
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
        }

        Weapon SetupDefaultWeapon()
        {
            AttachWeapon(defaultWeapon);
            return defaultWeapon;
        }

        void Start()
        {
            currentWeapon.ForceInit();
        }

        void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            if (target == null) { return; }
            if (target.IsDead()) { return; }

            if (!GetIsInRange(target.transform))
            {
                mover.MoveToDestination(target.transform.position, 1f);
            }
            else
            {
                mover.Cancel();              
                AttackBehavior();
            }
        }

        void AttackBehavior()
        {
            transform.LookAt(target.transform);
            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                // This will trigger the Hit() event
                TriggerAttack();
                timeSinceLastAttack = 0f;
            }
        }

        private void TriggerAttack()
        {
            animator.ResetTrigger("stopAttack");
            animator.SetTrigger("attack");
        }

        // Animation evenet
        void Hit()
        {
            if(target == null) { return; }

            float calculatedDamage = GetComponent<BaseStats>().GetStat(Stat.Damage);

            if(currentWeapon.value.HasProjectile())
            {
                currentWeapon.value.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, calculatedDamage);
            }
            else
            {
                target.TakeDamage(gameObject, calculatedDamage);
            }
        }

        // Animation evenet
        void Shoot()
        {
            Hit();
        }

        bool GetIsInRange(Transform targetTrasform)
        {
            return Vector3.Distance(transform.position, targetTrasform.position) < currentWeapon.value.GetRange();
        }

        public void Attack(GameObject combatTarget)
        {
            actionScheduler.StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            StopAttack();
            target = null;
        }

        private void StopAttack()
        {
            animator.ResetTrigger("attack");
            animator.SetTrigger("stopAttack");
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if(stat == Stat.Damage)
            {
                yield return currentWeapon.value.GetDamage();
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if(stat == Stat.Damage)
            {
                yield return currentWeapon.value.GetPercentageBonus();
            }
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if(combatTarget == null) { return false; }
            if(!mover.CanMoveTo(combatTarget.transform.position) && !GetIsInRange(combatTarget.transform)) { return false; }

            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        public void EquipWeapon(Weapon weapon)
        {
            currentWeapon.value = weapon;
            AttachWeapon(weapon);
        }

        private void AttachWeapon(Weapon weapon)
        {
            Animator animator = GetComponent<Animator>();
            weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(currentWeapon.value.name);
        }

        public void RestoreFromJToken(JToken state)
        {
            string weaponName = state.ToObject<string>();
            Weapon weapon = Resources.Load<Weapon>(weaponName);
            EquipWeapon(weapon);
        }
    }
}
