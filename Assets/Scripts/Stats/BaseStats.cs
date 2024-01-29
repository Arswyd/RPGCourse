using System;
using System.Linq.Expressions;
using GameDevTV.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1,99)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression;
        [SerializeField] GameObject levelUpVFX;
        [SerializeField] bool shouldUseModifiers = false;
        LazyValue<int> currentLevel;
        Experience experience;
        public event Action onLevelUp;

        void Awake()
        {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }

        void Start()
        {
            currentLevel.ForceInit();
        }

        void OnEnable()
        {
            if(experience != null)
            {
                experience.onExperienceGained += UpdateLevel; 
            }       
        }

        void OnDisable()
        {
            if(experience != null)
            {
                experience.onExperienceGained -= UpdateLevel; 
            }          
        }

        void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if(newLevel > currentLevel.value)
            {
                currentLevel.value = newLevel;
                LevelUpEffect();
                onLevelUp();
            }
        }

        public float GetStat(Stat stat)
        {
            return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * 1 + GetPercentageModifier(stat)/100;
        }

        float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }

        float GetAdditiveModifier(Stat stat)
        {
            if(!shouldUseModifiers) { return 0; }

            float total = 0;
            foreach(IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach(float modifier in provider.GetAdditiveModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        float GetPercentageModifier(Stat stat)
        {
            if(!shouldUseModifiers) { return 0; }
            
            float total = 0;
            foreach(IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach(float modifier in provider.GetPercentageModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        public int GetLevel()
        {
            return currentLevel.value;
        }

        int CalculateLevel()
        {
            if(experience == null) { return startingLevel; }

            float currentXP = experience.GetExperiencePoints();
            int penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass); // maxLevel - 1 
            for (int level = 1; level <= penultimateLevel; level++)
            {
                float XPToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);
                if(XPToLevelUp > currentXP)
                {
                    return level;
                }
            }

            return penultimateLevel + 1;
        }

        void LevelUpEffect()
        {
            Instantiate(levelUpVFX, transform);
        }
    }
}
