using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace RPG.Saving
{
    [ExecuteAlways]
    public class SaveableEntity : MonoBehaviour
    {
        [SerializeField] string uniqueIdentifier = "";
        static Dictionary<string, SaveableEntity> globalLookup = new Dictionary<string, SaveableEntity>();

        public string GetUniqueIdentifier()
        {
            return uniqueIdentifier;
        }

        public JToken CaptureAsJtoken()
        {
            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state; //maps a Dictionary<string, JToken> right onto the JObject. Both state and stateDict point to the same object.
            foreach (ISaveable jsonSaveable in GetComponents<ISaveable>())
            {
                JToken token = jsonSaveable.CaptureAsJToken();
                string component = jsonSaveable.GetType().ToString();
                Debug.Log($"{name} Capture {component} = {token.ToString()}");
                stateDict[jsonSaveable.GetType().ToString()] = token;
            }
            return state;
        }



        public void RestoreFromJToken(JToken token) 
        {
            JObject state = token.ToObject<JObject>();
            IDictionary<string, JToken> stateDict = state;
            foreach (ISaveable jsonSaveable in GetComponents<ISaveable>())
            {
                string component = jsonSaveable.GetType().ToString();
                if (stateDict.ContainsKey(component))
                {
                    Debug.Log($"{name} Restore {component} =>{stateDict[component].ToString()}");
                    jsonSaveable.RestoreFromJToken(stateDict[component]);
                }
            }
        }


#if UNITY_EDITOR
        void Update()
        {
            if(Application.IsPlaying(gameObject)) { return; }
            if(string.IsNullOrEmpty(gameObject.scene.path)) { return; }

            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty property =  serializedObject.FindProperty("uniqueIdentifier");

            if(string.IsNullOrEmpty(property.stringValue) || !IsUnique(property.stringValue))
            {
                property.stringValue = System.Guid.NewGuid().ToString();
                serializedObject.ApplyModifiedProperties();
            }

            globalLookup[property.stringValue] = this;
        }

        bool IsUnique(string candidate)
        {
            if(!globalLookup.ContainsKey(candidate)) { return true; }

            if(globalLookup[candidate] == this) { return true; }

            if(globalLookup[candidate] == null)
            {
                globalLookup.Remove(candidate);
                return true; 
            }

            if(globalLookup[candidate].GetUniqueIdentifier() != candidate)
            {
                globalLookup.Remove(candidate);
                return true; 
            }

            return false;
        }
#endif
    }
}
