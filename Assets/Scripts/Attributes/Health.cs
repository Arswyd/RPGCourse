using GameDevTV.Utils;
using Newtonsoft.Json.Linq;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] UnityEvent<float> takeDamage;
        LazyValue<float> healthPoints;
        float maxHealthPoints = -1f;

        bool isDead = false;
        BaseStats baseStats;

        void Awake()
        {
            baseStats = GetComponent<BaseStats>();
            healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        float GetInitialHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        void Start()
        {
            healthPoints.ForceInit();
            maxHealthPoints = healthPoints.value;
        }

        void OnEnable()
        {
            baseStats.onLevelUp += RegenerateHealth;
        }

         void OnDisable()
        {
            baseStats.onLevelUp -= RegenerateHealth;
        }       

        public float GetHealth()
        {
            return healthPoints.value;
        }
        public float GetMaxHealth()
        {
            return maxHealthPoints;
        }

        public float GetFraction()
        {
            return healthPoints.value / maxHealthPoints;
        }

        public bool IsDead()
        {
            return isDead;
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            if(isDead) { return; }

            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);
            takeDamage.Invoke(damage);
            if(healthPoints.value == 0)
            {
                Die();
                AwardExperience(instigator);
            }
        }

        void RegenerateHealth()
        {
            maxHealthPoints = baseStats.GetStat(Stat.Health);
            healthPoints.value = maxHealthPoints;
        }

        void Die()
        {
            isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();

            if(experience == null) { return; }

            experience.GainExperience(baseStats.GetStat(Stat.ExperienceReward));
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(healthPoints.value);
        }

        public void RestoreFromJToken(JToken state)
        {
            maxHealthPoints = baseStats.GetStat(Stat.Health);
            healthPoints.value = state.ToObject<float>();
            
            if(healthPoints.value <= 0)
            {
                Die();
            }
        }
    }
}
