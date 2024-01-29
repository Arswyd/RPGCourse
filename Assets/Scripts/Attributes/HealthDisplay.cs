using System;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class HealthDisplay : MonoBehaviour
    {
        Health health;
        void Awake()
        {
            health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        }

        void Update()
        {
            GetComponent<Text>().text = String.Format("{0:0}/{1:0}", health.GetMaxHealth(), health.GetHealth());
        }
    }
}
