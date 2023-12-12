using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class PersistentObjectSpawner : MonoBehaviour
    {
        [SerializeField] GameObject persistentObjectPrefab;

        static bool hasSpawned = false;

        private void Awake()
        {
            if(hasSpawned) return;

            SpawnPersistenetObjects();

            hasSpawned = true;
        }

        private void SpawnPersistenetObjects()
        {
            DontDestroyOnLoad(Instantiate(persistentObjectPrefab));
        }
    }   
}
