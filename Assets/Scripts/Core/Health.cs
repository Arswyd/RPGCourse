using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPG.Saving;
using UnityEngine;

namespace RPG.Core
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float health = 100f;

        bool isDead = false;

        public bool IsDead()
        {
            return isDead;
        }

        public void TakeDamage(float damage)
        {
            if(isDead) { return; }

            health = Mathf.Max(health - damage, 0);
            if(health == 0)
            {
                Die();
            }
        }

        void Die()
        {
            isDead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        public JToken CaptureAsJToken()
        {
            return JToken.FromObject(health);
        }

        public void RestoreFromJToken(JToken state)
        {
            health = state.ToObject<float>();
            
            if(health <= 0)
            {
                Die();
            }
        }
    }
}
