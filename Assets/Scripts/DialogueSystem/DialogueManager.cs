using System;
using System.Collections.Generic;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DialogueSystem
{
    [Serializable]
    public struct ObjectiveDialogue
    {
        public string objectiveName;
        
        public List<string> randomDialogue;
        public int lastDialogueIdx;
        
        
        
        public string GetRandomDialogue()
        {
            int randomIndex = Random.Range(0, randomDialogue.Count);
            if (randomDialogue.Count > 1)
            {
                if (randomIndex == lastDialogueIdx)
                {
                    randomIndex = Random.Range(0, randomDialogue.Count);
                }
            }
            
            lastDialogueIdx = randomIndex;
            return randomDialogue[randomIndex];
        }
    }
    
    [Serializable] 
    public struct Dialogue
    {
        public string speakerName;
    
        public Sprite expression1;
        public Sprite expression2;

        // Whenever the object is interacted with when NOT involved in an objective
        public List<string> randomDialogue; 
        // Whenever the object is interacted with when involved in an objective
        public List<ObjectiveDialogue> objectiveDialogues;

        public string currentObjective;
        public bool isInvolved;
        
        public int lastDialogueIdx;

        public string GetRandomDialogue()
        {
            int randomIndex = Random.Range(0, randomDialogue.Count);
            if (randomDialogue.Count > 1)
            {
                if (randomIndex == lastDialogueIdx)
                {
                    randomIndex = Random.Range(0, randomDialogue.Count);
                }
            }
            
            lastDialogueIdx = randomIndex;
            return randomDialogue[randomIndex];
        }

        public string GetCurrentObjectiveRandomDialogue()
        {
            foreach (ObjectiveDialogue objectiveDialogue in objectiveDialogues)
            {
                if (objectiveDialogue.objectiveName == currentObjective)
                {
                    int index =  Random.Range(0, objectiveDialogue.randomDialogue.Count);
                    if (index == objectiveDialogue.lastDialogueIdx)
                    {
                        index = Random.Range(0, objectiveDialogue.randomDialogue.Count);
                    }
                    
                    lastDialogueIdx = index;
                    return objectiveDialogue.randomDialogue[index];
                }
            }
            
            return "";
        }
    }

    public class DialogueManager : MonoBehaviour
    {
        public DialogueBox dialogueBox;

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
