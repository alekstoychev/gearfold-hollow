using System.Collections.Generic;
using UnityEngine;

namespace ObjectiveSystem
{
    public class ObjectiveManager : MonoBehaviour
    {
        private List<string> objectives = new();
        private int currentObjectiveIndex = 0;

        public string GetCurrentObjective()
        {
            if (currentObjectiveIndex < objectives.Count)
            {
                return objectives[currentObjectiveIndex];
            }
            
            return "No objective";
        }

        public void ProgressObjective()
        {
            if (currentObjectiveIndex < objectives.Count)
            {
                currentObjectiveIndex++;   
            }
        }
    }
}

