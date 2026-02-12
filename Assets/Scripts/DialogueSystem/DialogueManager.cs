using System;
using System.Collections.Generic;
using Interact;
using ObjectiveSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DialogueSystem
{
    [Serializable]
    public class DialogueText
    {
        public string text;
        public float waitTimeAfterText;
        public bool continueAfterText;
        
        public static implicit operator string(DialogueText dialogueText)
        {
            return dialogueText.text;
        }
    }
    
    [Serializable]
    public class ObjectiveDialogue
    {
        public string objectiveName;
        
        [SerializeField] private List<string> randomDialogue;
        private int lastDialogueIdx;

        [SerializeField] private List<DialogueText> objectiveDialoguesText;
        private int currentIdx;

        public float GetWaitTimeAfterText()
        {
            return objectiveDialoguesText[currentIdx].waitTimeAfterText;
        }

        public bool GetContinueAfterText()
        {
            return objectiveDialoguesText[currentIdx].continueAfterText;
        }

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

        public bool isInvolved;
        
        private int lastDialogueIdx;

        public float GetWaitTimeAfterText()
        {
            return GetCurrentObjective().GetWaitTimeAfterText();
        }

        public bool GetContinueAfterText()
        {
            return GetCurrentObjective().GetContinueAfterText();
        }

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
                if (objectiveDialogue.objectiveName == ObjectiveManager.GetCurrentObjective())
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
        
        private bool isDialogueActive;

        private void Awake()
        {
            Interactable.OnInteract += TriggerDialogue;
        }
        
        public void TriggerDialogue(Dialogue dialogue) // new
        {
            if (isDialogueActive && dialogue.CanRemoveDialogue())
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

        public void ActivateDialogue(Dialogue dialogue) 
        {
            dialogueBox.ShowDialogueBox(dialogue);
        }

        public void DeactivateDialogue()
        {
            dialogueBox.HideDialogueBox();
        }
    }
    
}
