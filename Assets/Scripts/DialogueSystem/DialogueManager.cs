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
        public bool completeObjectiveAfterText;
        
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
            return objectiveDialoguesText[currentIdx-1].waitTimeAfterText;
        }
        
        public bool GetCompleteObjectiveAfterText()
        {
            return objectiveDialoguesText[currentIdx-1].completeObjectiveAfterText;
        }

        public bool GetContinueAfterText()
        {
            if (currentIdx < objectiveDialoguesText.Count)
            {
                return objectiveDialoguesText[currentIdx-1].continueAfterText;
            }
            
            return false;
        }

        public string GetNewObjectiveDialogue()
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

        public event Action OnObjectiveDialogueComplete;
        
        private int lastDialogueIdx;

        public void CompleteObjectiveDialogue()
        {
            OnObjectiveDialogueComplete?.Invoke();
        }

        public float GetWaitTimeAfterText()
        {
            if (GetCurrentObjective() != null)
            {
                return GetCurrentObjective().GetWaitTimeAfterText();
            }

            return 0f;
        }

        public bool GetContinueAfterText()
        {
            if (GetCurrentObjective() != null)
            {
                return GetCurrentObjective().GetContinueAfterText();
            }
            
            return false;
        }
        
        public bool GetCompleteObjectiveAfterText()
        {
            if (GetCurrentObjective() != null)
            {
                return GetCurrentObjective().GetCompleteObjectiveAfterText();
            }
            
            return false;
        }

        public string GetDialogue()
        {
            if (isInvolved)
            {
                if (GetCurrentObjective()  != null)
                {
                    return GetCurrentObjective().GetNewObjectiveDialogue();
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
            dialogueBox.OnContinueDialogue += OnContinueDialogueTriggered;
        }

        private void OnContinueDialogueTriggered(bool canContinue)
        {
            isDialogueActive = canContinue;
        }
        
        public void TriggerDialogue(Dialogue dialogue) // new
        {
            if (isDialogueActive)
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
