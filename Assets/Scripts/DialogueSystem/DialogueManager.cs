using System;
using System.Collections.Generic;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DialogueSystem
{
    [Serializable]
    public class ObjectiveDialogue
    {
        public string objectiveName;
        
        [SerializeField] private List<string> randomDialogue;
        private int lastDialogueIdx;

        [SerializeField] private List<string> objectiveDialoguesText;
        private int currentIdx;

        public string GetObjectiveDialogue()
        {
            if (currentIdx < objectiveDialoguesText.Count)
            {
                return objectiveDialoguesText[currentIdx++];
            }
            
            return GetRandomDialogue();
        }
        
        private string GetRandomDialogue()
        {
            int randomIndex = Random.Range(0, randomDialogue.Count);
            if (randomDialogue.Count > 1)
            {
                if (randomIndex == lastDialogueIdx)
                {
                    randomIndex = Random.Range(0, randomDialogue.Count);
                }
            }
            else
            {
                return "...";
            }
            
            lastDialogueIdx = randomIndex;
            return randomDialogue[randomIndex];
        }

        public bool IsInObjectiveDialogue()
        {
            return currentIdx < objectiveDialoguesText.Count;
        }
    }
    
    [Serializable] 
    public class Dialogue
    {
        public string speakerName;
    
        public Sprite expression1;
        public Sprite expression2;

        // Whenever the object is interacted with when NOT involved in an objective
        [SerializeField] private List<string> randomDialogue; 
        // Whenever the object is interacted with when involved in an objective
        [SerializeField] private List<ObjectiveDialogue> objectiveDialogues;

        public string currentObjective;
        public bool isInvolved;
        
        private int lastDialogueIdx;

        public string GetDialogue()
        {
            if (isInvolved)
            {
                if (GetCurrentObjective()  != null)
                {
                    return GetCurrentObjective().GetObjectiveDialogue();
                }
            }
            
            return GetRandomDialogue();
        }
        
        private string GetRandomDialogue()
        {
            int randomIndex = Random.Range(0, randomDialogue.Count);
            if (randomDialogue.Count > 1)
            {
                if (randomIndex == lastDialogueIdx)
                {
                    randomIndex = Random.Range(0, randomDialogue.Count);
                }
            }
            else
            {
                return "...";
            }
            
            lastDialogueIdx = randomIndex;
            return randomDialogue[randomIndex];
        }

        private ObjectiveDialogue GetCurrentObjective()
        {
            foreach (ObjectiveDialogue objectiveDialogue in objectiveDialogues)
            {
                if (objectiveDialogue.objectiveName == currentObjective)
                {
                    return objectiveDialogue;
                }
            }

            return null;
        }

        public bool CanRemoveDialogue()
        {
            if (isInvolved)
            {
                if (GetCurrentObjective()  != null)
                {
                   return !GetCurrentObjective().IsInObjectiveDialogue();
                }
            }
            
            return true;
        }
    }

    public class DialogueManager : MonoBehaviour
    {
        public DialogueBox dialogueBox;
        
        private Dialogue currentDialogue;
        private bool isDialogueActive;
        
        public void TriggerDialogue(Dialogue dialogue) // new
        {
            currentDialogue = dialogue;
            if (isDialogueActive && currentDialogue.CanRemoveDialogue())
            {
                DeactivateDialogue();
                isDialogueActive = false;
            }
            else
            {
                ActivateDialogue(dialogue);
                isDialogueActive = true;
            }
        }

        // Debugging test method
        public void ActivateDialogue(Dialogue dialogue) 
        {
            dialogueBox.ShowDialogueBox(dialogue);
            PlayerController.isInDialogue = true;
        }

        public void DeactivateDialogue()
        {
            dialogueBox.HideDialogueBox();
            PlayerController.isInDialogue = false;
        }
        
        public static DialogueManager GetDialogueManager()
        {
            return FindFirstObjectByType<DialogueManager>();
        }
    }
    
}
