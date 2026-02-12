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

        public void ProgressObjective()
        {
            currentObjectiveIndex++;
            UpdatePeopleInvolved();
        }

        private void UpdatePeopleInvolved()
        {
            if (currentObjectiveIndex >= objectives.Count || currentObjectiveIndex < 0)
            {
                return;
            }
            
            Objective currentObjective = objectives[currentObjectiveIndex];

            Interact.Interactable[] foundInteractables = FindObjectsByType<Interact.Interactable>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            for (int i = 0; i < foundInteractables.Length; i++)
            {
                foundInteractables[i].dialogue.isInvolved = 
                    currentObjective.IsPersonInvolved(foundInteractables[i].dialogue.speakerName);
            }
        }
    }
}

