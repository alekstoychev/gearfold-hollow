using System;
using System.Collections.Generic;
using Interact;
using ObjectiveSystem;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public enum Expression
{
    Expression1,
    Expression2,
    NoExpression
}

namespace DialogueSystem
{
    [Serializable]
    public struct DialogueText
    {
        public string text;
        public Expression expression;
    }
    
    [Serializable]
    public struct ObjectiveDialogueText
    {
        public DialogueText dialogueText;
        public float autoContinueDelay;
        public bool autoContinue;
        public bool completeObjectiveAfterText;
        
        public UnityEvent onDialogueContinue;
        
        public static implicit operator string(ObjectiveDialogueText objectiveDialogueText)
        {
            return objectiveDialogueText.dialogueText.text;
        }
    }
    
    [Serializable]
    public class ObjectiveDialogue
    {
        public string objectiveName;
        
        [SerializeField] private List<string> randomDialogue;
        private int lastDialogueIdx;

        [SerializeField] private List<ObjectiveDialogueText> objectiveDialoguesText;
        private int currentIdx = -1;

        public float GetWaitTimeAfterText()
        {
            return objectiveDialoguesText[currentIdx].autoContinueDelay;
        }

        public void TriggerEndOfDialogue()
        {
            objectiveDialoguesText[currentIdx].onDialogueContinue?.Invoke();
        }

        public bool GetCompleteObjectiveAfterText()
        {
            return objectiveDialoguesText[currentIdx].completeObjectiveAfterText;
        }

        public Expression GetExpression()
        {
            return objectiveDialoguesText[currentIdx].dialogueText.expression;
        }

        public bool GetContinueAfterText()
        {
            if (currentIdx >= 0 && currentIdx < objectiveDialoguesText.Count)
            {
                return objectiveDialoguesText[currentIdx].autoContinue;
            }
            
            return false;
        }

        public string GetNewObjectiveDialogue()
        {
            if (currentIdx < objectiveDialoguesText.Count)
            {
                return objectiveDialoguesText[++currentIdx];
            }
            
            Debug.LogError($"No objective dialogue available at {currentIdx}");
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
        #region Variables
        [Header("Character Name")]
        public string speakerName;
    
        [Header("Expression images")]
        public Sprite expression1;
        public Sprite expression2;
        
        [Header("Dialogues")]
        
        [Tooltip("Whenever the object is interacted with when NOT involved in an objective")]
        [SerializeField] private List<DialogueText> randomDialogue; 
        [Tooltip("Whenever the object is interacted with when involved in an objective")]
        [SerializeField] private List<ObjectiveDialogue> objectiveDialogues;

        public bool isInvolved;

        //public event Action OnObjectiveDialogueComplete;
        
        private int lastDialogueIdx;
        #endregion

        /*public void CompleteObjectiveDialogue()
        {
            OnObjectiveDialogueComplete?.Invoke();
        }*/

        public void TriggerEndOfDialogue()
        {
            GetCurrentObjective().TriggerEndOfDialogue();
        }

        public Sprite GetExpressionSprite()
        {
            Expression expression = randomDialogue[lastDialogueIdx].expression;
            
            if (GetCurrentObjective() != null)
            {
                expression = GetCurrentObjective().GetExpression();
            }
            
            switch (expression)
            {
                case Expression.Expression1:
                    return expression1;
                case Expression.Expression2:
                    return expression2;
                default:
                    return null;
            }
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
            return randomDialogue[randomIndex].text;
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
