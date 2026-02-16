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
            return peopleInvolved.Contains(nameToFind);
        }
    }
    
    public class ObjectiveManager : MonoBehaviour
    {
        [SerializeField] private List<Objective> objectives = new();
        private int currentObjectiveIndex = 0;

        private void Awake()
        {
            UpdatePeopleInvolved();
        }

        public void DebugGetCurrentObjective(TMPro.TextMeshProUGUI objectiveText)
        {
            objectiveText.text = GetCurrentObjective();
        }
        
        public static string GetCurrentObjective()
        {
            ObjectiveManager objectiveManager = FindFirstObjectByType<ObjectiveManager>();
            
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
                foundListeners[i].InformAboutInvolvement(
                    currentObjective.IsPersonInvolved(foundListeners[i].AttachedNPC));
            }
        }
    }
}

