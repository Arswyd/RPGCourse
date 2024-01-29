using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
        Experience experience;
        void Awake()
        {
            experience = GameObject.FindGameObjectWithTag("Player").GetComponent<Experience>();
        }

        void Update()
        {
            GetComponent<Text>().text = experience.GetExperiencePoints().ToString();
        }
    }
}
