using System;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectiveSystem
{
    [Serializable] public struct Objective
    {
        public string name;
        public List<string> peopleInvolved;

        public static implicit operator string(Objective objective)
        {
            return objective.name;
        }

        public bool IsPersonInvolved(string nameToFind)
        {
            Debug.Log($"Is {nameToFind} involved: {peopleInvolved.Contains(nameToFind)}");
            return peopleInvolved.Contains(nameToFind);
        }
    }
    
    public class ObjectiveManager : MonoBehaviour
    {
        [SerializeField] public List<Objective> objectives = new();
        public int currentObjectiveIndex = 0;
        
        private static ObjectiveManager Instance { get; set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            
            UpdatePeopleInvolved();
        }

        public void DebugGetCurrentObjective(TMPro.TextMeshProUGUI objectiveText)
        {
            objectiveText.text = GetCurrentObjective();
        }
        
        public static string GetCurrentObjective()
        {
            ObjectiveManager objectiveManager = FindFirstObjectByType<ObjectiveManager>();
            if (!objectiveManager)
            {
                return "No objective";
            }
            
            if (objectiveManager.currentObjectiveIndex < objectiveManager.objectives.Count)
            {
                return objectiveManager.objectives[objectiveManager.currentObjectiveIndex];
            }
            
            return "No objective";
        }

        public static void ProgressObjective()
        {
            ObjectiveManager objectiveManager = FindFirstObjectByType<ObjectiveManager>();
            objectiveManager.currentObjectiveIndex++;
            objectiveManager.UpdatePeopleInvolved();
        }

        private void UpdatePeopleInvolved()
        {
            if (currentObjectiveIndex >= objectives.Count || currentObjectiveIndex < 0)
            {
                return;
            }
            
            Objective currentObjective = objectives[currentObjectiveIndex];

            ObjectiveListener[] foundListeners = FindObjectsByType<ObjectiveListener>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            for (int i = 0; i < foundListeners.Length; i++)
            {
                foundListeners[i].UpdateOwnerName(); // Update in case it hasnt found it yet
                
                Debug.Log($"Looking at NPC {foundListeners[i].AttachedNPC}");
                foundListeners[i].InformAboutInvolvement(
                    currentObjective.IsPersonInvolved(foundListeners[i].AttachedNPC));
            }
        }
    }
}

